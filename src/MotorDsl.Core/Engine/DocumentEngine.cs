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

    public DocumentEngine(IDslParser parser, IEvaluator evaluator, ILayoutEngine layoutEngine, IRendererRegistry rendererRegistry, IDataValidator? validator = null)
    {
        _parser = parser;
        _evaluator = evaluator;
        _layoutEngine = layoutEngine;
        _rendererRegistry = rendererRegistry;
        _validator = validator;
    }

    public RenderResult Render(string templateDsl, object data, DeviceProfile profile)
    {
        try
        {
            var template = _parser.Parse(templateDsl);
            return Render(template, data, profile);
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

    private IRenderer GetRenderer(string target)
    {
        return _rendererRegistry.GetRenderer(target)
            ?? throw new InvalidOperationException($"No renderer found for target: {target}");
    }
}
