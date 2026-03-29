using MotorDsl.Core.Models;

namespace MotorDsl.Core.Contracts;

/// <summary>
/// Contract for providing device profiles to the engine.
/// Read-only — the source of profiles is the client's responsibility.
/// Sprint 05 — TK-29 / CU-21 v2.0
/// </summary>
public interface IDeviceProfileProvider
{
    DeviceProfile? GetProfile(string name);
    IEnumerable<DeviceProfile> GetAll();
}
