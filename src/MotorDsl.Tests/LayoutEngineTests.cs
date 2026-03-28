using MotorDsl.Core.Contracts;
using MotorDsl.Core.Layout;
using MotorDsl.Core.Models;

namespace MotorDsl.Tests;

/// <summary>
/// Tests para LayoutEngine (ILayoutEngine).
/// Sprint 03 | BT-050 a BT-057
/// 8 test cases.
/// </summary>
public class LayoutEngineTests
{
    private readonly ILayoutEngine _layoutEngine = new LayoutEngine();

    private DeviceProfile DefaultProfile() => new("thermal-58mm", 32, "text");
    private DeviceProfile WideProfile() => new("thermal-80mm", 48, "text");
    private DeviceProfile NarrowProfile() => new("thermal-narrow", 20, "text");

    private EvaluatedDocument MakeEvaluated(DocumentNode root) => new()
    {
        Id = "test",
        Version = "1.0",
        Root = root
    };

    // ─── BT-050: Layout de un TextNode simple ───
    [Fact]
    public void ApplyLayout_SimpleTextNode_PopulatesLayoutInfo()
    {
        var text = new TextNode("Hola mundo");
        var doc = MakeEvaluated(text);

        var result = _layoutEngine.ApplyLayout(doc, DefaultProfile());

        Assert.NotNull(result);
        Assert.NotNull(result.NodeLayoutInfo);
        Assert.True(result.NodeLayoutInfo.Count > 0, "Debe haber al menos un LayoutInfo");
        var firstLayout = result.NodeLayoutInfo.Values.First();
        Assert.Equal("Hola mundo", firstLayout.WrappedText);
    }

    // ─── BT-051: Text wrap cuando el texto excede el ancho ───
    [Fact]
    public void ApplyLayout_LongText_WrapsToMultipleLines()
    {
        var longText = "Este texto es demasiado largo para caber en una sola linea de impresora termica";
        var text = new TextNode(longText);
        var doc = MakeEvaluated(text);

        var result = _layoutEngine.ApplyLayout(doc, DefaultProfile());

        var layout = result.NodeLayoutInfo.Values.First();
        Assert.True(layout.IsWrapped, "Texto largo debe marcarse como wrapped");
        Assert.True(layout.WrappedText.Contains("\n"), "Texto wrapped debe tener saltos de línea");
        Assert.True(layout.Height > 1, "Height debe ser > 1 para texto wrapped");
    }

    // ─── BT-052: Container vertical calcula posiciones ───
    [Fact]
    public void ApplyLayout_VerticalContainer_ChildrenOnConsecutiveLines()
    {
        var container = new ContainerNode("vertical", new List<DocumentNode>
        {
            new TextNode("Línea 1"),
            new TextNode("Línea 2"),
            new TextNode("Línea 3")
        });
        var doc = MakeEvaluated(container);

        var result = _layoutEngine.ApplyLayout(doc, DefaultProfile());

        // Debe haber layoutInfo para cada TextNode
        var textLayouts = result.NodeLayoutInfo.Values
            .Where(l => !string.IsNullOrEmpty(l.WrappedText))
            .OrderBy(l => l.LineNumber)
            .ToList();

        Assert.Equal(3, textLayouts.Count);
        // Las líneas deben ser consecutivas
        Assert.True(textLayouts[0].LineNumber < textLayouts[1].LineNumber);
        Assert.True(textLayouts[1].LineNumber < textLayouts[2].LineNumber);
    }

    // ─── BT-053: TotalWidth respeta perfil ───
    [Fact]
    public void ApplyLayout_RespectsDeviceWidth()
    {
        var text = new TextNode("Test");
        var doc = MakeEvaluated(text);
        var profile = WideProfile();

        var result = _layoutEngine.ApplyLayout(doc, profile);

        Assert.Equal(48, result.TotalWidth);
    }

    // ─── BT-054: Nodo condicional ───
    [Fact]
    public void ApplyLayout_ConditionalNode_LayoutsTrueBranch()
    {
        var cond = new ConditionalNode("visible == true",
            trueBranch: new TextNode("Visible"));
        var doc = MakeEvaluated(cond);

        var result = _layoutEngine.ApplyLayout(doc, DefaultProfile());

        // Debe haber layout del nodo conditional + su trueBranch
        Assert.True(result.NodeLayoutInfo.Count >= 2);
    }

    // ─── BT-055: Texto vacío no genera WrappedText ───
    [Fact]
    public void ApplyLayout_EmptyTextNode_EmptyWrappedText()
    {
        var text = new TextNode("");
        var doc = MakeEvaluated(text);

        var result = _layoutEngine.ApplyLayout(doc, DefaultProfile());

        var layout = result.NodeLayoutInfo.Values.First();
        Assert.Equal("", layout.WrappedText);
    }

    // ─── BT-056: Perfil angosto wrappea más ───
    [Fact]
    public void ApplyLayout_NarrowProfile_MoreWrapping()
    {
        var text = new TextNode("Texto que cabe en 32 pero no en 20");
        var doc = MakeEvaluated(text);

        var wideResult = _layoutEngine.ApplyLayout(doc, DefaultProfile());
        var narrowResult = _layoutEngine.ApplyLayout(doc, NarrowProfile());

        var wideLayout = wideResult.NodeLayoutInfo.Values.First();
        var narrowLayout = narrowResult.NodeLayoutInfo.Values.First();

        Assert.True(narrowLayout.Height >= wideLayout.Height,
            "Perfil angosto debe generar más líneas o iguales");
    }

    // ─── BT-057: TotalHeight se calcula correctamente ───
    [Fact]
    public void ApplyLayout_MultipleNodes_TotalHeightCorrect()
    {
        var container = new ContainerNode("vertical", new List<DocumentNode>
        {
            new TextNode("Línea A"),
            new TextNode("Línea B")
        });
        var doc = MakeEvaluated(container);

        var result = _layoutEngine.ApplyLayout(doc, DefaultProfile());

        Assert.True(result.TotalHeight >= 2, "TotalHeight debe ser >= 2 para 2 TextNodes");
    }
}
