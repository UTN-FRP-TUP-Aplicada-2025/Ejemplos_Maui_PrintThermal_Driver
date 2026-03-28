using MotorDsl.Core.Contracts;
using MotorDsl.Core.Models;
using System.Text;

namespace MotorDsl.Rendering;

/// <summary>
/// Renderer implementation for plain text output.
/// Converts LayoutedDocument to human-readable text format.
/// Target: "text"
/// </summary>
public class TextRenderer : IRenderer
{
    public string Target => "text";

    public RenderResult Render(LayoutedDocument document, DeviceProfile profile)
    {
        try
        {
            var result = new RenderResult("text");

            if (document == null || document.Root == null)
            {
                result.Output = "";
                return result;
            }

            var output = new StringBuilder();

            // Render all nodes with text, ordered by line number
            var orderedEntries = document.NodeLayoutInfo
                .Where(kvp => !string.IsNullOrEmpty(kvp.Value.WrappedText))
                .OrderBy(kvp => kvp.Value.LineNumber)
                .ThenBy(kvp => kvp.Value.ColumnNumber);

            foreach (var kvp in orderedEntries)
            {
                var layoutInfo = kvp.Value;
                var line = ApplyAlignment(layoutInfo.WrappedText, layoutInfo.Alignment, profile.Width);
                output.AppendLine(line);
            }

            result.Output = output.ToString();
            return result;
        }
        catch (Exception ex)
        {
            var result = new RenderResult("text");
            result.AddError($"Text rendering failed: {ex.Message}");
            result.Output = "";
            return result;
        }
    }

    private string ApplyAlignment(string text, string alignment, int width)
    {
        var lines = text.Split('\n');
        var alignedLines = new List<string>();

        foreach (var line in lines)
        {
            string aligned = alignment?.ToLower() switch
            {
                "center" => CenterText(line, width),
                "right" => RightAlignText(line, width),
                _ => LeftAlignText(line, width)
            };
            alignedLines.Add(aligned);
        }

        return string.Join("\n", alignedLines);
    }

    private string LeftAlignText(string text, int width)
    {
        if (text.Length >= width) return text.Substring(0, width);
        return text.PadRight(width);
    }

    private string RightAlignText(string text, int width)
    {
        if (text.Length >= width) return text.Substring(text.Length - width);
        return text.PadLeft(width);
    }

    private string CenterText(string text, int width)
    {
        if (text.Length >= width) return text.Substring(0, width);
        int padding = width - text.Length;
        int leftPad = padding / 2;
        return new string(' ', leftPad) + text + new string(' ', padding - leftPad);
    }
}
