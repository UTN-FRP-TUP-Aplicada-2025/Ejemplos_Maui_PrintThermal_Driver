using MotorDsl.Core.Contracts;
using MotorDsl.Core.Engine;
using MotorDsl.Core.Evaluators;
using MotorDsl.Core.Layout;
using MotorDsl.Core.Models;
using MotorDsl.Core.Validation;
using MotorDsl.Parser;
using MotorDsl.Rendering;

namespace MotorDsl.Tests;

/// <summary>
/// TDD tests para CU-23 — Aplicar Restricciones del Dispositivo.
/// Semántica PERMISIVA: capability no declarada → soportado por defecto.
/// Solo restringe cuando se declara explícitamente false.
/// Tests escritos ANTES de la implementación.
/// </summary>
public class CU23CapabilityTests
{
    private readonly ILayoutEngine _layoutEngine = new LayoutEngine();

    private DeviceProfile DefaultProfile() => new("thermal-58mm", 32, "text");

    private EvaluatedDocument MakeEvaluated(DocumentNode root) => new()
    {
        Id = "test-cu23",
        Version = "1.0",
        Root = root
    };

    // ─── Test 1: QR + perfil con supports_qrcode=false → degradado + warning ───
    [Fact]
    public void ApplyLayout_QR_CapabilityFalse_DegradesToText_WithWarning()
    {
        var qrNode = new ImageNode("https://example.com/pay", imageType: "qrcode");
        var profile = DefaultProfile();
        profile.SetCapability("supports_qrcode", false);

        var result = _layoutEngine.ApplyLayout(MakeEvaluated(qrNode), profile);

        var layout = result.NodeLayoutInfo.Values.First();
        Assert.False(layout.DeviceMetadata.ContainsKey("is_qr"),
            "QR degradado no debe tener metadata 'is_qr'");
        Assert.Contains("https://example.com/pay", layout.WrappedText);
        Assert.NotEmpty(result.Warnings);
        Assert.Contains(result.Warnings, w => w.Contains("qr", StringComparison.OrdinalIgnoreCase));
    }

    // ─── Test 2: QR + perfil sin capability declarada → is_qr normal (permisivo) ───
    [Fact]
    public void ApplyLayout_QR_NoCapabilityDeclared_WorksNormally()
    {
        var qrNode = new ImageNode("https://example.com/pay", imageType: "qrcode");
        var profile = DefaultProfile(); // sin SetCapability

        var result = _layoutEngine.ApplyLayout(MakeEvaluated(qrNode), profile);

        var layout = result.NodeLayoutInfo.Values.First();
        Assert.True(layout.DeviceMetadata.ContainsKey("is_qr"),
            "QR debe funcionar normalmente cuando no hay capability declarada");
        Assert.True((bool)layout.DeviceMetadata["is_qr"]);
        Assert.Empty(result.Warnings);
    }

    // ─── Test 3: Barcode + perfil con supports_barcode=false → degradado + warning ───
    [Fact]
    public void ApplyLayout_Barcode_CapabilityFalse_DegradesToText_WithWarning()
    {
        var barcodeNode = new ImageNode("1234567890", imageType: "barcode");
        var profile = DefaultProfile();
        profile.SetCapability("supports_barcode", false);

        var result = _layoutEngine.ApplyLayout(MakeEvaluated(barcodeNode), profile);

        var layout = result.NodeLayoutInfo.Values.First();
        Assert.False(layout.DeviceMetadata.ContainsKey("is_barcode"),
            "Barcode degradado no debe tener metadata 'is_barcode'");
        Assert.Contains("1234567890", layout.WrappedText);
        Assert.NotEmpty(result.Warnings);
        Assert.Contains(result.Warnings, w => w.Contains("barcode", StringComparison.OrdinalIgnoreCase));
    }

