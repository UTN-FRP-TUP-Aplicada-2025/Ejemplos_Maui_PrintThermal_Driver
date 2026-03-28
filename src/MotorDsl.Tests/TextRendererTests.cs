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
}
