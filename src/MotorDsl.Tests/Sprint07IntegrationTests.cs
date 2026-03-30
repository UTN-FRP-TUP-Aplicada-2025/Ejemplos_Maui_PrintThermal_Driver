using MotorDsl.Core.Contracts;
using MotorDsl.Core.Engine;
using MotorDsl.Core.Evaluators;
using MotorDsl.Core.Layout;
using MotorDsl.Core.Models;
using MotorDsl.Core.Validation;
using MotorDsl.Parser;
using MotorDsl.Rendering;

namespace MotorDsl.Tests;

/// <summary>
/// Integration tests Sprint 07 — Full pipeline with validation.
/// TK-58
/// </summary>
public class Sprint07IntegrationTests
{
    private DocumentEngine CreateFullEngine()
    {
        var parser = new DslParser();
        var evaluator = new Evaluator();
        var layoutEngine = new LayoutEngine();
        var registry = new RendererRegistry();
        registry.Register(new TextRenderer());
        var dataValidator = new DataValidator();
        var templateValidator = new TemplateValidator();
        var profileValidator = new ProfileValidator();

        return new DocumentEngine(parser, evaluator, layoutEngine, registry,
            dataValidator, templateValidator, profileValidator);
    }

    [Fact]
    public void FullPipeline_ValidTemplateAndData_IsSuccessfulNoWarnings()
    {
        var engine = CreateFullEngine();
        var dsl = """
        {
            "id": "int-001",
            "version": "1.0",
            "root": {
                "type": "text",
                "text": "Hola {{nombre}}"
            }
        }
        """;
        var data = new { nombre = "Mundo" };
        var profile = new DeviceProfile("thermal-58mm", 32, "text");

        var result = engine.Render(dsl, data, profile);

        Assert.True(result.IsSuccessful);
        Assert.Empty(result.Warnings);
    }

    [Fact]
    public void FullPipeline_UnknownNodeType_ReturnsError()
    {
        var engine = CreateFullEngine();
        var dsl = """
        {
            "id": "int-002",
            "version": "1.0",
            "root": {
                "type": "foobar",
                "text": "Hola"
            }
        }
        """;
        var data = new { };
        var profile = new DeviceProfile("thermal-58mm", 32, "text");

        var result = engine.Render(dsl, data, profile);

        Assert.False(result.IsSuccessful);
        Assert.Contains(result.Errors, e => e.Contains("TemplateValidation") && e.Contains("UnknownNodeType"));
    }

    [Fact]
    public void FullPipeline_ProfileWidthZero_ReturnsError()
    {
        var engine = CreateFullEngine();
        var dsl = """
        {
            "id": "int-003",
            "version": "1.0",
            "root": {
                "type": "text",
                "text": "Hola"
            }
        }
        """;
        var data = new { };
        var profile = new DeviceProfile("thermal-58mm", 0, "text");

        var result = engine.Render(dsl, data, profile);

        Assert.False(result.IsSuccessful);
        Assert.Contains(result.Errors, e => e.Contains("ProfileValidation"));
    }

    [Fact]
    public void FullPipeline_NullField_IsSuccessfulWithWarning()
    {
        var engine = CreateFullEngine();
        var dsl = """
        {
            "id": "int-004",
            "version": "1.0",
            "root": {
                "type": "text",
                "text": "Obs: {{observaciones}}"
            }
        }
        """;
        var data = new { observaciones = (string?)null };
        var profile = new DeviceProfile("thermal-58mm", 32, "text");

        var result = engine.Render(dsl, data, profile);

        Assert.True(result.IsSuccessful);
        Assert.Contains(result.Warnings, w => w.Contains("observaciones"));
    }

    [Fact]
    public void RenderLayout_InvalidTemplate_ThrowsInvalidOperationException()
    {
        var engine = CreateFullEngine();
        var dsl = "{ esto no es JSON {{{";
        var data = new { };
        var profile = new DeviceProfile("thermal-58mm", 32, "text");

        Assert.Throws<InvalidOperationException>(() =>
            engine.RenderLayout(dsl, data, profile));
    }
}
