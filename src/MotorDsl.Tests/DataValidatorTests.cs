using MotorDsl.Core.Contracts;
using MotorDsl.Core.Models;
using MotorDsl.Core.Validation;

namespace MotorDsl.Tests;

/// <summary>
/// Tests para DataValidator (IDataValidator).
/// Sprint 06 | TK-36, TK-37, TK-38
/// 9 test cases.
/// </summary>
public class DataValidatorTests
{
    private readonly IDataValidator _validator = new DataValidator();

    // ═══════════════════════════════════════════════════
    // Categoría 1 — MissingField en TextNode
    // ═══════════════════════════════════════════════════

    [Fact]
    public void Validate_TextNodeWithExistingBinding_IsValid()
    {
        var ast = new TextNode("Hola {{nombre}}");
        var data = new Dictionary<string, object> { { "nombre", "Juan" } };

        var result = _validator.Validate(ast, data);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Validate_TextNodeWithMissingBinding_ReturnsMissingField()
    {
        var ast = new TextNode("Hola {{nombre}}");
        var data = new Dictionary<string, object>();

        var result = _validator.Validate(ast, data);

        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal(ValidationErrorType.MissingField, result.Errors[0].Type);
        Assert.Equal("nombre", result.Errors[0].Field);
        Assert.Equal("text", result.Errors[0].NodeType);
    }

    // ═══════════════════════════════════════════════════
    // Categoría 2 — MissingField en LoopNode
    // ═══════════════════════════════════════════════════

    [Fact]
    public void Validate_LoopNodeWithExistingSource_IsValid()
    {
        var body = new TextNode("Item: {{item.nombre}}");
        var ast = new LoopNode("items", "item", body);
        var data = new Dictionary<string, object>
        {
            { "items", new List<object> { new Dictionary<string, object> { { "nombre", "Café" } } } }
        };

        var result = _validator.Validate(ast, data);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Validate_LoopNodeWithMissingSource_ReturnsMissingField()
    {
        var body = new TextNode("Item");
        var ast = new LoopNode("items", "item", body);
        var data = new Dictionary<string, object>();

        var result = _validator.Validate(ast, data);

        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal(ValidationErrorType.MissingField, result.Errors[0].Type);
        Assert.Equal("items", result.Errors[0].Field);
        Assert.Equal("loop", result.Errors[0].NodeType);
    }

    // ═══════════════════════════════════════════════════
    // Categoría 3 — TypeMismatch en LoopNode
    // ═══════════════════════════════════════════════════

    [Fact]
    public void Validate_LoopNodeSourceNotCollection_ReturnsTypeMismatch()
    {
        var body = new TextNode("Item");
        var ast = new LoopNode("total", "item", body);
        var data = new Dictionary<string, object> { { "total", 150 } };

        var result = _validator.Validate(ast, data);

        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal(ValidationErrorType.TypeMismatch, result.Errors[0].Type);
        Assert.Equal("total", result.Errors[0].Field);
        Assert.Equal("loop", result.Errors[0].NodeType);
    }

    // ═══════════════════════════════════════════════════
    // Categoría 4 — InvalidStructure
    // ═══════════════════════════════════════════════════

    [Fact]
    public void Validate_ConditionalNodeWithoutBranches_ReturnsInvalidStructure()
    {
        var ast = new ConditionalNode("status == 'activo'", trueBranch: null, falseBranch: null);
        var data = new Dictionary<string, object> { { "status", "activo" } };

        var result = _validator.Validate(ast, data);

        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal(ValidationErrorType.InvalidStructure, result.Errors[0].Type);
        Assert.Equal("conditional", result.Errors[0].NodeType);
    }

    [Fact]
    public void Validate_LoopNodeWithoutBody_ReturnsInvalidStructure()
    {
        var ast = new LoopNode("items", "item", body: null);
        var data = new Dictionary<string, object>
        {
            { "items", new List<object> { "a", "b" } }
        };

        var result = _validator.Validate(ast, data);

        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Equal(ValidationErrorType.InvalidStructure, result.Errors[0].Type);
        Assert.Equal("loop", result.Errors[0].NodeType);
    }

    // ═══════════════════════════════════════════════════
    // Categoría 5 — Casos válidos
    // ═══════════════════════════════════════════════════

    [Fact]
    public void Validate_CompleteDocumentWithAllFields_IsValid()
    {
        var container = new ContainerNode();
        container.AddChild(new TextNode("Cliente: {{cliente}}"));
        container.AddChild(new LoopNode("items", "item", new TextNode("{{item.nombre}}")));
        container.AddChild(new ConditionalNode("mostrar_total", new TextNode("Total: {{total}}")));

        var data = new Dictionary<string, object>
        {
            { "cliente", "Juan Pérez" },
            { "items", new List<object> { new Dictionary<string, object> { { "nombre", "Café" } } } },
            { "mostrar_total", true },
            { "total", "$150.00" }
        };

        var result = _validator.Validate(container, data);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Validate_EmptyDataWithNoBindings_IsValid()
    {
        var ast = new TextNode("Texto plano sin bindings");
        var data = new Dictionary<string, object>();

        var result = _validator.Validate(ast, data);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }
}
