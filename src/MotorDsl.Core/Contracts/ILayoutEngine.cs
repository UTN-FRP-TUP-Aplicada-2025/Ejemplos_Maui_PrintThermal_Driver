using MotorDsl.Core.Models;

namespace MotorDsl.Core.Contracts;

/// <summary>
/// Interface for applying layout rules to an abstract document.
/// Adjusts document structure and formatting based on device profile constraints.
/// 
/// Source: contratos-del-motor_v1.0.md (Section 8)
/// Architecture: arquitectura-solucion_v1.0.md (Section 5.7)
/// </summary>
public interface ILayoutEngine
{
    /// <summary>
    /// Applies layout transformations to a document tree based on device profile.
    /// This includes adjustments for width, wrapping, and other device-specific layout rules.
    /// </summary>
    /// <param name="document">Root document node</param>
    /// <param name="profile">Device profile defining layout constraints (width, capabilities, etc.)</param>
    /// <returns>Document node tree with applied layout rules</returns>
    DocumentNode ApplyLayout(DocumentNode document, DeviceProfile profile);
}
