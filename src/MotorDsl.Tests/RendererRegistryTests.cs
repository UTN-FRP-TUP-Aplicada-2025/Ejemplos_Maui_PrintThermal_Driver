using MotorDsl.Core.Contracts;
using MotorDsl.Core.Engine;
using MotorDsl.Core.Models;
using MotorDsl.Rendering;

namespace MotorDsl.Tests;

/// <summary>
/// Tests para IRendererRegistry + RendererRegistry.
/// Sprint 04 | TK-22
/// 4 tests.
/// </summary>
public class RendererRegistryTests
{
    // ─── Register() + GetRenderer() por target exacto ───
    [Fact]
    public void Register_ThenGetRenderer_ReturnsSameInstance()
    {
        var registry = new RendererRegistry();
        var textRenderer = new TextRenderer();

        registry.Register(textRenderer);

        var retrieved = registry.GetRenderer("text");
        Assert.NotNull(retrieved);
        Assert.Same(textRenderer, retrieved);
    }

    // ─── GetAvailableTargets() lista los registrados ───
    [Fact]
    public void GetAvailableTargets_ReturnsAllRegisteredTargets()
    {
        var registry = new RendererRegistry();
        registry.Register(new TextRenderer());
        registry.Register(new StubRenderer("escpos"));

        var targets = registry.GetAvailableTargets().ToList();

        Assert.Equal(2, targets.Count);
        Assert.Contains("text", targets);
        Assert.Contains("escpos", targets);
    }

    // ─── Fallback a "text" cuando target no existe ───
    [Fact]
    public void GetRenderer_UnknownTarget_FallsBackToText()
    {
        var registry = new RendererRegistry();
        var textRenderer = new TextRenderer();
        registry.Register(textRenderer);
        registry.Register(new StubRenderer("escpos"));

        var result = registry.GetRenderer("pdf");

        Assert.NotNull(result);
        Assert.Same(textRenderer, result);
        Assert.Equal("text", result!.Target);
    }

    // ─── Target no registrado y sin fallback → null ───
    [Fact]
    public void GetRenderer_UnknownTarget_NoTextRegistered_ReturnsNull()
    {
        var registry = new RendererRegistry();
        registry.Register(new StubRenderer("escpos"));

        var result = registry.GetRenderer("pdf");

        Assert.Null(result);
    }

    // ─── Stub renderer para tests ───
    private class StubRenderer : IRenderer
    {
        public string Target { get; }
        public StubRenderer(string target) => Target = target;
        public RenderResult Render(LayoutedDocument document, DeviceProfile profile)
            => new(Target, Array.Empty<byte>());
    }
}
