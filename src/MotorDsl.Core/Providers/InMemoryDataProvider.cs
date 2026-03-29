using MotorDsl.Core.Contracts;

namespace MotorDsl.Core.Providers;

/// <summary>
/// In-memory implementation of IDataProvider.
/// Stores data dictionaries by key. Supports Add() for registration.
/// Sprint 05 — TK-28 / CU-25 v2.0
/// </summary>
public class InMemoryDataProvider : IDataProvider
{
    private readonly Dictionary<string, IDictionary<string, object>> _data = new();

    public IDictionary<string, object>? GetData(string dataKey)
        => _data.TryGetValue(dataKey, out var d) ? d : null;

    public void Add(string dataKey, IDictionary<string, object> data)
        => _data[dataKey] = data;
}
