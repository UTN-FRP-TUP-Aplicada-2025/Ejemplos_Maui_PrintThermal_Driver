using MotorDsl.Core.Contracts;
using MotorDsl.Core.Models;

namespace MotorDsl.Core.Layout;

/// <summary>
/// Default implementation of ILayoutEngine.
/// Calculates layout for document nodes respecting device profile constraints.
/// </summary>
public class LayoutEngine : ILayoutEngine
{
    private int _nodeCounter;

    public LayoutedDocument ApplyLayout(EvaluatedDocument document, DeviceProfile profile)
    {
        _nodeCounter = 0;

        var layoutedDoc = new LayoutedDocument
        {
            Root = document.Root,
            DeviceProfile = profile,
            TotalWidth = profile.Width,
            NodeLayoutInfo = new Dictionary<string, LayoutInfo>(),
            LayoutMetadata = new Dictionary<string, object>()
        };

        if (document.Root != null)
        {
            int currentLine = 0;
            CalculateNodeLayout(document.Root, ref currentLine, layoutedDoc, profile);
            layoutedDoc.TotalHeight = currentLine > 0 ? currentLine : 1;
        }

        return layoutedDoc;
    }

    private void CalculateNodeLayout(DocumentNode node, ref int currentLine, LayoutedDocument layoutedDoc, DeviceProfile profile)
    {
        if (node == null) return;

        string nodeId = $"node_{_nodeCounter++}";

        var nodeAlign = node.Style?.Attributes?.TryGetValue("align", out var alignVal) == true
            ? alignVal?.ToString() ?? "left"
            : "left";

        var layoutInfo = new LayoutInfo
        {
            LineNumber = currentLine,
            ColumnNumber = 0,
            Alignment = nodeAlign,
            IsWrapped = false,
            WrappedText = "",
            IsTruncated = false,
            DeviceMetadata = new Dictionary<string, object>()
        };

        if (node is TextNode textNode)
        {
            ApplyTextNodeLayout(textNode, layoutInfo, profile);
            layoutedDoc.NodeLayoutInfo[nodeId] = layoutInfo;
            currentLine += layoutInfo.Height;
        }
        else if (node is ContainerNode containerNode)
        {
            layoutInfo.Width = profile.Width;
            layoutInfo.Height = 0;
            layoutedDoc.NodeLayoutInfo[nodeId] = layoutInfo;

            if (containerNode.Children != null)
            {
                foreach (var child in containerNode.Children)
                {
                    CalculateNodeLayout(child, ref currentLine, layoutedDoc, profile);
                }
            }
        }
        else if (node is ConditionalNode conditionalNode)
        {
            layoutedDoc.NodeLayoutInfo[nodeId] = layoutInfo;
            if (conditionalNode.TrueBranch != null)
                CalculateNodeLayout(conditionalNode.TrueBranch, ref currentLine, layoutedDoc, profile);
        }
        else if (node is LoopNode loopNode)
        {
            layoutedDoc.NodeLayoutInfo[nodeId] = layoutInfo;
            if (loopNode.Children != null)
            {
                foreach (var child in loopNode.Children)
                {
                    CalculateNodeLayout(child, ref currentLine, layoutedDoc, profile);
                }
            }
        }
        else if (node is TableNode tableNode)
        {
            ApplyTableNodeLayout(tableNode, ref currentLine, layoutedDoc, profile);
        }
    }

