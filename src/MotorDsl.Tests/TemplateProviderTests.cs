using MotorDsl.Core.Contracts;
using MotorDsl.Core.Providers;

namespace MotorDsl.Tests;

/// <summary>
/// Tests for ITemplateProvider contract and InMemoryTemplateProvider implementation.
/// Sprint 05 — TK-27
/// 
/// Interface contract (read-only):
///   string? GetTemplate(string templateId)
///   IEnumerable&lt;string&gt; GetAvailableTemplateIds()
/// 
/// InMemory adds: void Add(string templateId, string dslContent)
/// </summary>
public class TemplateProviderTests
{
    // ── GetTemplate ──────────────────────────────────────────────

    [Fact]
    public void GetTemplate_RegisteredId_ReturnsDslContent()
    {
        var provider = new InMemoryTemplateProvider();
        var dsl = """{ "type": "document", "body": [] }""";
        provider.Add("ticket-venta", dsl);

        var result = provider.GetTemplate("ticket-venta");

        Assert.Equal(dsl, result);
    }

    [Fact]
    public void GetTemplate_UnknownId_ReturnsNull()
    {
        var provider = new InMemoryTemplateProvider();

        var result = provider.GetTemplate("no-existe");

        Assert.Null(result);
    }

    [Fact]
    public void GetTemplate_EmptyProvider_ReturnsNull()
    {
        var provider = new InMemoryTemplateProvider();

        var result = provider.GetTemplate("cualquier-id");

        Assert.Null(result);
    }

    // ── GetAvailableTemplateIds ──────────────────────────────────

    [Fact]
    public void GetAvailableTemplateIds_WithTemplates_ReturnsAllIds()
    {
        var provider = new InMemoryTemplateProvider();
        provider.Add("ticket-venta", "{}");
        provider.Add("recibo", "{}");

        var ids = provider.GetAvailableTemplateIds().ToList();

        Assert.Equal(2, ids.Count);
        Assert.Contains("ticket-venta", ids);
        Assert.Contains("recibo", ids);
    }

    [Fact]
    public void GetAvailableTemplateIds_EmptyProvider_ReturnsEmpty()
    {
        var provider = new InMemoryTemplateProvider();

        var ids = provider.GetAvailableTemplateIds().ToList();

        Assert.Empty(ids);
    }

    // ── Add (upsert behavior) ────────────────────────────────────

    [Fact]
    public void Add_SameIdTwice_OverwritesPreviousContent()
    {
        var provider = new InMemoryTemplateProvider();
        provider.Add("ticket", "version-1");
        provider.Add("ticket", "version-2");

        var result = provider.GetTemplate("ticket");

        Assert.Equal("version-2", result);
    }

    [Fact]
    public void Add_SameIdTwice_DoesNotDuplicateInIds()
    {
        var provider = new InMemoryTemplateProvider();
        provider.Add("ticket", "v1");
        provider.Add("ticket", "v2");

        var ids = provider.GetAvailableTemplateIds().ToList();

        Assert.Single(ids);
    }

    // ── Interface compliance ─────────────────────────────────────

    [Fact]
    public void InMemoryTemplateProvider_ImplementsITemplateProvider()
    {
        var provider = new InMemoryTemplateProvider();

        Assert.IsAssignableFrom<ITemplateProvider>(provider);
    }
}
