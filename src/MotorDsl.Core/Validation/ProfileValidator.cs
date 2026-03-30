using MotorDsl.Core.Contracts;
using MotorDsl.Core.Models;

namespace MotorDsl.Core.Validation;

/// <summary>
/// Validates DeviceProfile required properties: Name, Width, RenderTarget.
/// Sprint 07 | TK-53
/// Supports: CU-22
/// </summary>
public class ProfileValidator : IProfileValidator
{
    public ValidationResult ValidateProfile(DeviceProfile profile)
    {
        var result = new ValidationResult();

        if (string.IsNullOrWhiteSpace(profile.Name))
        {
            result.Errors.Add(new ValidationError(
                "Name", ValidationErrorType.MissingField,
                "Profile name cannot be empty", "DeviceProfile")
            {
                Location = "DeviceProfile.Name"
            });
        }

        if (profile.Width <= 0)
        {
            result.Errors.Add(new ValidationError(
                "Width", ValidationErrorType.InvalidStructure,
                "Profile width must be greater than zero", "DeviceProfile")
            {
                Location = "DeviceProfile.Width"
            });
        }

        if (string.IsNullOrWhiteSpace(profile.RenderTarget))
        {
            result.Errors.Add(new ValidationError(
                "RenderTarget", ValidationErrorType.MissingField,
                "Profile render target cannot be empty", "DeviceProfile")
            {
                Location = "DeviceProfile.RenderTarget"
            });
        }

        return result;
    }
}
