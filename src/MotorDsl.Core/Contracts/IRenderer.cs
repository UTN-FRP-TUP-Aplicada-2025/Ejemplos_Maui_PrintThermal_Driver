using MotorDsl.Core.Models;

namespace MotorDsl.Core.Contracts;

/// <summary>
/// Interface for rendering an abstract document to a specific output format.
/// Each renderer handles a specific target format (ESC/POS, UI, PDF, Text, etc.).
/// 
/// Source: contratos-del-motor_v1.0.md (Section 6)
/// </summary>
public interface IRenderer
{
    /// <summary>
    /// Gets the rendering target type this renderer handles (e.g., "escpos", "ui", "pdf", "text").
    /// </summary>
    string Target { get; }

    /// <summary>
    /// Renders an abstract document node tree to the target format.
    /// </summary>
    /// <param name="document">Root document node to render</param>
    /// <param name="profile">Device profile defining rendering constraints and capabilities</param>
    /// <returns>Render result containing the formatted output</returns>
    RenderResult Render(DocumentNode document, DeviceProfile profile);
}
