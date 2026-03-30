using MotorDsl.Core.Contracts;
using MotorDsl.Core.Layout;
using MotorDsl.Core.Models;
using MotorDsl.Rendering;
using System.Text;

namespace MotorDsl.Tests;

/// <summary>
/// Tests para Barcode EAN-13 en ESC/POS y TextRenderer.
/// Sprint 06 | TK-43, TK-44
/// 3 test cases.
/// </summary>
public class BarcodeEscPosTests
{
    private readonly ILayoutEngine _layoutEngine = new LayoutEngine();

    private DeviceProfile EscPosProfile() => new("thermal-80mm", 48, "escpos");
    private DeviceProfile TextProfile() => new("thermal-58mm", 32, "text");

    private EvaluatedDocument MakeEvaluated(DocumentNode root) => new()
    {
        Id = "test",
        Version = "1.0",
        Root = root
    };

    private LayoutedDocument Layout(DocumentNode root, DeviceProfile? profile = null)
    {
        var p = profile ?? EscPosProfile();
        return _layoutEngine.ApplyLayout(MakeEvaluated(root), p);
    }

    // ─── TK-43-01: Barcode genera byte[] con GS k (0x1D 0x6B) ───
    [Fact]
    public void Render_BarcodeEan13_ContainsGsK()
    {
        var barcodeNode = new ImageNode("779123456789", imageType: "barcode");
        var renderer = new EscPosRenderer();
        var layouted = Layout(barcodeNode);

        var result = renderer.Render(layouted, EscPosProfile());
        var bytes = (byte[])result.Output!;

        Assert.True(ContainsSequence(bytes, new byte[] { 0x1D, 0x6B }),
            "Debe contener GS k (0x1D 0x6B) para barcode EAN-13");
    }

    // ─── TK-43-02: Barcode contiene los dígitos ASCII del código ───
    [Fact]
    public void Render_BarcodeEan13_ContainsAsciiDigits()
    {
        var code = "779123456789";
        var barcodeNode = new ImageNode(code, imageType: "barcode");
        var renderer = new EscPosRenderer();
        var layouted = Layout(barcodeNode);

        var result = renderer.Render(layouted, EscPosProfile());
        var bytes = (byte[])result.Output!;

        var expected = Encoding.ASCII.GetBytes(code);
        Assert.True(ContainsSequence(bytes, expected),
            "Debe contener los dígitos ASCII del código EAN-13");
    }

    // ─── TK-44-01: TextRenderer con ImageNode barcode → output contiene "[BARCODE:" ───
    [Fact]
    public void Render_BarcodeNode_TextRenderer_ContainsBarcodePlaceholder()
    {
        var barcodeNode = new ImageNode("779123456789", imageType: "barcode");
        var renderer = new TextRenderer();
        var layouted = Layout(barcodeNode, TextProfile());

        var result = renderer.Render(layouted, TextProfile());
        var output = result.Output!.ToString()!;

        Assert.Contains("[BARCODE:", output);
        Assert.Contains("779123456789", output);
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
