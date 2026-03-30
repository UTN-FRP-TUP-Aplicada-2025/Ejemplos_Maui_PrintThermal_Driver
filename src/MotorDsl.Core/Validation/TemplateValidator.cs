using System.Text.Json;
using MotorDsl.Core.Contracts;
using MotorDsl.Core.Models;

namespace MotorDsl.Core.Validation;

/// <summary>
/// Validates DSL template JSON for syntax, schema, node types and required properties.
/// Sprint 07 | TK-50
/// Supports: CU-14
/// </summary>
public class TemplateValidator : ITemplateValidator
{
    private static readonly HashSet<string> ValidNodeTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "text", "container", "conditional", "loop", "table", "image"
    };

    public ValidationResult ValidateTemplate(string dslJson)
    {
        var result = new ValidationResult();

        // 1. JSON syntax
        JsonDocument doc;
        try
        {
            doc = JsonDocument.Parse(dslJson);
        }
        catch (JsonException ex)
        {
            result.Errors.Add(new ValidationError(
                "dslJson", ValidationErrorType.InvalidSyntax, $"Invalid JSON: {ex.Message}", "Template")
            {
                Location = "root"
            });
            return result;
        }

        var root = doc.RootElement;

        // 2. Required root fields: id, version, root
        ValidateRequiredRootField(root, "id", result);
        ValidateRequiredRootField(root, "version", result);

        if (!root.TryGetProperty("root", out var rootNode))
        {
            result.Errors.Add(new ValidationError(
                "root", ValidationErrorType.MissingRequiredField, "Template must have a 'root' node", "Template")
            {
                Location = "root"
            });
            return result;
        }

        // 3. Validate nodes recursively
        ValidateNode(rootNode, "root", result);

        return result;
    }

    private static void ValidateRequiredRootField(JsonElement root, string fieldName, ValidationResult result)
    {
        if (!root.TryGetProperty(fieldName, out _))
        {
            result.Errors.Add(new ValidationError(
                fieldName, ValidationErrorType.MissingRequiredField,
                $"Template must have '{fieldName}'", "Template")
            {
                Location = "root"
            });
        }
    }

    private static void ValidateNode(JsonElement node, string path, ValidationResult result)
    {
        if (node.ValueKind != JsonValueKind.Object)
            return;

        // Check "type" field exists
        if (!node.TryGetProperty("type", out var typeProp))
        {
            result.Errors.Add(new ValidationError(
                "type", ValidationErrorType.MissingRequiredField,
                "Node must have a 'type' property", "Unknown")
            {
                Location = path
            });
            return;
        }

        var nodeType = typeProp.GetString() ?? "";

        // Validate node type is known
        if (!ValidNodeTypes.Contains(nodeType))
        {
            result.Errors.Add(new ValidationError(
                "type", ValidationErrorType.UnknownNodeType,
                $"Unknown node type: '{nodeType}'", nodeType)
            {
                Location = path
            });
            return;
        }

        // Validate required properties per node type
        switch (nodeType.ToLower())
        {
            case "loop":
                ValidateRequiredNodeField(node, "source", nodeType, path, result);
                ValidateRequiredNodeField(node, "itemAlias", nodeType, path, result);
                ValidateRequiredNodeField(node, "body", nodeType, path, result);
                // Recurse into body
                if (node.TryGetProperty("body", out var loopBody))
                    ValidateNode(loopBody, $"{path}.body", result);
                break;

            case "conditional":
                ValidateRequiredNodeField(node, "expression", nodeType, path, result);
                // Recurse into branches
                if (node.TryGetProperty("trueBranch", out var trueBranch))
                    ValidateNode(trueBranch, $"{path}.trueBranch", result);
                if (node.TryGetProperty("falseBranch", out var falseBranch))
                    ValidateNode(falseBranch, $"{path}.falseBranch", result);
                break;

            case "container":
                // Recurse into children
                if (node.TryGetProperty("children", out var children) && children.ValueKind == JsonValueKind.Array)
                {
                    int idx = 0;
                    foreach (var child in children.EnumerateArray())
                    {
                        ValidateNode(child, $"{path}.children[{idx}]", result);
                        idx++;
                    }
                }
                break;

            case "image":
                ValidateRequiredNodeField(node, "source", nodeType, path, result);
                break;

            case "text":
            case "table":
                // No mandatory fields beyond "type"
                break;
        }
    }

    private static void ValidateRequiredNodeField(JsonElement node, string fieldName, string nodeType, string path, ValidationResult result)
    {
        if (!node.TryGetProperty(fieldName, out _))
        {
            result.Errors.Add(new ValidationError(
                fieldName, ValidationErrorType.MissingRequiredField,
                $"'{nodeType}' node must have '{fieldName}'", nodeType)
            {
                Location = path
            });
        }
    }
}