    private void ApplyTableNodeLayout(TableNode tableNode, ref int currentLine, LayoutedDocument layoutedDoc, DeviceProfile profile)
    {
        int numCols = Math.Max(
            tableNode.Headers.Count,
            tableNode.Rows.Count > 0 ? tableNode.Rows[0].Count : 0);

        if (numCols == 0) return;

        int width = profile.Width;
        int[] colWidths = CalculateColumnWidths(numCols, width);

        bool isBold = tableNode.Style?.Attributes?.TryGetValue("bold", out var boldVal) == true
            && boldVal?.ToString()?.ToLower() == "true";

        // Header line
        if (tableNode.Headers.Count > 0)
        {
            string headerText = FormatTableRow(tableNode.Headers, colWidths);
            string headerId = $"node_{_nodeCounter++}";
            layoutedDoc.NodeLayoutInfo[headerId] = new LayoutInfo
            {
                LineNumber = currentLine++,
                ColumnNumber = 0,
                Width = width,
                Height = 1,
                Alignment = "left",
                WrappedText = headerText,
                DeviceMetadata = isBold
                    ? new Dictionary<string, object> { ["bold"] = true }
                    : new Dictionary<string, object>()
            };

            // Separator line
            string separator = new string('-', width);
            string sepId = $"node_{_nodeCounter++}";
            layoutedDoc.NodeLayoutInfo[sepId] = new LayoutInfo
            {
                LineNumber = currentLine++,
                ColumnNumber = 0,
                Width = width,
                Height = 1,
                Alignment = "left",
                WrappedText = separator,
                DeviceMetadata = new Dictionary<string, object>()
            };
        }

        // Data rows
        foreach (var row in tableNode.Rows)
        {
            string rowText = FormatTableRow(row, colWidths);
            string rowId = $"node_{_nodeCounter++}";
            layoutedDoc.NodeLayoutInfo[rowId] = new LayoutInfo
            {
                LineNumber = currentLine++,
                ColumnNumber = 0,
                Width = width,
                Height = 1,
                Alignment = "left",
                WrappedText = rowText,
                DeviceMetadata = new Dictionary<string, object>()
            };
        }
    }

    private static int[] CalculateColumnWidths(int numCols, int totalWidth)
    {
        int[] widths = new int[numCols];
        if (numCols == 1)
        {
            widths[0] = totalWidth;
            return widths;
        }

        int separators = numCols - 1;
        int usable = totalWidth - separators;
        int col0Width = usable / 2;
        int remaining = usable - col0Width;
        int otherCols = numCols - 1;
        int perOther = remaining / otherCols;
        int extraChars = remaining - (perOther * otherCols);

        widths[0] = col0Width;
        for (int i = 1; i < numCols; i++)
        {
            widths[i] = perOther + (i <= extraChars ? 1 : 0);
        }

        return widths;
    }

    private static string FormatTableRow(IList<string> cells, int[] colWidths)
    {
        var parts = new List<string>();
        for (int i = 0; i < colWidths.Length; i++)
        {
            string cell = i < cells.Count ? cells[i] : "";
            if (cell.Length > colWidths[i])
                cell = cell.Substring(0, colWidths[i]);
            parts.Add(cell.PadRight(colWidths[i]));
        }
        return string.Join(" ", parts);
    }

    private void ApplyTextNodeLayout(TextNode textNode, LayoutInfo layoutInfo, DeviceProfile profile)
    {
        string text = textNode.Text ?? "";

        if (text.Length <= profile.Width)
        {
            layoutInfo.Width = text.Length;
            layoutInfo.Height = 1;
            layoutInfo.WrappedText = text;
            layoutInfo.IsWrapped = false;
        }
        else
        {
            layoutInfo.IsWrapped = true;
            var wrappedLines = WrapText(text, profile.Width);
            layoutInfo.Height = wrappedLines.Count;
            layoutInfo.Width = profile.Width;
            layoutInfo.WrappedText = string.Join("\n", wrappedLines);
        }
    }

    private List<string> WrapText(string text, int maxWidth)
    {
        var lines = new List<string>();
        var words = text.Split(' ');
        var currentLine = "";

        foreach (var word in words)
        {
            if (string.IsNullOrEmpty(currentLine))
            {
                currentLine = word;
            }
            else if (currentLine.Length + 1 + word.Length <= maxWidth)
            {
                currentLine += " " + word;
            }
            else
            {
                lines.Add(currentLine);
                currentLine = word;
            }
        }

        if (!string.IsNullOrEmpty(currentLine))
            lines.Add(currentLine);

        return lines;
    }
}
