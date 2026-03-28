using MotorDsl.Core.Models;

namespace MotorDsl.Core.Contracts;

/// <summary>
/// Contract for evaluating DSL AST nodes against data context.
/// Resolves variables, evaluates conditions, and processes loops.
/// 
/// Sprint 2 | TK-07
/// Supports: CU-02, CU-15, CU-16, CU-17
/// </summary>
public interface IEvaluator
{
    /// <summary>
    /// Evaluates an AST node against the provided data context.
    /// Returns an EvaluatedDocument with resolved nodes and metadata.
    /// </summary>
    EvaluatedDocument Evaluate(DocumentNode ast, object? data);

    /// <summary>
    /// Resolves a data path (e.g., "cliente.nombre", "items[0]")
    /// against the provided data context.
    /// Returns null if path does not exist.
    /// </summary>
    object? ResolveVariable(object? data, string path);

    /// <summary>
    /// Evaluates a condition expression (e.g., "status == 'activo' && cantidad > 50")
    /// against the provided data context.
    /// Throws ArgumentException if expression is invalid.
    /// </summary>
    bool EvaluateCondition(string expression, object? data);

    /// <summary>
    /// Resolves a collection path (e.g., "ordenes", "items")
    /// and returns an enumerable of items for loop iteration.
    /// Returns empty enumerable if collection does not exist.
    /// </summary>
    IEnumerable<object> ResolveCollection(object? data, string path);
}
