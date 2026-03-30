using Microsoft.Extensions.DependencyInjection;
using MotorDsl.Core.Contracts;
using MotorDsl.Core.Layout;
using MotorDsl.Core.Models;
using MotorDsl.Extensions;

namespace MotorDsl.Tests;

/// <summary>
/// TDD tests para Sprint 08 — Bloque 1: Core Changes.
/// Tests escritos ANTES de la implementación.
/// TK-59: LayoutEngine bitmap metadata
/// TK-60: AddRenderer&lt;T&gt;() extensibilidad
/// TK-61: IBitmapRasterizer contrato
/// </summary>
public class Sprint08CoreTests
{
    private readonly ILayoutEngine _layoutEngine = new LayoutEngine();

    private DeviceProfile DefaultProfile() => new("thermal-58mm", 32, "text");

    private EvaluatedDocument MakeEvaluated(DocumentNode root) => new()
    {
        Id = "test",
        Version = "1.0",
        Root = root
    };

    // ─── TK-59-01: LayoutEngine con ImageNode bitmap → metadata is_bitmap=true ───
    [Fact]
    public void ApplyLayout_BitmapImageNode_HasIsBitmapMetadata()
    {
        var bitmapNode = new ImageNode("data:image/png;base64,iVBORw0KGgo=");
        var doc = MakeEvaluated(bitmapNode);

        var result = _layoutEngine.ApplyLayout(doc, DefaultProfile());

        var layout = result.NodeLayoutInfo.Values.First();
        Assert.True(layout.DeviceMetadata.ContainsKey("is_bitmap"),
            "Debe tener metadata 'is_bitmap'");
        Assert.True((bool)layout.DeviceMetadata["is_bitmap"],
            "is_bitmap debe ser true");
    }

    // ─── TK-59-02: LayoutEngine con ImageNode bitmap → WrappedText contiene "[BITMAP:" ───
    [Fact]
    public void ApplyLayout_BitmapImageNode_WrappedTextContainsBitmapMarker()
    {
        var bitmapNode = new ImageNode("data:image/png;base64,iVBORw0KGgo=");
        var doc = MakeEvaluated(bitmapNode);

        var result = _layoutEngine.ApplyLayout(doc, DefaultProfile());

        var layout = result.NodeLayoutInfo.Values.First();
        Assert.Contains("[BITMAP:", layout.WrappedText);
    }

    // ─── TK-59-03: LayoutEngine con ImageNode qrcode → NO tiene is_bitmap ───
    [Fact]
    public void ApplyLayout_QrCodeImageNode_DoesNotHaveIsBitmapMetadata()
    {
        var qrNode = new ImageNode("https://example.com", imageType: "qrcode");
        var doc = MakeEvaluated(qrNode);

        var result = _layoutEngine.ApplyLayout(doc, DefaultProfile());

        var layout = result.NodeLayoutInfo.Values.First();
        Assert.False(layout.DeviceMetadata.ContainsKey("is_bitmap"),
            "QR no debe tener metadata 'is_bitmap'");
    }

    // ─── TK-60-01: AddRenderer<T>() registra renderer en el registry ───
    [Fact]
    public void AddRenderer_CustomRenderer_IsAvailableInRegistry()
    {
        var services = new ServiceCollection();
        var builder = services.AddMotorDslEngine();

        builder.AddRenderer<FakeRenderer>();

        var sp = services.BuildServiceProvider();
        var registry = sp.GetRequiredService<IRendererRegistry>();
        Assert.NotNull(registry.GetRenderer("fake"));
        Assert.Contains("fake", registry.GetAvailableTargets());
    }

    // ─── TK-61-01: IBitmapRasterizer existe en MotorDsl.Core.Contracts ───
    [Fact]
    public void IBitmapRasterizer_ExistsInCoreContracts()
    {
        var type = typeof(IBitmapRasterizer);
        Assert.NotNull(type);
        Assert.True(type.IsInterface, "IBitmapRasterizer debe ser una interface");
        Assert.Equal("MotorDsl.Core.Contracts", type.Namespace);
    }

    // ── Helper: dummy renderer para test TK-60 ──
    private class FakeRenderer : IRenderer
    {
        public string Target => "fake";
        public RenderResult Render(LayoutedDocument document, DeviceProfile profile)
            => new("fake", "fake-output");
    }
}
