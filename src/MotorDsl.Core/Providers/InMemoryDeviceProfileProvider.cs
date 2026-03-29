using MotorDsl.Core.Contracts;
using MotorDsl.Core.Models;

namespace MotorDsl.Core.Providers;

/// <summary>
/// In-memory implementation of IDeviceProfileProvider (v2.0).
/// Stores profiles by name. Supports Add() for registration.
/// Sprint 05 — TK-29 / CU-21 v2.0
/// </summary>
public class InMemoryDeviceProfileProvider : IDeviceProfileProvider
{
    private readonly Dictionary<string, DeviceProfile> _profiles = new();

    public DeviceProfile? GetProfile(string name)
        => _profiles.TryGetValue(name, out var p) ? p : null;

    public IEnumerable<DeviceProfile> GetAll()
        => _profiles.Values;

    public void Add(DeviceProfile profile)
        => _profiles[profile.Name] = profile;
}
