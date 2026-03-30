using MotorDsl.Core.Contracts;
using MotorDsl.Core.Engine;
using MotorDsl.Core.Evaluators;
using MotorDsl.Core.Layout;
using MotorDsl.Core.Models;
using MotorDsl.Core.Validation;
using MotorDsl.Parser;
using MotorDsl.Rendering;
using System.Text;

namespace MotorDsl.Tests;

/// <summary>
/// Tests de integración Sprint 06.
/// Verifican el pipeline completo con las features nuevas:
/// validación, barcode, QR, RenderLayout.
/// Sprint 06 | TK-47
/// 5 test cases.
/// </summary>
public class Sprint06IntegrationTests
{
    private IDocumentEngine CreateEngine()
    {
        var registry = new RendererRegistry();
        registry.Register(new TextRenderer());
        registry.Register(new EscPosRenderer());
        return new DocumentEngine(
            new DslParser(),
            new Evaluator(),
            new LayoutEngine(),
            registry,
            new DataValidator()
        );
    }

    private DeviceProfile EscPosProfile() => new("thermal-58mm", 32, "escpos");
    private DeviceProfile TextProfile() => new("thermal-58mm", 32, "text");

    // ─── 1. Pipeline completo con validación fallida → errores, no llega al rendering ───
    [Fact]
    public void Pipeline_ValidationFailed_ReturnsErrorsWithoutRendering()
    {
        var engine = CreateEngine();
        var dsl = """
        {
            "id": "int-001",
            "version": "1.0",
            "root": {
                "type": "container",
                "layout": "vertical",
                "children": [
                    { "type": "text", "text": "Cliente: {{nombre}}" },
                    { "type": "text", "text": "Total: {{total}}" }
                ]
            }
        }
        """;
        // Datos vacíos — campos "nombre" y "total" faltantes
        var data = new Dictionary<string, object>();

        var result = engine.Render(dsl, data, TextProfile());

        Assert.False(result.IsSuccessful);
        Assert.True(result.Errors.Count >= 2, "Debe reportar al menos 2 campos faltantes");
        Assert.Contains(result.Errors, e => e.Contains("nombre"));
        Assert.Contains(result.Errors, e => e.Contains("total"));
        Assert.Equal("", result.Output?.ToString());
    }

    // ─── 2. Pipeline completo con barcode → byte[] contiene GS k ───
    [Fact]
    public void Pipeline_BarcodeEan13_OutputContainsGsK()
    {
        var engine = CreateEngine();
        var dsl = """
        {
            "id": "int-002",
            "version": "1.0",
            "root": {
                "type": "container",
                "layout": "vertical",
                "children": [
                    { "type": "text", "text": "Comprobante: {{nroComprobante}}" },
                    { "type": "image", "source": "779123456789", "imageType": "barcode" }
                ]
            }
        }
        """;
        var data = new Dictionary<string, object>
        {
            { "nroComprobante", "A-0001-00012345" }
        };

        var result = engine.Render(dsl, data, EscPosProfile());

        Assert.True(result.IsSuccessful);
        var bytes = (byte[])result.Output!;
        Assert.True(ContainsSequence(bytes, new byte[] { 0x1D, 0x6B }),
            "Debe contener GS k (0x1D 0x6B) para barcode EAN-13");
        Assert.True(ContainsSequence(bytes, Encoding.ASCII.GetBytes("779123456789")),
            "Debe contener los dígitos ASCII del código");
    }

    // ─── 3. Pipeline completo con QR → byte[] contiene GS ( k ───
    [Fact]
    public void Pipeline_QrCode_OutputContainsGsParenK()
    {
        var engine = CreateEngine();
        var dsl = """
        {
            "id": "int-003",
            "version": "1.0",
            "root": {
                "type": "container",
                "layout": "vertical",
                "children": [
                    { "type": "text", "text": "Verificá en:" },
                    { "type": "image", "source": "https://factura.ar/v/12345", "imageType": "qrcode" }
                ]
            }
        }
        """;
        var data = new { };

        var result = engine.Render(dsl, data, EscPosProfile());

        Assert.True(result.IsSuccessful);
        var bytes = (byte[])result.Output!;
        // GS ( k = 0x1D 0x28 0x6B
        Assert.True(ContainsSequence(bytes, new byte[] { 0x1D, 0x28, 0x6B }),
            "Debe contener GS ( k para QR code");
        Assert.True(ContainsSequence(bytes, Encoding.ASCII.GetBytes("https://factura.ar/v/12345")),
            "Debe contener la URL del QR");
    }

    // ─── 4. RenderLayout devuelve LayoutedDocument sin rendering ───
    [Fact]
    public void RenderLayout_ReturnsLayoutedDocument_WithoutRendering()
    {
        var engine = CreateEngine();
        var dsl = """
        {
            "id": "int-004",
            "version": "1.0",
            "root": {
                "type": "container",
                "layout": "vertical",
                "children": [
                    { "type": "text", "text": "Linea 1" },
                    { "type": "text", "text": "Linea 2" }
                ]
            }
        }
        """;

        var layouted = engine.RenderLayout(dsl, new { }, TextProfile());

        Assert.NotNull(layouted);
        Assert.NotNull(layouted.Root);
        Assert.True(layouted.NodeLayoutInfo.Count > 0, "Debe tener layout info calculado");
        // Verificar que hay WrappedText con contenido
        var texts = layouted.NodeLayoutInfo.Values
            .Where(li => !string.IsNullOrEmpty(li.WrappedText))
            .Select(li => li.WrappedText)
            .ToList();
        Assert.Contains(texts, t => t.Contains("Linea 1"));
        Assert.Contains(texts, t => t.Contains("Linea 2"));
    }

    // ─── 5. DataValidator detecta campo faltante en template complejo ───
    [Fact]
    public void Pipeline_ComplexTemplate_MissingFields_ReturnsValidationErrors()
    {
        var engine = CreateEngine();
        var dsl = """
        {
            "id": "int-005",
            "version": "1.0",
            "root": {
                "type": "container",
                "layout": "vertical",
                "children": [
                    { "type": "text", "text": "Comercio: {{comercio}}" },
                    {
                        "type": "loop",
                        "source": "items",
                        "itemAlias": "item",
                        "body": { "type": "text", "text": "{{item.desc}} x{{item.cant}}" }
                    },
                    {
                        "type": "conditional",
                        "expression": "descuento > 0",
                        "trueBranch": { "type": "text", "text": "Desc: {{descuento}}" },
                        "falseBranch": { "type": "text", "text": "Sin descuento" }
                    },
                    { "type": "text", "text": "Total: {{total}}" }
                ]
            }
        }
        """;
        // Solo proveemos "items" — faltan "comercio", "descuento" y "total"
        var data = new Dictionary<string, object>
        {
            { "items", new[] { new { desc = "Café", cant = 2 } } }
        };

        var result = engine.Render(dsl, data, TextProfile());

        Assert.False(result.IsSuccessful);
        Assert.Contains(result.Errors, e => e.Contains("comercio"));
        Assert.Contains(result.Errors, e => e.Contains("total"));
    }

    // ─── Helper: busca una subsecuencia dentro de un byte[] ───
    private static bool ContainsSequence(byte[] source, byte[] pattern)
    {
        for (int i = 0; i <= source.Length - pattern.Length; i++)
        {
            bool match = true;
            for (int j = 0; j < pattern.Length; j++)
            {
                if (source[i + j] != pattern[j])
                {
                    match = false;
                    break;
                }
            }
            if (match) return true;
        }
        return false;
    }
}
