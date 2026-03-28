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
    private readonly Dictionary<string, IRenderer> _renderers;

    public DocumentEngine(IDslParser parser, IEvaluator evaluator, ILayoutEngine layoutEngine, IRenderer defaultRenderer)
    {
        _parser = parser;
        _evaluator = evaluator;
        _layoutEngine = layoutEngine;
        _renderers = new Dictionary<string, IRenderer>
        {
            [defaultRenderer.Target] = defaultRenderer
        };
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
        if (_renderers.TryGetValue(target, out var renderer))
            return renderer;

        // Default to text renderer
        if (_renderers.TryGetValue("text", out var textRenderer))
            return textRenderer;

        throw new InvalidOperationException($"No renderer found for target: {target}");
    }
}
