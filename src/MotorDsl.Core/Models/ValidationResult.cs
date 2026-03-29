namespace MotorDsl.Core.Models;

/// <summary>
/// Types of validation errors detected by IDataValidator.
/// Sprint 06 | TK-38
/// </summary>
public enum ValidationErrorType
{
    /// <summary>A binding references a field that does not exist in the data.</summary>
    MissingField,

    /// <summary>A field exists but its type is incompatible (e.g., loop source is not a collection).</summary>
    TypeMismatch,

    /// <summary>A structural problem in the AST (e.g., loop without body, conditional without branches).</summary>
    InvalidStructure
}

/// <summary>
/// Represents a single validation error found during data validation.
/// Immutable after construction.
/// Sprint 06 | TK-38
/// </summary>
public class ValidationError
{
    public string Field { get; }
    public ValidationErrorType Type { get; }
    public string Message { get; }
    public string NodeType { get; }

    public ValidationError(string field, ValidationErrorType type, string message, string nodeType)
    {
        Field = field;
        Type = type;
        Message = message;
        NodeType = nodeType;
    }
}

/// <summary>
/// Result of validating data against a DSL template AST.
/// IsValid is true when there are no errors.
/// Sprint 06 | TK-38
/// </summary>
public class ValidationResult
{
    public bool IsValid => Errors.Count == 0;
    public List<ValidationError> Errors { get; } = new();

    public void AddError(string field, ValidationErrorType type, string message, string nodeType)
    {
        Errors.Add(new ValidationError(field, type, message, nodeType));
    }
}
