using MotorDsl.Core.Contracts;
using MotorDsl.Core.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MotorDsl.MultaApp.Renderers;

/// <summary>
/// Renderer que genera PDF usando QuestPDF.
/// Mapea NodeLayoutInfo a elementos QuestPDF respetando alineación y negritas.
/// Sprint 08 | TK-65
/// </summary>
public class PdfRenderer : IRenderer
{
    public string Target => "pdf";

    public RenderResult Render(LayoutedDocument document, DeviceProfile profile)
    {
        try
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var result = new RenderResult(Target);

            var pdfBytes = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Content().Column(col =>
                    {
                        if (document?.Root == null) return;

                        var entries = document.NodeLayoutInfo
                            .Where(kvp => !string.IsNullOrEmpty(kvp.Value.WrappedText))
                            .OrderBy(kvp => kvp.Value.LineNumber)
                            .ThenBy(kvp => kvp.Value.ColumnNumber);

                        foreach (var kvp in entries)
                        {
                            var info = kvp.Value;

                            // Bitmap image
                            if (info.DeviceMetadata.TryGetValue("is_bitmap", out var bmpFlag) && bmpFlag is true)
                            {
                                var source = info.DeviceMetadata.TryGetValue("bitmap_source", out var src)
                                    ? src?.ToString() ?? ""
                                    : "";
                                if (source.Contains(","))
                                {
                                    var base64 = source[(source.IndexOf(',') + 1)..];
                                    var imgBytes = Convert.FromBase64String(base64);
                                    var item = col.Item();
                                    if (info.Alignment?.ToLower() == "center")
                                        item.AlignCenter().Width(60).Image(imgBytes);
                                    else
                                        item.Width(60).Image(imgBytes);
                                }
                                continue;
                            }

                            // QR code
                            if (info.DeviceMetadata.TryGetValue("is_qr", out var qrFlag) && qrFlag is true)
                            {
                                var qrData = info.DeviceMetadata["qr_data"]?.ToString() ?? "";
                                col.Item().AlignCenter().Text(text =>
                                {
                                    text.Span($"[QR: {qrData}]").Italic();
                                });
                                continue;
                            }

                            // Barcode
                            if (info.DeviceMetadata.TryGetValue("is_barcode", out var bcFlag) && bcFlag is true)
                            {
                                var bcData = info.DeviceMetadata["barcode_data"]?.ToString() ?? "";
                                col.Item().AlignCenter().Text(text =>
                                {
                                    text.Span($"[BARCODE: {bcData}]").Italic();
                                });
                                continue;
                            }

                            // Text node
                            bool isBold = info.DeviceMetadata.TryGetValue("bold", out var bv) && bv is true;

                            var container2 = col.Item();
                            var aligned = info.Alignment?.ToLower() switch
                            {
                                "center" => container2.AlignCenter(),
                                "right" => container2.AlignRight(),
                                _ => container2.AlignLeft()
                            };

                            aligned.Text(text =>
                            {
                                var span = text.Span(info.WrappedText);
                                if (isBold) span.Bold();
                            });
                        }
                    });
                });
            }).GeneratePdf();

            result.Output = pdfBytes;
            return result;
        }
        catch (Exception ex)
        {
            var result = new RenderResult(Target);
            result.AddError($"PDF rendering failed: {ex.Message}");
            result.Output = Array.Empty<byte>();
            return result;
        }
    }
}
