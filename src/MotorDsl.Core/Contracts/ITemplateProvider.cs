namespace MotorDsl.Core.Contracts;

/// <summary>
/// Contract for providing DSL templates to the engine.
/// Read-only — the source of templates is the client's responsibility.
/// Sprint 05 — TK-27 / CU-24 v2.0
/// </summary>
public interface ITemplateProvider
{
    string? GetTemplate(string templateId);
    IEnumerable<string> GetAvailableTemplateIds();
}
