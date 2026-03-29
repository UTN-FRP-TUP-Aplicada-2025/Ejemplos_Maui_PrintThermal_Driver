using MotorDsl.Core.Contracts;
using MotorDsl.Core.Models;
using System.Text;

namespace MotorDsl.Rendering;

/// <summary>
/// Renderer that generates ESC/POS byte[] commands for thermal printers.
/// No external dependencies (no ESCPOS_NET).
/// Sprint 04 | TK-20
/// </summary>
public class EscPosRenderer : IRenderer
{
    public string Target => "escpos";

    public RenderResult Render(LayoutedDocument document, DeviceProfile profile)
    {
        try
        {
            var result = new RenderResult("escpos");
            var buffer = new List<byte>();

            // Initialize printer
            buffer.AddRange(EscPosCommands.Init);

            if (document?.Root != null)
            {
                var entries = document.NodeLayoutInfo
                    .Where(kvp => !string.IsNullOrEmpty(kvp.Value.WrappedText))
                    .OrderBy(kvp => kvp.Value.LineNumber)
                    .ThenBy(kvp => kvp.Value.ColumnNumber);

                foreach (var kvp in entries)
                {
                    var layoutInfo = kvp.Value;

                    // Emit alignment command
                    buffer.AddRange(GetAlignmentCommand(layoutInfo.Alignment));

                    // Bold on if metadata says so
                    bool isBold = layoutInfo.DeviceMetadata.TryGetValue("bold", out var bv) && bv is true;
                    if (isBold)
                        buffer.AddRange(EscPosCommands.StyleBold);

                    // Emit text bytes
                    var encoding = GetEncoding(profile);
                    buffer.AddRange(encoding.GetBytes(layoutInfo.WrappedText));

                    // Bold off after bold line
                    if (isBold)
                        buffer.AddRange(EscPosCommands.StyleNormal);

                    // Line feed after each text entry
                    buffer.AddRange(EscPosCommands.LineFeed);
                }
            }

            // Paper cut
            buffer.AddRange(EscPosCommands.CutFull);

            result.Output = buffer.ToArray();
            return result;
        }
        catch (Exception ex)
        {
            var result = new RenderResult("escpos");
            result.AddError($"ESC/POS rendering failed: {ex.Message}");
            result.Output = Array.Empty<byte>();
            return result;
        }
    }

    private static byte[] GetAlignmentCommand(string? alignment) => alignment?.ToLower() switch
    {
        "center" => EscPosCommands.AlignCenter,
        "right" => EscPosCommands.AlignRight,
        _ => EscPosCommands.AlignLeft
    };

    private static Encoding GetEncoding(DeviceProfile profile)
    {
        var codepage = profile.GetCapability("codepage")?.ToString();
        return codepage switch
        {
            "850" => Encoding.GetEncoding(850),
            "437" => Encoding.GetEncoding(437),
            _ => Encoding.ASCII
        };
    }
}
