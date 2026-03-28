namespace MotorDsl.Core.Models;

/// <summary>
/// Represents the capabilities and constraints of a rendering device.
/// Defines how documents should be adapted for specific hardware.
/// 
/// Source: modelo-datos-logico_v1.0.md (Section 10)
/// Contracts: contratos-del-motor_v1.0.md (Section 4)
/// </summary>
public class DeviceProfile
{
    /// <summary>
    /// Gets or sets the profile name/identifier.
    /// Example: "thermal_80mm", "ui_preview", "pdf_a4"
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the available width in character units or pixels.
    /// For thermal printers: typically 32 (80mm at 10cpi) or 48 (80mm at 12cpi)
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// Gets or sets the render target type.
    /// Possible values: "escpos", "ui", "text", "pdf", "image"
    /// </summary>
    public string RenderTarget { get; set; }

    /// <summary>
    /// Gets or sets device capabilities as key-value pairs.
    /// Examples:
    ///   - "supports_qrcode" : true
    ///   - "supports_tables" : true
    ///   - "supports_images" : false
    ///   - "max_line_width" : 80
    /// </summary>
    public Dictionary<string, object>? Capabilities { get; set; }

    /// <summary>
    /// Constructor for device profiles.
    /// </summary>
    /// <param name="name">Profile name</param>
    /// <param name="width">Available width</param>
    /// <param name="renderTarget">Target render type</param>
    public DeviceProfile(string name, int width, string renderTarget)
    {
        Name = name;
        Width = width;
        RenderTarget = renderTarget;
        Capabilities = new Dictionary<string, object>();
    }

    /// <summary>
    /// Checks if the device supports a specific capability.
    /// </summary>
    public bool HasCapability(string capabilityName)
    {
        return Capabilities?.ContainsKey(capabilityName) ?? false;
    }

    /// <summary>
    /// Adds or updates a capability.
    /// </summary>
    public void SetCapability(string capabilityName, object value)
    {
        Capabilities ??= new Dictionary<string, object>();
        Capabilities[capabilityName] = value;
    }

    /// <summary>
    /// Gets a capability value.
    /// </summary>
    public object? GetCapability(string capabilityName)
    {
        return Capabilities?.TryGetValue(capabilityName, out var value) == true ? value : null;
    }
}
