using MotorDsl.Core.Contracts;

namespace MotorDsl.Core.Providers;

/// <summary>
/// In-memory implementation of ITemplateProvider.
/// Stores templates in a dictionary. Supports Add() for registration.
/// Sprint 05 — TK-27 / CU-24 v2.0
/// </summary>
public class InMemoryTemplateProvider : ITemplateProvider
{
    private readonly Dictionary<string, string> _templates = new();

    public string? GetTemplate(string templateId)
        => _templates.TryGetValue(templateId, out var t) ? t : null;

    public IEnumerable<string> GetAvailableTemplateIds()
        => _templates.Keys;

    public void Add(string templateId, string dslContent)
        => _templates[templateId] = dslContent;
}
