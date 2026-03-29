using MotorDsl.Core.Contracts;
using MotorDsl.Core.Layout;
using MotorDsl.Core.Models;
using MotorDsl.Rendering;
using System.Text;

namespace MotorDsl.Tests;

/// <summary>
/// Tests para TableNode rendering en EscPosRenderer.
/// Sprint 05 | TK-33
/// 4 test cases.
/// </summary>
public class TableNodeEscPosTests
{
    private readonly ILayoutEngine _layoutEngine = new LayoutEngine();
    private readonly EscPosRenderer _renderer = new();

    private DeviceProfile TableProfile() => new("thermal_40", 40, "escpos");

    private EvaluatedDocument MakeEvaluated(DocumentNode root) => new()
    {
        Id = "test",
        Version = "1.0",
        Root = root
    };

    private LayoutedDocument Layout(DocumentNode root)
    {
        return _layoutEngine.ApplyLayout(MakeEvaluated(root), TableProfile());
    }

    // ─── TK-33-01: Tabla genera byte[] válido con Init y Cut ───
    [Fact]
    public void Render_TableNode_OutputIsByteArrayWithInitAndCut()
    {
        var table = new TableNode(
            new List<string> { "Descripcion", "Cant", "Total" },
            new List<List<string>>
            {
                new() { "Cafe", "2", "$300.00" }
            });

        var layouted = Layout(table);
        var result = _renderer.Render(layouted, TableProfile());

        Assert.NotNull(result.Output);
        Assert.IsType<byte[]>(result.Output);

        var bytes = (byte[])result.Output;
        // Starts with ESC @ (Init)
        Assert.Equal(0x1B, bytes[0]);
        Assert.Equal(0x40, bytes[1]);
        // Ends with GS V 0 (Cut)
        Assert.Equal(0x1D, bytes[^3]);
        Assert.Equal(0x56, bytes[^2]);
        Assert.Equal(0x00, bytes[^1]);
    }

    // ─── TK-33-02: Tabla contiene ASCII de headers y datos ───
    [Fact]
    public void Render_TableNode_BytesContainHeaderAndRowData()
    {
        var table = new TableNode(
            new List<string> { "Descripcion", "Total" },
            new List<List<string>>
            {
                new() { "Cafe", "$300.00" }
            });

        var layouted = Layout(table);
        var result = _renderer.Render(layouted, TableProfile());
        var bytes = (byte[])result.Output!;

        Assert.True(ContainsSequence(bytes, Encoding.ASCII.GetBytes("Descripcion")),
            "Debe contener 'Descripcion' en ASCII");
        Assert.True(ContainsSequence(bytes, Encoding.ASCII.GetBytes("Cafe")),
            "Debe contener 'Cafe' en ASCII");
        Assert.True(ContainsSequence(bytes, Encoding.ASCII.GetBytes("$300.00")),
            "Debe contener '$300.00' en ASCII");
    }

    // ─── TK-33-03: Tabla vacía no rompe, IsSuccessful ───
    [Fact]
    public void Render_EmptyTable_IsSuccessful()
    {
        var table = new TableNode();

        var layouted = Layout(table);
        var result = _renderer.Render(layouted, TableProfile());

        Assert.True(result.IsSuccessful);
        Assert.IsType<byte[]>(result.Output);
    }

    // ─── TK-33-04: Tabla con header bold contiene StyleBold bytes ───
    [Fact]
    public void Render_TableWithBoldStyle_ContainsBoldCommand()
    {
        var table = new TableNode(
            new List<string> { "Producto", "Precio" },
            new List<List<string>>
            {
                new() { "Cafe", "$150.00" }
            });
        table.Style = new StyleDefinition();
        table.Style.Attributes["bold"] = "true";

        var layouted = Layout(table);
        var result = _renderer.Render(layouted, TableProfile());
        var bytes = (byte[])result.Output!;

        // ESC ! 8 = bold on (0x1B, 0x21, 0x08)
        Assert.True(ContainsSequence(bytes, new byte[] { 0x1B, 0x21, 0x08 }),
            "Debe contener comando ESC ! 8 (bold on)");
    }

    // ─── Helper: busca subsecuencia en byte[] ───
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
