using MotorDsl.Core.Contracts;
using MotorDsl.Core.Models;

namespace MotorDsl.Tests;

/// <summary>
/// Tests TDD para Sprint 07 — Validación formal.
/// TK-48: Severity + Location en ValidationError
/// TK-49/50: ITemplateValidator + TemplateValidator
/// TK-52/53: IProfileValidator + ProfileValidator
/// Sprint 07 | TK-57
/// </summary>
public class Sprint07ValidationTests
{
    // ═══════════════════════════════════════════
    // Categoría 1 — ValidationError con Severity (TK-48)
    // ═══════════════════════════════════════════

    [Fact]
    public void ValidationError_DefaultSeverity_IsError()
    {
        var error = new ValidationError("campo", ValidationErrorType.MissingField, "falta campo", "TextNode");

        Assert.Equal(ValidationSeverity.Error, error.Severity);
    }

    [Fact]
    public void ValidationError_CanCreateWithSeverityWarning()
    {
        var error = new ValidationError("campo", ValidationErrorType.MissingField, "valor null", "TextNode")
        {
            Severity = ValidationSeverity.Warning
        };

        Assert.Equal(ValidationSeverity.Warning, error.Severity);
    }

    [Fact]
    public void ValidationError_LocationDefaultIsNull_CanBeSet()
    {
        var errorDefault = new ValidationError("campo", ValidationErrorType.MissingField, "falta", "TextNode");
        Assert.Null(errorDefault.Location);

        var errorWithLocation = new ValidationError("source", ValidationErrorType.MissingField, "falta source", "LoopNode")
        {
            Location = "root.children[2].body"
        };
        Assert.Equal("root.children[2].body", errorWithLocation.Location);
    }

    // ═══════════════════════════════════════════
    // Categoría 2 — ITemplateValidator (TK-49/50)
    // ═══════════════════════════════════════════

    private ITemplateValidator CreateTemplateValidator() => new MotorDsl.Core.Validation.TemplateValidator();

    [Fact]
    public void TemplateValidator_ValidSimpleTemplate_IsValid()
    {
        var validator = CreateTemplateValidator();
        var dsl = """
        {
            "id": "test-001",
            "version": "1.0",
            "root": {
                "type": "text",
                "text": "Hola mundo"
            }
        }
        """;

        var result = validator.ValidateTemplate(dsl);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void TemplateValidator_MalformedJson_InvalidSyntax()
    {
        var validator = CreateTemplateValidator();
        var dsl = "{ esto no es JSON válido {{{";

        var result = validator.ValidateTemplate(dsl);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Type == ValidationErrorType.InvalidSyntax);
    }

    [Fact]
    public void TemplateValidator_MissingId_MissingRequiredField()
    {
        var validator = CreateTemplateValidator();
        var dsl = """
        {
            "version": "1.0",
            "root": {
                "type": "text",
                "text": "Hola"
            }
        }
        """;

        var result = validator.ValidateTemplate(dsl);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e =>
            e.Type == ValidationErrorType.MissingRequiredField && e.Field == "id");
    }

    [Fact]
    public void TemplateValidator_UnknownNodeType_UnknownNodeType()
    {
        var validator = CreateTemplateValidator();
        var dsl = """
        {
            "id": "test-002",
            "version": "1.0",
            "root": {
                "type": "unknowntype",
                "text": "Hola"
            }
        }
        """;

        var result = validator.ValidateTemplate(dsl);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Type == ValidationErrorType.UnknownNodeType);
    }

    [Fact]
    public void TemplateValidator_LoopWithoutSource_MissingRequiredFieldWithLocation()
    {
        var validator = CreateTemplateValidator();
        var dsl = """
        {
            "id": "test-003",
            "version": "1.0",
            "root": {
                "type": "loop",
                "itemAlias": "item",
                "body": { "type": "text", "text": "item" }
            }
        }
        """;

        var result = validator.ValidateTemplate(dsl);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e =>
            e.Type == ValidationErrorType.MissingRequiredField
            && e.Field == "source"
            && e.Location != null);
    }

    // ═══════════════════════════════════════════
    // Categoría 3 — IProfileValidator (TK-52/53)
    // ═══════════════════════════════════════════

    private IProfileValidator CreateProfileValidator() => new MotorDsl.Core.Validation.ProfileValidator();

    [Fact]
    public void ProfileValidator_ValidProfile_IsValid()
    {
        var validator = CreateProfileValidator();
        var profile = new DeviceProfile("thermal-58mm", 32, "text");

        var result = validator.ValidateProfile(profile);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void ProfileValidator_WidthZero_ReturnsError()
    {
        var validator = CreateProfileValidator();
        var profile = new DeviceProfile("thermal-58mm", 0, "text");

        var result = validator.ValidateProfile(profile);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Field == "Width");
    }

    [Fact]
    public void ProfileValidator_RenderTargetEmpty_ReturnsError()
    {
        var validator = CreateProfileValidator();
        var profile = new DeviceProfile("thermal-58mm", 32, "");

        var result = validator.ValidateProfile(profile);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Field == "RenderTarget");
    }

    [Fact]
    public void ProfileValidator_NameEmpty_ReturnsError()
    {
        var validator = CreateProfileValidator();
        var profile = new DeviceProfile("", 32, "text");

        var result = validator.ValidateProfile(profile);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Field == "Name");
    }

    // ═══════════════════════════════════════════
    // Categoría 4 — Pipeline integration (TK-51/54/56)
    // ═══════════════════════════════════════════

    [Fact]
    public void DocumentEngine_InvalidTemplate_ReturnsRenderResultWithErrors()
    {
        var parser = new MotorDsl.Parser.DslParser();
        var evaluator = new MotorDsl.Core.Evaluators.Evaluator();
        var layoutEngine = new MotorDsl.Core.Layout.LayoutEngine();
        var registry = new MotorDsl.Core.Engine.RendererRegistry();
        registry.Register(new MotorDsl.Rendering.TextRenderer());
        var templateValidator = new MotorDsl.Core.Validation.TemplateValidator();

        var engine = new MotorDsl.Core.Engine.DocumentEngine(
            parser, evaluator, layoutEngine, registry,
            templateValidator: templateValidator);

        var profile = new DeviceProfile("test", 32, "text");
        var result = engine.Render("{ esto no es JSON válido {{{", new { }, profile);

        Assert.False(result.IsSuccessful);
        Assert.Contains(result.Errors, e => e.Contains("TemplateValidation"));
    }

    [Fact]
    public void DocumentEngine_InvalidProfile_ReturnsRenderResultWithErrors()
    {
        var parser = new MotorDsl.Parser.DslParser();
        var evaluator = new MotorDsl.Core.Evaluators.Evaluator();
        var layoutEngine = new MotorDsl.Core.Layout.LayoutEngine();
        var registry = new MotorDsl.Core.Engine.RendererRegistry();
        registry.Register(new MotorDsl.Rendering.TextRenderer());
        var profileValidator = new MotorDsl.Core.Validation.ProfileValidator();

        var engine = new MotorDsl.Core.Engine.DocumentEngine(
            parser, evaluator, layoutEngine, registry,
            profileValidator: profileValidator);

        var dsl = """
        {
            "id": "test-001",
            "version": "1.0",
            "root": {
                "type": "text",
                "text": "Hola"
            }
        }
        """;
        var profile = new DeviceProfile("test", 0, "text");
        var result = engine.Render(dsl, new { }, profile);

        Assert.False(result.IsSuccessful);
        Assert.Contains(result.Errors, e => e.Contains("ProfileValidation"));
    }
}
