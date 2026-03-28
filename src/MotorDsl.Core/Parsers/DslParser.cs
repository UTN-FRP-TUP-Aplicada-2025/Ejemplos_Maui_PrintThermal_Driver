using System.Text.Json;
using MotorDsl.Core.Contracts;
using MotorDsl.Core.Models;

namespace MotorDsl.Core.Parsers;

/// <summary>
/// Implementation of IDslParser using System.Text.Json.
/// Parses DSL templates in JSON format into DocumentTemplate objects.
/// 
/// Source: contratos-del-motor_v1.0.md (Section 7)
/// Supports element types: text, container, conditional, loop, table, image
/// </summary>
public class DslParser : IDslParser
{
    private static readonly HashSet<string> SupportedTypes = new()
    {
        "text",
        "container",
        "conditional",
        "loop",
        "table",
        "image"
    };

    public DocumentTemplate Parse(string dsl)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(dsl))
            throw new ArgumentException("DSL cannot be null or empty");

        try
        {
            // Parse JSON
            var doc = JsonDocument.Parse(dsl);
            var root = doc.RootElement;

            // Extract id (required)
            if (!root.TryGetProperty("id", out var idToken))
                throw new ArgumentException("DSL must contain 'id' property");

            var id = idToken.GetString();
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException("'id' cannot be null or empty");

            // Extract version (required)
            if (!root.TryGetProperty("version", out var versionToken))
                throw new ArgumentException("DSL must contain 'version' property");

            var version = versionToken.GetString();
            if (string.IsNullOrEmpty(version))
                throw new ArgumentException("'version' cannot be null or empty");

            // Extract root node (required)
            if (!root.TryGetProperty("root", out var rootNodeToken))
                throw new ArgumentException("DSL must contain 'root' property");

            var parsedRoot = ParseNode(rootNodeToken);

            // Extract metadata (optional)
            var metadata = new Dictionary<string, object>();
            if (root.TryGetProperty("metadata", out var metadataToken) && metadataToken.ValueKind == JsonValueKind.Object)
            {
                foreach (var prop in metadataToken.EnumerateObject())
                {
                    object value = prop.Value.ValueKind switch
                    {
                        JsonValueKind.String => (object)(prop.Value.GetString() ?? ""),
                        JsonValueKind.Number => (object)prop.Value.GetDouble(),
                        JsonValueKind.True => (object)true,
                        JsonValueKind.False => (object)false,
                        _ => (object)prop.Value.ToString()
                    };
                    metadata[prop.Name] = value;
                }
            }

            return new DocumentTemplate(id, version, parsedRoot)
            {
                Metadata = metadata
            };
        }
        catch (JsonException ex)
        {
            throw new ArgumentException("Invalid JSON format: " + ex.Message, ex);
        }
    }

    private DocumentNode ParseNode(JsonElement nodeToken)
    {
        if (nodeToken.ValueKind != JsonValueKind.Object)
            throw new ArgumentException("Node must be a JSON object");

        if (!nodeToken.TryGetProperty("type", out var typeToken))
            throw new ArgumentException("Node must have a 'type' property");

        var type = typeToken.GetString();
        if (string.IsNullOrEmpty(type))
            throw new ArgumentException("Node type cannot be empty");

        if (!SupportedTypes.Contains(type))
            throw new ArgumentException($"Unsupported element type: '{type}'. Supported types are: {string.Join(", ", SupportedTypes)}");

        return type switch
        {
            "text" => ParseTextNode(nodeToken),
            "container" => ParseContainerNode(nodeToken),
            "conditional" => ParseConditionalNode(nodeToken),
            "loop" => ParseLoopNode(nodeToken),
            "table" => ParseTableNode(nodeToken),
            "image" => ParseImageNode(nodeToken),
            _ => throw new ArgumentException($"Unsupported element type: '{type}'")
        };
    }

    private TextNode ParseTextNode(JsonElement nodeToken)
    {
        var text = nodeToken.TryGetProperty("text", out var textToken)
            ? textToken.GetString() ?? ""
            : "";

        var bindPath = nodeToken.TryGetProperty("bindPath", out var bindToken)
            ? bindToken.GetString()
            : null;

        return new TextNode(text, bindPath);
    }

    private ContainerNode ParseContainerNode(JsonElement nodeToken)
    {
        var layout = nodeToken.TryGetProperty("layout", out var layoutToken)
            ? layoutToken.GetString()
            : null;

        var container = new ContainerNode(layout);

        if (nodeToken.TryGetProperty("children", out var childrenToken) && childrenToken.ValueKind == JsonValueKind.Array)
        {
            foreach (var childToken in childrenToken.EnumerateArray())
            {
                var childNode = ParseNode(childToken);
                container.AddChild(childNode);
            }
        }

        return container;
    }

    private ConditionalNode ParseConditionalNode(JsonElement nodeToken)
    {
        var expression = nodeToken.TryGetProperty("expression", out var expToken)
            ? expToken.GetString() ?? ""
            : "";

        var trueBranch = nodeToken.TryGetProperty("trueBranch", out var trueToken)
            ? ParseNode(trueToken)
            : null;

        var falseBranch = nodeToken.TryGetProperty("falseBranch", out var falseToken)
            ? ParseNode(falseToken)
            : null;

        return new ConditionalNode(expression, trueBranch, falseBranch);
    }

    private LoopNode ParseLoopNode(JsonElement nodeToken)
    {
        var source = nodeToken.TryGetProperty("source", out var srcToken)
            ? srcToken.GetString() ?? ""
            : "";

        var itemAlias = nodeToken.TryGetProperty("itemAlias", out var aliasToken)
            ? aliasToken.GetString() ?? ""
            : "";

        var body = nodeToken.TryGetProperty("body", out var bodyToken)
            ? ParseNode(bodyToken)
            : null;

        return new LoopNode(source, itemAlias, body);
    }

    private TableNode ParseTableNode(JsonElement nodeToken)
    {
        var headers = new List<string>();
        if (nodeToken.TryGetProperty("headers", out var headersToken) && headersToken.ValueKind == JsonValueKind.Array)
        {
            foreach (var headerToken in headersToken.EnumerateArray())
            {
                headers.Add(headerToken.GetString() ?? "");
            }
        }

        var rows = new List<List<string>>();
        if (nodeToken.TryGetProperty("rows", out var rowsToken) && rowsToken.ValueKind == JsonValueKind.Array)
        {
            foreach (var rowToken in rowsToken.EnumerateArray())
            {
                var row = new List<string>();
                if (rowToken.ValueKind == JsonValueKind.Array)
                {
                    foreach (var cellToken in rowToken.EnumerateArray())
                    {
                        row.Add(cellToken.GetString() ?? "");
                    }
                }
                rows.Add(row);
            }
        }

        return new TableNode(headers, rows);
    }

    private ImageNode ParseImageNode(JsonElement nodeToken)
    {
        var source = nodeToken.TryGetProperty("source", out var srcToken)
            ? srcToken.GetString() ?? ""
            : "";

        var width = nodeToken.TryGetProperty("width", out var widthToken) && widthToken.ValueKind == JsonValueKind.Number
            ? widthToken.GetInt32() as int?
            : null;

        var height = nodeToken.TryGetProperty("height", out var heightToken) && heightToken.ValueKind == JsonValueKind.Number
            ? heightToken.GetInt32() as int?
            : null;

        var imageType = nodeToken.TryGetProperty("imageType", out var typeToken)
            ? typeToken.GetString()
            : null;

        return new ImageNode(source, width, height, imageType);
    }
}
