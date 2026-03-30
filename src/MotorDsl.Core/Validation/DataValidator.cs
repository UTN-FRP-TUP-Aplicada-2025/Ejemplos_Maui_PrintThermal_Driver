using System.Collections;
using System.Reflection;
using System.Text.RegularExpressions;
using MotorDsl.Core.Contracts;
using MotorDsl.Core.Models;

namespace MotorDsl.Core.Validation;

/// <summary>
/// Validates data structure against a DSL template AST.
/// Recursively walks the AST extracting bindings and checking
/// that referenced fields exist with correct types.
/// 
/// Sprint 06 | TK-36, TK-37
/// Supports: CU-19
/// </summary>
public class DataValidator : IDataValidator
{
    private static readonly Regex BindingRegex = new(@"\{\{([^}]+)\}\}", RegexOptions.Compiled);
    private readonly IDataResolver _resolver;

    public DataValidator(IDataResolver? resolver = null)
    {
        _resolver = resolver ?? new DataResolver();
    }

    public ValidationResult Validate(DocumentNode ast, object? data)
    {
        var result = new ValidationResult();
        ValidateNode(ast, data, result, new HashSet<string>());
        return result;
    }

    private void ValidateNode(DocumentNode node, object? data, ValidationResult result, HashSet<string> loopAliases)
    {
        switch (node)
        {
            case TextNode textNode:
                ValidateTextNode(textNode, data, result, loopAliases);
                break;

            case LoopNode loopNode:
                ValidateLoopNode(loopNode, data, result, loopAliases);
                break;

            case ConditionalNode conditionalNode:
                ValidateConditionalNode(conditionalNode, data, result, loopAliases);
                break;
        }

        // Recurse into children
        if (node.Children != null)
        {
            foreach (var child in node.Children)
            {
                ValidateNode(child, data, result, loopAliases);
            }
        }
    }

    private bool IsLoopAlias(string field, HashSet<string> loopAliases)
    {
        foreach (var alias in loopAliases)
        {
            if (field == alias || field.StartsWith(alias + "."))
                return true;
        }
        return false;
    }

    private void ValidateTextNode(TextNode textNode, object? data, ValidationResult result, HashSet<string> loopAliases)
    {
        var matches = BindingRegex.Matches(textNode.Text);
        foreach (Match match in matches)
        {
            var field = match.Groups[1].Value;

            // Skip fields that reference a loop alias (validated at loop level)
            if (IsLoopAlias(field, loopAliases))
                continue;

            var resolved = _resolver.Resolve(data, field);
            if (resolved == null)
            {
                if (FieldExistsButNull(data, field))
                {
                    result.Errors.Add(new ValidationError(
                        field, ValidationErrorType.MissingField,
                        $"Field '{field}' exists but is null", "text")
                    {
                        Severity = ValidationSeverity.Warning
                    });
                }
                else
                {
                    result.AddError(field, ValidationErrorType.MissingField,
                        $"Field '{field}' referenced in text binding not found in data", "text");
                }
            }
        }
    }

    private bool FieldExistsButNull(object? data, string field)
    {
        if (data == null) return false;

        var parts = field.Split('.');
        object? current = data;

        // Navigate to the parent of the last segment
        for (int i = 0; i < parts.Length - 1; i++)
        {
            current = _resolver.Resolve(current, parts[i]);
            if (current == null) return false;
        }

        var lastPart = parts[^1];

        if (current is IDictionary<string, object> dict1)
            return dict1.ContainsKey(lastPart);
        if (current is IDictionary<string, object?> dict2)
            return dict2.ContainsKey(lastPart);

        var prop = current.GetType().GetProperty(lastPart,
            BindingFlags.Public | BindingFlags.Instance);
        return prop != null;
    }

    private void ValidateLoopNode(LoopNode loopNode, object? data, ValidationResult result, HashSet<string> loopAliases)
    {
        // Structure: loop must have a body
        if (loopNode.Body == null)
        {
            result.AddError(loopNode.Source, ValidationErrorType.InvalidStructure,
                "Loop node has no body", "loop");
            return;
        }

        // Source must exist
        var resolved = _resolver.Resolve(data, loopNode.Source);
        if (resolved == null)
        {
            result.AddError(loopNode.Source, ValidationErrorType.MissingField,
                $"Loop source '{loopNode.Source}' not found in data", "loop");
            return;
        }

        // Source must be a collection
        if (resolved is not IEnumerable || resolved is string)
        {
            result.AddError(loopNode.Source, ValidationErrorType.TypeMismatch,
                $"Loop source '{loopNode.Source}' is not a collection (found {resolved.GetType().Name})", "loop");
            return;
        }

        // Recurse into body with the loop alias registered
        var innerAliases = new HashSet<string>(loopAliases) { loopNode.ItemAlias };
        ValidateNode(loopNode.Body, data, result, innerAliases);
    }

    private void ValidateConditionalNode(ConditionalNode conditionalNode, object? data, ValidationResult result, HashSet<string> loopAliases)
    {
        // Structure: must have at least one branch
        if (conditionalNode.TrueBranch == null && conditionalNode.FalseBranch == null)
        {
            result.AddError(conditionalNode.Expression, ValidationErrorType.InvalidStructure,
                "Conditional node has no true or false branch", "conditional");
            return;
        }

        // Recurse into branches
        if (conditionalNode.TrueBranch != null)
            ValidateNode(conditionalNode.TrueBranch, data, result, loopAliases);
        if (conditionalNode.FalseBranch != null)
            ValidateNode(conditionalNode.FalseBranch, data, result, loopAliases);
    }
}
