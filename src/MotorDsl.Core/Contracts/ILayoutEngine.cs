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
    /// Applies layout transformations to an evaluated document based on device profile.
    /// This includes adjustments for width, wrapping, alignment, and other device-specific layout rules.
    /// Produces LayoutedDocument as intermediate representation.
    /// </summary>
    /// <param name="document">Evaluated document from the Evaluator stage</param>
    /// <param name="profile">Device profile defining layout constraints (width, capabilities, etc.)</param>
    /// <returns>LayoutedDocument with calculated positions and layout information for each node</returns>
    LayoutedDocument ApplyLayout(EvaluatedDocument document, DeviceProfile profile);
}
