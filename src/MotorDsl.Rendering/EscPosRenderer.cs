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

                    // QR code — emit GS ( k sequence instead of text
                    if (layoutInfo.DeviceMetadata.TryGetValue("is_qr", out var qrFlag) && qrFlag is true)
                    {
                        var qrData = layoutInfo.DeviceMetadata["qr_data"]?.ToString() ?? "";
                        buffer.AddRange(EmitQrCode(qrData, GetEncoding(profile)));
                        continue;
                    }

                    // Barcode EAN-13 — emit GS k 2 sequence
                    if (layoutInfo.DeviceMetadata.TryGetValue("is_barcode", out var bcFlag) && bcFlag is true)
                    {
                        var barcodeData = layoutInfo.DeviceMetadata["barcode_data"]?.ToString() ?? "";
                        buffer.AddRange(EmitBarcodeEan13(barcodeData, GetEncoding(profile)));
                        continue;
                    }

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

    private static byte[] EmitQrCode(string data, Encoding encoding)
    {
        var buffer = new List<byte>();
        var urlBytes = encoding.GetBytes(data);
        int len = urlBytes.Length + 3;
        byte ll = (byte)(len & 0xFF);
        byte lh = (byte)((len >> 8) & 0xFF);

        // Set QR module size
        buffer.AddRange(EscPosCommands.QrSetSize3);
        // Set QR error correction level
        buffer.AddRange(EscPosCommands.QrSetErrorM);
        // Store QR data: GS ( k pL pH 31 50 30 <data>
        buffer.AddRange(new byte[] { 0x1D, 0x28, 0x6B, ll, lh, 0x31, 0x50, 0x30 });
        buffer.AddRange(urlBytes);
        // Print QR code
        buffer.AddRange(EscPosCommands.QrPrint);

        return buffer.ToArray();
    }

    private static byte[] EmitBarcodeEan13(string data, Encoding encoding)
    {
        var buffer = new List<byte>();

        // GS k 2 — EAN-13 barcode (mode 2, NUL-terminated)
        buffer.AddRange(EscPosCommands.BarcodeEan13);
        buffer.AddRange(encoding.GetBytes(data));
        buffer.Add(0x00); // NUL terminator

        return buffer.ToArray();
    }
}
