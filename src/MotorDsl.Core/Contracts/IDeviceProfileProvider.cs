using MotorDsl.Core.Models;

namespace MotorDsl.Core.Contracts;

/// <summary>
/// Interface for providing and managing device profiles.
/// Returns device profile definitions based on profile names.
/// 
/// Source: contratos-del-motor_v1.0.md (Section 10)
/// </summary>
public interface IDeviceProfileProvider
{
    /// <summary>
    /// Gets a registered device profile by name.
    /// </summary>
    /// <param name="name">Profile name (e.g., "thermalprinter_80mm", "ui_preview")</param>
    /// <returns>DeviceProfile defining the device's capabilities and constraints</returns>
    /// <exception cref="KeyNotFoundException">If profile name not found</exception>
    DeviceProfile GetProfile(string name);
}
