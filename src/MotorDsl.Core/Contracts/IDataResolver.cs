namespace MotorDsl.Core.Contracts;

/// <summary>
/// Interface for resolving data references and bindings within documents.
/// Handles path-based data resolution and interpolation.
/// 
/// Source: contratos-del-motor_v1.0.md (Section 9)
/// </summary>
public interface IDataResolver
{
    /// <summary>
    /// Resolves a data value using a path expression.
    /// </summary>
    /// <param name="data">Root data object to query</param>
    /// <param name="path">Path expression (e.g., "customer.name", "items[0]")</param>
    /// <returns>Resolved value, or null if path not found</returns>
    object? Resolve(object? data, string path);

    /// <summary>
    /// Resolves a collection path for loop iteration.
    /// </summary>
    /// <param name="data">Root data object to query</param>
    /// <param name="path">Path expression (e.g., "ordenes", "items")</param>
    /// <returns>Enumerable of items, or empty if collection not found</returns>
    IEnumerable<object> ResolveCollection(object? data, string path);
}
