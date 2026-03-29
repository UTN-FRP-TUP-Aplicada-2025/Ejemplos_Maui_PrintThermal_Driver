using MotorDsl.Core.Models;

namespace MotorDsl.Core.Contracts;

/// <summary>
/// Contract for validating data structure against a DSL template AST.
/// Detects missing fields, type mismatches, and structural problems
/// before evaluation to prevent runtime errors.
/// 
/// Sprint 06 | TK-36
/// Supports: CU-19
/// </summary>
public interface IDataValidator
{
    /// <summary>
    /// Validates that the provided data contains all fields required by the AST,
    /// with correct types for loops and conditions.
    /// </summary>
    /// <param name="ast">The document AST to validate against</param>
    /// <param name="data">The data context to validate</param>
    /// <returns>ValidationResult with any errors found</returns>
    ValidationResult Validate(DocumentNode ast, object? data);
}
