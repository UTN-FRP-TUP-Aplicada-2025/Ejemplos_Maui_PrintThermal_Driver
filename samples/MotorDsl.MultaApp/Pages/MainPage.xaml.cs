using MotorDsl.Core.Contracts;
using MotorDsl.Core.Models;
using MotorDsl.MultaApp.Templates;
using System.Net.Http.Json;

namespace MotorDsl.MultaApp.Pages;

public partial class MainPage : ContentPage
{
    private readonly IDocumentEngine _engine;
    private readonly Button[] _tabs;
    private readonly ScrollView[] _panels;

    public MainPage(IDocumentEngine engine)
    {
        InitializeComponent();
        _engine = engine;
        _tabs = new[] { TabPreview, TabEscPos, TabPdf, TabExport };
        _panels = new[] { PanelPreview, PanelEscPos, PanelPdf, PanelExport };
    }

    // ─── Tab switching ───

    private void OnTabClicked(object? sender, EventArgs e)
    {
        if (sender is not Button clicked) return;
        int idx = Array.IndexOf(_tabs, clicked);
        for (int i = 0; i < _tabs.Length; i++)
        {
            _tabs[i].BackgroundColor = i == idx ? Color.FromArgb("#1565C0") : Color.FromArgb("#1976D2");
            _panels[i].IsVisible = i == idx;
        }
    }

    // ─── Tab 1: Preview MAUI ───

    private void OnGeneratePreviewClicked(object? sender, EventArgs e)
    {
        try
        {
            ShowStatus("Generando preview...");
            var profile = new DeviceProfile("thermal_58mm", 32, "text");
            var layouted = _engine.RenderLayout(MultaDsl.Template, MultaDsl.GetSampleData(), profile);

            MauiPreview.Document = layouted;
            ShowStatus("Preview generado.");
        }
        catch (Exception ex)
        {
            ShowStatus($"Error: {ex.Message}");
        }
    }

    // ─── Tab 2: ESC/POS Hex dump ───

    private void OnGenerateEscPosClicked(object? sender, EventArgs e)
    {
        try
        {
            ShowStatus("Generando ESC/POS...");
            var profile = new DeviceProfile("thermal_58mm", 32, "escpos-bitmap");
            var result = _engine.Render(MultaDsl.Template, MultaDsl.GetSampleData(), profile);

            if (result.IsSuccessful && result.Output is byte[] bytes)
            {
                var hex = BitConverter.ToString(bytes).Replace("-", " ");
                var lines = new List<string>();
                for (int i = 0; i < hex.Length; i += 48)
                    lines.Add(hex[i..Math.Min(i + 48, hex.Length)]);

                EscPosOutput.Text = $"ESC/POS ({bytes.Length} bytes):\n\n{string.Join("\n", lines)}";
                ShowStatus($"ESC/POS generado: {bytes.Length} bytes");
            }
            else
            {
                EscPosOutput.Text = "ERRORES:\n" + string.Join("\n", result.Errors);
                ShowStatus("Error al generar ESC/POS");
            }
        }
        catch (Exception ex)
        {
            EscPosOutput.Text = $"Excepción: {ex.Message}";
            ShowStatus("Error");
        }
    }

    // ─── Tab 3: PDF Preview ───

    private void OnGeneratePdfClicked(object? sender, EventArgs e)
    {
        try
        {
            ShowStatus("Generando PDF...");
            var profile = new DeviceProfile("a4-pdf", 80, "pdf");
            var result = _engine.Render(MultaDsl.Template, MultaDsl.GetSampleData(), profile);

            if (result.IsSuccessful && result.Output is byte[] pdfBytes)
            {
                var base64 = Convert.ToBase64String(pdfBytes);
                PdfWebView.Source = new HtmlWebViewSource
                {
                    Html = $@"<html><body style='margin:0;padding:0;'>
                        <embed src='data:application/pdf;base64,{base64}'
                               width='100%' height='100%' type='application/pdf' />
                        <p style='text-align:center;font-family:sans-serif;'>
                            PDF generado ({pdfBytes.Length:N0} bytes).
                            Si no se ve, el WebView de Android puede no soportar embed PDF.
                        </p>
                    </body></html>"
                };
                PdfStatus.Text = $"PDF generado: {pdfBytes.Length:N0} bytes";
                ShowStatus($"PDF generado: {pdfBytes.Length:N0} bytes");
            }
            else
            {
                PdfStatus.Text = "ERRORES:\n" + string.Join("\n", result.Errors);
                ShowStatus("Error al generar PDF");
            }
        }
        catch (Exception ex)
        {
            PdfStatus.Text = $"Excepción: {ex.Message}";
            ShowStatus("Error");
        }
    }

    // ─── Tab 4: Exportar a API REST ───

    private async void OnExportClicked(object? sender, EventArgs e)
    {
        try
        {
            var url = ApiUrlEntry.Text?.Trim();
            if (string.IsNullOrEmpty(url))
            {
                ExportStatus.Text = "Ingresá una URL válida.";
                return;
            }

            ShowStatus("Exportando...");

            // Render ESC/POS
            var escposProfile = new DeviceProfile("thermal_58mm", 32, "escpos-bitmap");
            var escposResult = _engine.Render(MultaDsl.Template, MultaDsl.GetSampleData(), escposProfile);

            // Render PDF
            var pdfProfile = new DeviceProfile("a4-pdf", 80, "pdf");
            var pdfResult = _engine.Render(MultaDsl.Template, MultaDsl.GetSampleData(), pdfProfile);

            var payload = new
            {
                actaNumero   = "SMT-2026-004571",
                timestamp    = DateTime.UtcNow,
                target       = escposResult.Target,
                contentB64   = escposResult.ToBase64(),
                pdfB64       = pdfResult.ToBase64(),
                agenteLegajo = "T-1247"
            };

            using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(15) };
            var response = await http.PostAsJsonAsync(url, payload);

            ExportStatus.Text = response.IsSuccessStatusCode
                ? $"Exportado OK — {response.StatusCode}"
                : $"Error — {response.StatusCode}: {await response.Content.ReadAsStringAsync()}";
            ShowStatus(ExportStatus.Text);
        }
        catch (Exception ex)
        {
            ExportStatus.Text = $"Excepción: {ex.Message}";
            ShowStatus("Error al exportar");
        }
    }

    // ─── Helpers ───

    private void ShowStatus(string msg)
    {
        StatusLabel.Text = $"{DateTime.Now:HH:mm:ss} — {msg}";
    }
}
