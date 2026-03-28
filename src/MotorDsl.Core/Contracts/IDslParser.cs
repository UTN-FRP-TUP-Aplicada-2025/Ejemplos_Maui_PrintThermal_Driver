using MotorDsl.Core.Models;

namespace MotorDsl.Core.Contracts;

/// <summary>
/// Interface for parsing DSL templates into an Abstract Syntax Tree (AST).
/// Transforms raw DSL strings into structured DocumentTemplate objects.
/// 
/// Source: contratos-del-motor_v1.0.md (Section 7)
/// </summary>
public interface IDslParser
{
    /// <summary>
    /// Parses a DSL template string into a structured DocumentTemplate.
    /// </summary>
    /// <param name="dsl">Raw DSL template as string (JSON format expected)</param>
    /// <returns>Parsed DocumentTemplate with AST root node</returns>
    /// <exception cref="ArgumentException">If DSL is invalid or malformed</exception>
    DocumentTemplate Parse(string dsl);
}
