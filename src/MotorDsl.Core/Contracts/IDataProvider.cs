namespace MotorDsl.Core.Contracts;

/// <summary>
/// Contract for providing document data to the engine.
/// Read-only — the source of data is the client's responsibility.
/// Sprint 05 — TK-28 / CU-25 v2.0
/// </summary>
public interface IDataProvider
{
    IDictionary<string, object>? GetData(string dataKey);
}
