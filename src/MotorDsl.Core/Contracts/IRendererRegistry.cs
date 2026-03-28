using MotorDsl.Core.Models;

namespace MotorDsl.Core.Contracts;

/// <summary>
/// Registry for managing available renderers by target type.
/// Sprint 04 | TK-22
/// </summary>
public interface IRendererRegistry
{
    void Register(IRenderer renderer);
    IRenderer? GetRenderer(string target);
    IEnumerable<string> GetAvailableTargets();
}
