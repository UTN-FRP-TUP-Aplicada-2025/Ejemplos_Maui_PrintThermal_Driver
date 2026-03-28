using MotorDsl.Core.Contracts;
using MotorDsl.Core.Layout;
using MotorDsl.Core.Models;
using MotorDsl.Rendering;
using System.Text;

namespace MotorDsl.Tests;

/// <summary>
/// Tests para EscPosRenderer (IRenderer, target "escpos").
/// Sprint 04 | TK-20, TK-21, TK-26
/// Valida generación de byte[] ESC/POS sin hardware.
/// 6 tests.
/// </summary>
public class EscPosRendererTests
{
    private readonly ILayoutEngine _layoutEngine = new LayoutEngine();

    private DeviceProfile EscPosProfile() => new("thermal-80mm", 48, "escpos");

    private EvaluatedDocument MakeEvaluated(DocumentNode root) => new()
    {
        Id = "test",
        Version = "1.0",
        Root = root
    };

    private LayoutedDocument Layout(DocumentNode root)
    {
        return _layoutEngine.ApplyLayout(MakeEvaluated(root), EscPosProfile());
    }

    // ─── 1. Target == "escpos" ───
    [Fact]
    public void Target_IsEscPos()
    {
        var renderer = new EscPosRenderer();

        Assert.Equal("escpos", renderer.Target);
    }

    // ─── 2. Output es byte[] (no string) ───
    [Fact]
    public void Render_SimpleText_OutputIsByteArray()
    {
        var renderer = new EscPosRenderer();
        var layouted = Layout(new TextNode("Hola"));

        var result = renderer.Render(layouted, EscPosProfile());

        Assert.NotNull(result.Output);
        Assert.IsType<byte[]>(result.Output);
    }

    // ─── 3. Primer byte es 0x1B (ESC @, inicialización) ───
    [Fact]
    public void Render_SimpleText_StartsWithEscInit()
    {
        var renderer = new EscPosRenderer();
        var layouted = Layout(new TextNode("Test"));

        var result = renderer.Render(layouted, EscPosProfile());
        var bytes = (byte[])result.Output!;

        Assert.True(bytes.Length >= 2, "Debe tener al menos ESC @");
        Assert.Equal(0x1B, bytes[0]); // ESC
        Assert.Equal(0x40, bytes[1]); // @ (initialize)
    }

    // ─── 4. TextNode "Hola" → output contiene bytes ASCII de "Hola" ───
    [Fact]
    public void Render_TextNodeHola_BytesContainAsciiHola()
    {
        var renderer = new EscPosRenderer();
        var layouted = Layout(new TextNode("Hola"));

        var result = renderer.Render(layouted, EscPosProfile());
        var bytes = (byte[])result.Output!;

        var expected = Encoding.ASCII.GetBytes("Hola");
        Assert.True(ContainsSequence(bytes, expected),
            "Los bytes deben contener 'Hola' en ASCII");
    }

    // ─── 5. Último comando es GS V (corte de papel: 0x1D 0x56) ───
    [Fact]
    public void Render_SimpleText_EndsWithPaperCut()
    {
        var renderer = new EscPosRenderer();
        var layouted = Layout(new TextNode("Ticket"));

        var result = renderer.Render(layouted, EscPosProfile());
        var bytes = (byte[])result.Output!;

        Assert.True(bytes.Length >= 3, "Debe tener al menos init + cut");
        // GS V m — últimos 3 bytes: 0x1D, 0x56, m (0x00 = corte total)
        Assert.Equal(0x1D, bytes[^3]); // GS
        Assert.Equal(0x56, bytes[^2]); // V
        Assert.Equal(0x00, bytes[^1]); // corte total
    }

    // ─── 6. RenderResult.IsSuccessful == true en caso feliz ───
    [Fact]
    public void Render_HappyPath_IsSuccessful()
    {
        var renderer = new EscPosRenderer();
        var layouted = Layout(new TextNode("Ok"));

        var result = renderer.Render(layouted, EscPosProfile());

        Assert.True(result.IsSuccessful);
        Assert.Empty(result.Errors);
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
