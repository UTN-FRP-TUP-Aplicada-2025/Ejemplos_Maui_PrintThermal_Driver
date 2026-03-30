using MotorDsl.Core.Models;

namespace MotorDsl.Core.Contracts;

/// <summary>
/// Contract for validating device profiles before use in layout/render.
/// Ensures profiles have valid configuration (width, render target, name).
/// 
/// Sprint 07 | TK-52
/// Supports: CU-22
/// </summary>
public interface IProfileValidator
{
    /// <summary>
    /// Validates a device profile for correctness.
    /// </summary>
    /// <param name="profile">The device profile to validate</param>
    /// <returns>ValidationResult with any errors found</returns>
    ValidationResult ValidateProfile(DeviceProfile profile);
}
