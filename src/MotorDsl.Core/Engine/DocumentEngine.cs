using MotorDsl.Core.Contracts;
using MotorDsl.Core.Evaluators;
using MotorDsl.Core.Layout;
using MotorDsl.Core.Models;

namespace MotorDsl.Core.Engine;

/// <summary>
/// Orchestrates the full 4-stage pipeline: Parse → Evaluate → Layout → Render.
/// </summary>
public class DocumentEngine : IDocumentEngine
{
    private readonly IDslParser _parser;
    private readonly IEvaluator _evaluator;
    private readonly ILayoutEngine _layoutEngine;
    private readonly IRendererRegistry _rendererRegistry;
    private readonly IDataValidator? _validator;
    private readonly ITemplateValidator? _templateValidator;
    private readonly IProfileValidator? _profileValidator;

    public DocumentEngine(IDslParser parser, IEvaluator evaluator, ILayoutEngine layoutEngine, IRendererRegistry rendererRegistry, IDataValidator? validator = null, ITemplateValidator? templateValidator = null, IProfileValidator? profileValidator = null)
    {
        _parser = parser;
        _evaluator = evaluator;
        _layoutEngine = layoutEngine;
        _rendererRegistry = rendererRegistry;
        _validator = validator;
        _templateValidator = templateValidator;
        _profileValidator = profileValidator;
    }

    public RenderResult Render(string templateDsl, object data, DeviceProfile profile)
    {
        try
        {
            // TK-51: Validate template DSL before parsing
            var warnings = new List<string>();
            if (_templateValidator != null)
            {
                var templateValidation = _templateValidator.ValidateTemplate(templateDsl);
                foreach (var err in templateValidation.Errors)
                {
                    if (err.Severity == ValidationSeverity.Error)
                    {
                        var errorResult = new RenderResult(profile.RenderTarget);
                        foreach (var e in templateValidation.Errors.Where(e => e.Severity == ValidationSeverity.Error))
                            errorResult.AddError($"TemplateValidation: [{e.Type}] {e.Field} \u2014 {e.Message}");
                        errorResult.Output = "";
                        return errorResult;
                    }
                    if (err.Severity == ValidationSeverity.Warning)
                        warnings.Add($"TemplateValidation: [{err.Type}] {err.Field} \u2014 {err.Message}");
                }
            }

            var template = _parser.Parse(templateDsl);
            var result = Render(template, data, profile);

            // TK-56: Propagate accumulated warnings
            foreach (var w in warnings)
                result.AddWarning(w);

            return result;
        }
        catch (Exception ex)
        {
            var result = new RenderResult(profile.RenderTarget);
            result.AddError($"Parse failed: {ex.Message}");
            result.Output = "";
            return result;
        }
    }

    public RenderResult Render(DocumentTemplate template, object data, DeviceProfile profile)
    {
        try
        {
            // Stage 1.5: Validate (optional)
            if (_validator != null)
            {
                var validation = _validator.Validate(template.Root!, data);
                if (!validation.IsValid)
                {
                    var errorResult = new RenderResult(profile.RenderTarget);
                    foreach (var error in validation.Errors)
                        errorResult.AddError($"Validation: [{error.Type}] {error.Field} \u2014 {error.Message}");
                    errorResult.Output = "";
                    return errorResult;
                }
            }

            // TK-54: Validate profile before layout
            if (_profileValidator != null)
            {
                var profileValidation = _profileValidator.ValidateProfile(profile);
                if (!profileValidation.IsValid)
                {
                    var errorResult = new RenderResult(profile.RenderTarget);
                    foreach (var e in profileValidation.Errors.Where(e => e.Severity == ValidationSeverity.Error))
                        errorResult.AddError($"ProfileValidation: [{e.Type}] {e.Field} \u2014 {e.Message}");
                    errorResult.Output = "";
                    return errorResult;
                }
            }

            // Stage 2: Evaluate
            var evaluated = _evaluator.Evaluate(template.Root!, data);

            // Stage 3: Layout
            var layouted = _layoutEngine.ApplyLayout(evaluated, profile);

            // Stage 4: Render
            var renderer = GetRenderer(profile.RenderTarget);
            return renderer.Render(layouted, profile);
        }
        catch (Exception ex)
        {
            var result = new RenderResult(profile.RenderTarget);
            result.AddError($"Render failed: {ex.Message}");
            result.Output = "";
            return result;
        }
    }

    public LayoutedDocument RenderLayout(string templateDsl, object data, DeviceProfile profile)
    {
        // TK-51: Validate template DSL
        if (_templateValidator != null)
        {
            var templateValidation = _templateValidator.ValidateTemplate(templateDsl);
            if (templateValidation.Errors.Any(e => e.Severity == ValidationSeverity.Error))
                throw new InvalidOperationException(
                    "Template validation failed: " + string.Join("; ", templateValidation.Errors.Where(e => e.Severity == ValidationSeverity.Error).Select(e => e.Message)));
        }

        var template = _parser.Parse(templateDsl);

        if (_validator != null)
        {
            var validation = _validator.Validate(template.Root!, data);
            if (!validation.IsValid)
                throw new InvalidOperationException(
                    "Validation failed: " + string.Join("; ", validation.Errors.Select(e => e.Message)));
        }

        // TK-54: Validate profile before layout
        if (_profileValidator != null)
        {
            var profileValidation = _profileValidator.ValidateProfile(profile);
            if (profileValidation.Errors.Any(e => e.Severity == ValidationSeverity.Error))
                throw new InvalidOperationException(
                    "Profile validation failed: " + string.Join("; ", profileValidation.Errors.Where(e => e.Severity == ValidationSeverity.Error).Select(e => e.Message)));
        }

        var evaluated = _evaluator.Evaluate(template.Root!, data);
        return _layoutEngine.ApplyLayout(evaluated, profile);
    }

    private IRenderer GetRenderer(string target)
    {
        return _rendererRegistry.GetRenderer(target)
            ?? throw new InvalidOperationException($"No renderer found for target: {target}");
    }
}
