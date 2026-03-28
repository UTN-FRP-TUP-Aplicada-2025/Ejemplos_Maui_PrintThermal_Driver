namespace MotorDsl.Core.Models;

/// <summary>
/// Represents the result of a rendering operation.
/// Contains the generated output along with any diagnostic information.
/// 
/// Source: modelo-datos-logico_v1.0.md (Section 11)
/// Contracts: contratos-del-motor_v1.0.md (Section 6)
/// </summary>
public class RenderResult
{
    /// <summary>
    /// Gets or sets the rendering target type that produced this result.
    /// Should match the RenderTarget from DeviceProfile.
    /// </summary>
    public string Target { get; set; }

    /// <summary>
    /// Gets or sets the rendering output.
    /// Type depends on the renderer:
    ///   - ESC/POS: byte[] with printer commands
    ///   - Text: string with plain text
    ///   - UI: UIElement or View object
    ///   - PDF: byte[] with PDF document
    /// </summary>
    public object? Output { get; set; }

    /// <summary>
    /// Gets or sets a list of warning messages.
    /// Non-critical issues that don't affect rendering.
    /// </summary>
    public List<string> Warnings { get; set; }

    /// <summary>
    /// Gets or sets a list of error messages.
    /// Critical issues that may affect the output quality.
    /// </summary>
    public List<string> Errors { get; set; }

    /// <summary>
    /// Constructor for render results.
    /// </summary>
    /// <param name="target">Target render type</param>
    /// <param name="output">Rendered output</param>
    public RenderResult(string target, object? output = null)
    {
        Target = target;
        Output = output;
        Warnings = new List<string>();
        Errors = new List<string>();
    }

    /// <summary>
    /// Indicates whether the rendering completed successfully (no errors).
    /// </summary>
    public bool IsSuccessful => Errors.Count == 0;

    /// <summary>
    /// Adds a warning message.
    /// </summary>
    public void AddWarning(string message)
    {
        Warnings.Add(message);
    }

    /// <summary>
    /// Adds an error message.
    /// </summary>
    public void AddError(string message)
    {
        Errors.Add(message);
    }
}
