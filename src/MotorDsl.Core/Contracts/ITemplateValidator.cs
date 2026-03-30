using MotorDsl.Core.Models;

namespace MotorDsl.Core.Contracts;

/// <summary>
/// Contract for validating DSL template structure before processing.
/// Detects syntax errors, missing required fields, unknown node types,
/// and missing obligatory properties per node type.
/// 
/// Sprint 07 | TK-49
/// Supports: CU-14
/// </summary>
public interface ITemplateValidator
{
    /// <summary>
    /// Validates a raw DSL JSON string for correctness.
    /// </summary>
    /// <param name="dslJson">Raw DSL template as JSON string</param>
    /// <returns>ValidationResult with any errors found</returns>
    ValidationResult ValidateTemplate(string dslJson);
}
