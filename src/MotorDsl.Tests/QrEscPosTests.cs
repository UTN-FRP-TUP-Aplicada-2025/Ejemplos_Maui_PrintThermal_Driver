using MotorDsl.Core.Contracts;
using MotorDsl.Core.Layout;
using MotorDsl.Core.Models;
using MotorDsl.Rendering;
using System.Text;

namespace MotorDsl.Tests;

/// <summary>
/// Tests para QR code rendering en EscPosRenderer.
/// Sprint 05 | TK-34
/// 3 test cases.
/// </summary>
public class QrEscPosTests
{
    private readonly ILayoutEngine _layoutEngine = new LayoutEngine();
    private readonly EscPosRenderer _renderer = new();

    private DeviceProfile QrProfile() => new("thermal_58mm", 32, "escpos");

    private EvaluatedDocument MakeEvaluated(DocumentNode root) => new()
    {
        Id = "test",
        Version = "1.0",
        Root = root
    };

    private LayoutedDocument Layout(DocumentNode root)
    {
        return _layoutEngine.ApplyLayout(MakeEvaluated(root), QrProfile());
    }

    // ─── TK-34-01: QR genera byte[] con secuencia GS ( k ───
    [Fact]
    public void Render_QrCodeNode_ContainsGsParenK()
    {
        var qrNode = new ImageNode("https://example.com", imageType: "qrcode");

        var layouted = Layout(qrNode);
        var result = _renderer.Render(layouted, QrProfile());

        Assert.NotNull(result.Output);
        Assert.IsType<byte[]>(result.Output);

        var bytes = (byte[])result.Output;
        // GS ( k = 0x1D 0x28 0x6B
        Assert.True(ContainsSequence(bytes, new byte[] { 0x1D, 0x28, 0x6B }),
            "Debe contener secuencia GS ( k (0x1D 0x28 0x6B)");
    }

    // ─── TK-34-02: QR contiene bytes ASCII de la URL ───
    [Fact]
    public void Render_QrCodeNode_ContainsUrlBytes()
    {
        var url = "https://example.com";
        var qrNode = new ImageNode(url, imageType: "qrcode");

        var layouted = Layout(qrNode);
        var result = _renderer.Render(layouted, QrProfile());
        var bytes = (byte[])result.Output!;

        var urlBytes = Encoding.ASCII.GetBytes(url);
        Assert.True(ContainsSequence(bytes, urlBytes),
            $"Debe contener bytes ASCII de '{url}'");
    }

    // ─── TK-34-03: QR termina con print command ───
    [Fact]
    public void Render_QrCodeNode_ContainsPrintCommand()
    {
        var qrNode = new ImageNode("https://example.com", imageType: "qrcode");

        var layouted = Layout(qrNode);
        var result = _renderer.Render(layouted, QrProfile());
        var bytes = (byte[])result.Output!;

        // Print QR: 1D 28 6B 03 00 31 51 30
        var printCmd = new byte[] { 0x1D, 0x28, 0x6B, 0x03, 0x00, 0x31, 0x51, 0x30 };
        Assert.True(ContainsSequence(bytes, printCmd),
            "Debe contener comando de print QR (1D 28 6B 03 00 31 51 30)");
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