    // ─── Test 4: Bitmap + perfil con supports_images=false → degradado + warning ───
    [Fact]
    public void ApplyLayout_Bitmap_CapabilityFalse_DegradesToText_WithWarning()
    {
        var bitmapNode = new ImageNode("data:image/png;base64,iVBORw0KGgo=");
        var profile = DefaultProfile();
        profile.SetCapability("supports_images", false);

        var result = _layoutEngine.ApplyLayout(MakeEvaluated(bitmapNode), profile);

        var layout = result.NodeLayoutInfo.Values.First();
        Assert.False(layout.DeviceMetadata.ContainsKey("is_bitmap"),
            "Bitmap degradado no debe tener metadata 'is_bitmap'");
        Assert.Contains("[IMG-DEGRADED:", layout.WrappedText);
        Assert.NotEmpty(result.Warnings);
        Assert.Contains(result.Warnings, w => w.Contains("image", StringComparison.OrdinalIgnoreCase));
    }

    // ─── Test 5: Bitmap + perfil sin capability → is_bitmap normal (permisivo) ───
    [Fact]
    public void ApplyLayout_Bitmap_NoCapabilityDeclared_WorksNormally()
    {
        var bitmapNode = new ImageNode("data:image/png;base64,iVBORw0KGgo=");
        var profile = DefaultProfile(); // sin SetCapability

        var result = _layoutEngine.ApplyLayout(MakeEvaluated(bitmapNode), profile);

        var layout = result.NodeLayoutInfo.Values.First();
        Assert.True(layout.DeviceMetadata.ContainsKey("is_bitmap"),
            "Bitmap debe funcionar normalmente cuando no hay capability declarada");
        Assert.True((bool)layout.DeviceMetadata["is_bitmap"]);
        Assert.Empty(result.Warnings);
    }

    // ─── Test 6: Pipeline completo — QR restrictivo → RenderResult.Warnings ───
    [Fact]
    public void FullPipeline_QR_Restricted_RenderResultContainsDegradationWarning()
    {
        var engine = CreateFullEngine();
        var dsl = """
        {
            "id": "cu23-pipeline",
            "version": "1.0",
            "root": {
                "type": "image",
                "source": "https://example.com/pay",
                "imageType": "qrcode"
            }
        }
        """;
        var data = new { };
        var profile = DefaultProfile();
        profile.SetCapability("supports_qrcode", false);

        var result = engine.Render(dsl, data, profile);

        Assert.True(result.IsSuccessful, "Pipeline debe completar sin errores");
        Assert.NotEmpty(result.Warnings);
        Assert.Contains(result.Warnings, w => w.Contains("qr", StringComparison.OrdinalIgnoreCase));
    }

    // ─── Test 7: Perfil con todas las capabilities en false → 3 warnings ───
    [Fact]
    public void ApplyLayout_AllCapabilitiesFalse_AllNodesDegraded_ThreeWarnings()
    {
        var container = new ContainerNode();
        container.Children = new List<DocumentNode>
        {
            new ImageNode("https://example.com/pay", imageType: "qrcode"),
            new ImageNode("1234567890", imageType: "barcode"),
            new ImageNode("data:image/png;base64,iVBORw0KGgo=")
        };

        var profile = DefaultProfile();
        profile.SetCapability("supports_qrcode", false);
        profile.SetCapability("supports_barcode", false);
        profile.SetCapability("supports_images", false);

        var result = _layoutEngine.ApplyLayout(MakeEvaluated(container), profile);

        // Verificar que ninguno tiene metadata de imagen activa
        var imageLayouts = result.NodeLayoutInfo.Values
            .Where(l => !string.IsNullOrEmpty(l.WrappedText))
            .ToList();

        Assert.DoesNotContain(imageLayouts, l => l.DeviceMetadata.ContainsKey("is_qr"));
        Assert.DoesNotContain(imageLayouts, l => l.DeviceMetadata.ContainsKey("is_barcode"));
        Assert.DoesNotContain(imageLayouts, l => l.DeviceMetadata.ContainsKey("is_bitmap"));

        Assert.Equal(3, result.Warnings.Count);
    }

    // ── Helper: construir DocumentEngine completo para test pipeline ──
    private DocumentEngine CreateFullEngine()
    {
        var parser = new DslParser();
        var evaluator = new Evaluator();
        var layoutEngine = new LayoutEngine();
        var registry = new RendererRegistry();
        registry.Register(new TextRenderer());
        var dataValidator = new DataValidator();
        var templateValidator = new TemplateValidator();
        var profileValidator = new ProfileValidator();

        return new DocumentEngine(parser, evaluator, layoutEngine, registry,
            dataValidator, templateValidator, profileValidator);
    }
}
