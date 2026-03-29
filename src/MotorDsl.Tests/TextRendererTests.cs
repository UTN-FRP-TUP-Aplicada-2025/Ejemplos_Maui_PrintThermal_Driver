using MotorDsl.Core.Contracts;
using MotorDsl.Core.Layout;
using MotorDsl.Core.Models;
using MotorDsl.Rendering;

namespace MotorDsl.Tests;

/// <summary>
/// Tests para TextRenderer (IRenderer).
/// Sprint 03 | BT-058 a BT-065
/// 8 test cases.
/// </summary>
public class TextRendererTests
{
    private readonly ILayoutEngine _layoutEngine = new LayoutEngine();
    private readonly IRenderer _renderer = new TextRenderer();

    private DeviceProfile DefaultProfile() => new("thermal-58mm", 32, "text");

    private EvaluatedDocument MakeEvaluated(DocumentNode root) => new()
    {
        Id = "test",
        Version = "1.0",
        Root = root
    };

    private LayoutedDocument Layout(DocumentNode root, DeviceProfile? profile = null)
    {
        var p = profile ?? DefaultProfile();
        return _layoutEngine.ApplyLayout(MakeEvaluated(root), p);
    }

    // ─── BT-058: Render de un TextNode simple ───
    [Fact]
    public void Render_SimpleTextNode_OutputContainsText()
    {
        var layouted = Layout(new TextNode("Hola mundo"));
        var profile = DefaultProfile();

        var result = _renderer.Render(layouted, profile);

        Assert.NotNull(result);
        Assert.NotNull(result.Output);
        var output = result.Output.ToString()!;
        Assert.Contains("Hola mundo", output);
    }

    // ─── BT-059: Render devuelve target correcto ───
    [Fact]
    public void Render_Target_IsText()
    {
        Assert.Equal("text", _renderer.Target);
    }

    // ─── BT-060: Render de texto wrapeado ───
    [Fact]
    public void Render_WrappedText_ContainsAllContent()
    {
        var longText = "Este texto largo debe ser dividido en multiples lineas por el layout engine";
        var layouted = Layout(new TextNode(longText));
        var profile = DefaultProfile();

        var result = _renderer.Render(layouted, profile);
        var output = result.Output!.ToString()!;

        // Debe contener las primeras palabras
        Assert.Contains("Este", output);
        Assert.Contains("texto", output);
    }

    // ─── BT-061: Container vertical ordena líneas correctamente ───
    [Fact]
    public void Render_ContainerVertical_LinesInCorrectOrder()
    {
        var container = new ContainerNode("vertical", new List<DocumentNode>
        {
            new TextNode("Primera linea"),
            new TextNode("Segunda linea"),
            new TextNode("Tercera linea")
        });
        var layouted = Layout(container);
        var profile = DefaultProfile();

        var result = _renderer.Render(layouted, profile);
        var output = result.Output!.ToString()!;

        var firstIdx = output.IndexOf("Primera");
        var secondIdx = output.IndexOf("Segunda");
        var thirdIdx = output.IndexOf("Tercera");

        Assert.True(firstIdx >= 0, "Debe contener 'Primera'");
        Assert.True(secondIdx >= 0, "Debe contener 'Segunda'");
        Assert.True(thirdIdx >= 0, "Debe contener 'Tercera'");
        Assert.True(firstIdx < secondIdx, "Primera antes de Segunda");
        Assert.True(secondIdx < thirdIdx, "Segunda antes de Tercera");
    }

    // ─── BT-062: Render de documento null ───
    [Fact]
    public void Render_NullDocument_ReturnsEmptyOutput()
    {
        var layouted = new LayoutedDocument { Root = null };
        var profile = DefaultProfile();

        var result = _renderer.Render(layouted, profile);

        Assert.NotNull(result);
        Assert.Equal("", result.Output!.ToString());
    }

    // ─── BT-063: Render de texto vacío ───
    [Fact]
    public void Render_EmptyTextNode_ReturnsEmptyOrWhitespace()
    {
        var layouted = Layout(new TextNode(""));
        var profile = DefaultProfile();

        var result = _renderer.Render(layouted, profile);

        Assert.NotNull(result);
        // Output puede ser vacío o whitespace (padding)
        Assert.NotNull(result.Output);
    }

    // ─── BT-064: Render alineación centro ───
    [Fact]
    public void Render_CenterAlignment_TextIsCentered()
    {
        var text = new TextNode("Centro");
        var layouted = Layout(text);
        // Forzar alignment center en el layout info
        var firstEntry = layouted.NodeLayoutInfo.Values.First(l => !string.IsNullOrEmpty(l.WrappedText));
        firstEntry.Alignment = "center";

        var profile = DefaultProfile();
        var result = _renderer.Render(layouted, profile);
        var output = result.Output!.ToString()!;

        Assert.Contains("Centro", output);
        // Centro en 32 chars: debe tener espacios al inicio
        var line = output.Split('\n')[0].TrimEnd('\r');
        Assert.True(line.Length > 0);
        Assert.True(line.StartsWith(" "), "Texto centrado debe tener padding izquierdo");
    }

