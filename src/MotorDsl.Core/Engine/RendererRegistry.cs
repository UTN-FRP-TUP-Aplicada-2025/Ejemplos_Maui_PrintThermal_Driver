using MotorDsl.Core.Contracts;

namespace MotorDsl.Core.Engine;

/// <summary>
/// Default implementation of IRendererRegistry.
/// Stores renderers by target in a dictionary. Falls back to "text" when target not found.
/// Sprint 04 | TK-22
/// </summary>
public class RendererRegistry : IRendererRegistry
{
    private readonly Dictionary<string, IRenderer> _renderers = new();

    public void Register(IRenderer renderer)
    {
        _renderers[renderer.Target] = renderer;
    }

    public IRenderer? GetRenderer(string target)
    {
        if (_renderers.TryGetValue(target, out var renderer))
            return renderer;

        // Fallback to "text"
        if (_renderers.TryGetValue("text", out var textRenderer))
            return textRenderer;

        return null;
    }

    public IEnumerable<string> GetAvailableTargets()
    {
        return _renderers.Keys;
    }
}
