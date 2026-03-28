using System.Reflection;

namespace MotorDsl.Core.Models;

/// <summary>
/// Helper class for resolving variable paths and collections from data contexts.
/// Supports: simple fields, nested paths, array indexing (e.g., "items[0]", "cliente.nombre").
/// 
/// Sprint 2 | TK-09 (DataContext)
/// Supports: CU-02, CU-15, CU-16
/// </summary>
public class DataResolver
{
    /// <summary>
    /// Resolves a data path (e.g., "cliente.nombre", "items[0]") from the provided context.
    /// Returns null if the path does not exist or any intermediate value is null.
    /// </summary>
    public object? Resolve(object? data, string path)
    {
        if (data == null || string.IsNullOrWhiteSpace(path))
            return null;

        var parts = path.Split('.');
        object? current = data;

        foreach (var part in parts)
        {
            if (current == null)
                return null;

            // Handle array indexing: "items[0]"
            if (part.Contains('['))
            {
                current = ResolveArrayIndex(current, part);
                if (current == null)
                    return null;
            }
            else
            {
                // Resolve object property
                current = ResolveProperty(current, part);
                if (current == null)
                    return null;
            }
        }

        return current;
    }

    /// <summary>
    /// Resolves a collection (e.g., "ordenes", "items") for loop iteration.
    /// Returns empty enumerable if path does not exist or is not enumerable.
    /// </summary>
    public IEnumerable<object> ResolveCollection(object? data, string path)
    {
        if (data == null || string.IsNullOrWhiteSpace(path))
            return Enumerable.Empty<object>();

        var value = Resolve(data, path);
        if (value == null)
            return Enumerable.Empty<object>();

        // If it's already an IEnumerable<object>, return it
        if (value is IEnumerable<object> enumerable)
            return enumerable;

        // If it's any IEnumerable, cast to IEnumerable<object>
        if (value is System.Collections.IEnumerable nonGenericEnumerable)
        {
            return nonGenericEnumerable.Cast<object>();
        }

        // Single item, wrap in enumerable
        return new[] { value };
    }

    private object? ResolveProperty(object obj, string propertyName)
    {
        if (obj == null)
            return null;

        var type = obj.GetType();
        // Use IgnoreCase and Instance flags to find properties on anonymous types
        var property = type.GetProperty(propertyName, 
            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

        if (property == null)
            return null;

        try
        {
            return property.GetValue(obj);
        }
        catch
        {
            return null;
        }
    }

    private object? ResolveArrayIndex(object obj, string indexPart)
    {
        // Parse "items[0]" → property: "items", index: 0
        int bracketIndex = indexPart.IndexOf('[');
        if (bracketIndex < 0)
            return null;

        string propertyName = indexPart.Substring(0, bracketIndex);
        string indexStr = indexPart.Substring(bracketIndex + 1);
        indexStr = indexStr.TrimEnd(']');

        if (!int.TryParse(indexStr, out int index))
            return null;

        var property = ResolveProperty(obj, propertyName);
        if (property == null)
            return null;

        // Try to index into the collection
        try
        {
            if (property is System.Collections.IList list)
                return list[index];

            if (property is System.Collections.IEnumerable enumerable)
                return enumerable.Cast<object>().ElementAtOrDefault(index);
        }
        catch
        {
            return null;
        }

        return null;
    }
}