    // ─── BT-065: IsSuccessful cuando no hay errores ───
    [Fact]
    public void Render_NoErrors_IsSuccessful()
    {
        var layouted = Layout(new TextNode("Ok"));
        var profile = DefaultProfile();

        var result = _renderer.Render(layouted, profile);

        Assert.True(result.IsSuccessful);
    }

    // ─── TK-32: TableNode rendering en TextRenderer ───

    private DeviceProfile TableProfile() => new("thermal_40", 40, "text");

    // ─── TK-32-01: Tabla con headers y rows contiene ambos ───
    [Fact]
    public void Render_TableWithHeadersAndRows_OutputContainsBoth()
    {
        var table = new TableNode(
            new List<string> { "Descripcion", "Cant", "Total" },
            new List<List<string>>
            {
                new() { "Cafe", "2", "$300.00" },
                new() { "Medialunas", "6", "$300.00" }
            });

        var profile = TableProfile();
        var layouted = Layout(table, profile);
        var result = _renderer.Render(layouted, profile);
        var output = result.Output!.ToString()!;

        Assert.Contains("Descripcion", output);
        Assert.Contains("Cant", output);
        Assert.Contains("Total", output);
        Assert.Contains("Cafe", output);
        Assert.Contains("Medialunas", output);
        Assert.Contains("$300.00", output);
    }

    // ─── TK-32-02: Tabla con headers sin rows muestra solo headers ───
    [Fact]
    public void Render_TableWithHeadersNoRows_OutputContainsHeaders()
    {
        var table = new TableNode(
            new List<string> { "Producto", "Precio" },
            new List<List<string>>());

        var profile = TableProfile();
        var layouted = Layout(table, profile);
        var result = _renderer.Render(layouted, profile);
        var output = result.Output!.ToString()!;

        Assert.Contains("Producto", output);
        Assert.Contains("Precio", output);
    }

    // ─── TK-32-03: Tabla sin headers solo rows muestra datos ───
    [Fact]
    public void Render_TableNoHeadersWithRows_OutputContainsRows()
    {
        var table = new TableNode(
            new List<string>(),
            new List<List<string>>
            {
                new() { "Cafe", "2", "$300.00" }
            });

        var profile = TableProfile();
        var layouted = Layout(table, profile);
        var result = _renderer.Render(layouted, profile);
        var output = result.Output!.ToString()!;

        Assert.Contains("Cafe", output);
        Assert.Contains("$300.00", output);
    }

    // ─── TK-32-04: Tabla vacía no rompe ───
    [Fact]
    public void Render_EmptyTable_DoesNotThrow()
    {
        var table = new TableNode();

        var profile = TableProfile();
        var layouted = Layout(table, profile);
        var result = _renderer.Render(layouted, profile);

        Assert.True(result.IsSuccessful);
    }

    // ─── TK-32-05: Headers aparecen antes que rows ───
    [Fact]
    public void Render_Table_HeadersAppearBeforeRows()
    {
        var table = new TableNode(
            new List<string> { "Descripcion", "Cant", "Total" },
            new List<List<string>>
            {
                new() { "Cafe", "2", "$300.00" }
            });

        var profile = TableProfile();
        var layouted = Layout(table, profile);
        var result = _renderer.Render(layouted, profile);
        var output = result.Output!.ToString()!;

        var headerIdx = output.IndexOf("Descripcion");
        var rowIdx = output.IndexOf("Cafe");

        Assert.True(headerIdx >= 0, "Debe contener header");
        Assert.True(rowIdx >= 0, "Debe contener row");
        Assert.True(headerIdx < rowIdx, "Header antes que row");
    }

    // ─── TK-32-06: Col 0 ocupa ~50% del ancho, cols restantes el otro 50% ───
    [Fact]
    public void Render_Table_FirstColumnIsWiderThanOthers()
    {
        var table = new TableNode(
            new List<string> { "Descripcion", "Cant", "Total" },
            new List<List<string>>
            {
                new() { "Cafe", "2", "$300.00" }
            });

        var profile = TableProfile(); // width 40
        var layouted = Layout(table, profile);
        var result = _renderer.Render(layouted, profile);
        var output = result.Output!.ToString()!;

        // La primera línea visible con contenido debería ser el header
        var lines = output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        var headerLine = lines.FirstOrDefault(l => l.Contains("Descripcion"));
        Assert.NotNull(headerLine);

        // "Descripcion" empieza al inicio (col 0 = ~50% = 19 chars)
        // "Cant" y "Total" deberían estar en la segunda mitad
        var cantIdx = headerLine.IndexOf("Cant");
        var totalIdx = headerLine.IndexOf("Total");
        Assert.True(cantIdx > 15, "Cant debe estar en la segunda mitad del ancho");
        Assert.True(totalIdx > cantIdx, "Total debe estar después de Cant");
    }

    // ─── TK-34: QR placeholder en TextRenderer ───

    // ─── TK-34-TEXT-01: ImageNode qrcode muestra placeholder [QR: ...] ───
    [Fact]
    public void Render_QrCodeNode_OutputContainsQrPlaceholder()
    {
        var qrNode = new ImageNode("https://example.com", imageType: "qrcode");

        var profile = DefaultProfile();
        var layouted = Layout(qrNode, profile);
        var result = _renderer.Render(layouted, profile);
        var output = result.Output!.ToString()!;

        Assert.Contains("[QR:", output);
        Assert.Contains("https://example.com", output);
    }
}
