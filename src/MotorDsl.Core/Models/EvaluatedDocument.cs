using MotorDsl.Core.Models;

namespace MotorDsl.Core.Models;

/// <summary>
/// Result of evaluating a DocumentNode AST against data.
/// Contains the resolved AST, original template metadata, warnings, and errors.
/// 
/// Sprint 2 | TK-08
/// Supports: CU-02
/// </summary>
public class EvaluatedDocument
{
    /// <summary>
    /// Document identifier from original template.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// Template version from original template.
    /// </summary>
    public required string Version { get; set; }

    /// <summary>
    /// Root node of evaluated AST.
    /// May be null if evaluation resulted in empty/null (e.g., failed conditional with no false branch).
    /// </summary>
    public DocumentNode? Root { get; set; }

    /// <summary>
    /// Warnings generated during evaluation (e.g., unresolved variables, type mismatches).
    /// </summary>
    public List<string> Warnings { get; set; } = new();

    /// <summary>
    /// Errors that prevented full evaluation.
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// Timestamp when evaluation completed.
    /// </summary>
    public DateTime EvaluatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets whether evaluation succeeded without errors.
    /// </summary>
    public bool IsSuccessful => Errors.Count == 0;
}
