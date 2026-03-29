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
/// Tests para DocumentEngine (IDocumentEngine).
/// Sprint 03 | BT-066 a BT-071
/// 12 test cases.
/// </summary>
public class DocumentEngineTests
{
    private IDocumentEngine CreateEngine()
    {
        var registry = new RendererRegistry();
        registry.Register(new TextRenderer());
        return new DocumentEngine(
            new DslParser(),
            new Evaluator(),
            new LayoutEngine(),
            registry
        );
    }

    private DeviceProfile DefaultProfile() => new("thermal-58mm", 32, "text");

    // ─── BT-066: Pipeline completo con DSL simple ───
    [Fact]
    public void Render_SimpleDsl_ProducesOutput()
    {
        var engine = CreateEngine();
        var dsl = """
        {
            "id": "test-001",
            "version": "1.0",
            "root": {
                "type": "text",
                "text": "Hola Motor DSL"
            }
        }
        """;

        var result = engine.Render(dsl, new { }, DefaultProfile());

        Assert.NotNull(result);
        Assert.NotNull(result.Output);
        var output = result.Output.ToString()!;
        Assert.Contains("Hola Motor DSL", output);
    }

    // ─── BT-067: Pipeline con container vertical ───
    [Fact]
    public void Render_ContainerVertical_AllLinesPresent()
    {
        var engine = CreateEngine();
        var dsl = """
        {
            "id": "test-002",
            "version": "1.0",
            "root": {
                "type": "container",
                "layout": "vertical",
                "children": [
                    { "type": "text", "text": "Encabezado" },
                    { "type": "text", "text": "Detalle" },
                    { "type": "text", "text": "Pie" }
                ]
            }
        }
        """;

        var result = engine.Render(dsl, new { }, DefaultProfile());
        var output = result.Output!.ToString()!;

        Assert.Contains("Encabezado", output);
        Assert.Contains("Detalle", output);
        Assert.Contains("Pie", output);
    }

    // ─── BT-068: Pipeline con data binding ───
    [Fact]
    public void Render_ContainerWithDataBinding_ResolvesAndRenders()
    {
        var engine = CreateEngine();
        var dsl = """
        {
            "id": "test-003",
            "version": "1.0",
            "root": {
                "type": "container",
                "layout": "vertical",
                "children": [
                    { "type": "text", "text": "Cliente: {{nombre}}" },
                    { "type": "text", "text": "Total: {{total}}" }
                ]
            }
        }
        """;
        var data = new { nombre = "María", total = "1500" };

        var result = engine.Render(dsl, data, DefaultProfile());
        var output = result.Output?.ToString() ?? "";

        Assert.Contains("María", output);
        Assert.Contains("1500", output);
    }

    // ─── BT-069: Pipeline con condicional true ───
    [Fact]
    public void Render_ConditionalNode_EvaluatesAndRenders()
    {
        var engine = CreateEngine();
        var dsl = """
        {
            "id": "test-004",
            "version": "1.0",
            "root": {
                "type": "conditional",
                "expression": "mostrar == true",
                "trueBranch": { "type": "text", "text": "Se muestra" },
                "falseBranch": { "type": "text", "text": "No se muestra" }
            }
        }
        """;
        var data = new { mostrar = true };

        var result = engine.Render(dsl, data, DefaultProfile());
        var output = result.Output?.ToString() ?? "";

        Assert.Contains("Se muestra", output);
        Assert.DoesNotContain("No se muestra", output);
    }

    // ─── BT-070: Pipeline con loop ───
    [Fact]
    public void Render_LoopNode_IteratesAndRenders()
    {
        var engine = CreateEngine();
        var dsl = """
        {
            "id": "test-005",
            "version": "1.0",
            "root": {
                "type": "loop",
                "source": "items",
                "itemAlias": "item",
                "body": { "type": "text", "text": "Item: {{item.nombre}}" }
            }
        }
        """;
        var data = new
        {
            items = new[]
            {
                new { nombre = "Artículo 1" },
                new { nombre = "Artículo 2" },
                new { nombre = "Artículo 3" }
            }
        };

        var result = engine.Render(dsl, data, DefaultProfile());
        var output = result.Output?.ToString() ?? "";

        Assert.Contains("Artículo 1", output);
        Assert.Contains("Artículo 2", output);
        Assert.Contains("Artículo 3", output);
    }

    // ─── BT-071a: DSL inválido devuelve error ───
    [Fact]
    public void Render_InvalidDsl_ReturnsError()
    {
        var engine = CreateEngine();
        var dsl = "esto no es JSON válido {{{";

        var result = engine.Render(dsl, new { }, DefaultProfile());

        Assert.False(result.IsSuccessful);
        Assert.True(result.Errors.Count > 0);
    }

    // ─── BT-071b: Template pre-parseado funciona ───
    [Fact]
    public void Render_FromTemplate_ProducesOutput()
    {
        var engine = CreateEngine();
        var template = new DocumentTemplate("tpl-001", "1.0",
            new TextNode("Desde template"));

        var result = engine.Render(template, new { }, DefaultProfile());
        var output = result.Output!.ToString()!;

        Assert.Contains("Desde template", output);
    }

    // ─── BT-071c: Condicional false branch ───
    [Fact]
    public void Render_ConditionalFalse_RendersFalseBranch()
    {
        var engine = CreateEngine();
        var dsl = """
        {
            "id": "test-006",
            "version": "1.0",
            "root": {
                "type": "conditional",
                "expression": "activo == true",
                "trueBranch": { "type": "text", "text": "Activo" },
                "falseBranch": { "type": "text", "text": "Inactivo" }
            }
        }
        """;
        var data = new { activo = false };

        var result = engine.Render(dsl, data, DefaultProfile());
        var output = result.Output?.ToString() ?? "";

        Assert.Contains("Inactivo", output);
        Assert.DoesNotContain("Activo\n", output);
    }

    // ─── BT-071d: RenderResult tiene target correcto ───
    [Fact]
    public void Render_ResultTarget_MatchesProfile()
    {
        var engine = CreateEngine();
        var template = new DocumentTemplate("tpl-002", "1.0",
            new TextNode("Test"));

        var result = engine.Render(template, new { }, DefaultProfile());

        Assert.Equal("text", result.Target);
    }

    // ─── BT-071e: Container vacío no falla ───
    [Fact]
    public void Render_EmptyContainer_DoesNotFail()
    {
        var engine = CreateEngine();
        var dsl = """
        {
            "id": "test-007",
            "version": "1.0",
            "root": {
                "type": "container",
                "layout": "vertical",
                "children": []
            }
        }
        """;

        var result = engine.Render(dsl, new { }, DefaultProfile());

        Assert.NotNull(result);
        Assert.True(result.IsSuccessful);
    }

    // ─── BT-071f: Data binding con path anidado ───
    [Fact]
    public void Render_NestedDataBinding_Resolves()
    {
        var engine = CreateEngine();
        var dsl = """
        {
            "id": "test-008",
            "version": "1.0",
            "root": {
                "type": "text",
                "text": "Hola {{cliente.nombre}}"
            }
        }
        """;
        var data = new { cliente = new { nombre = "Carlos" } };

        var result = engine.Render(dsl, data, DefaultProfile());
        var output = result.Output?.ToString() ?? "";

        Assert.Contains("Carlos", output);
    }

    // ═══════════════════════════════════════════════════
    // Sprint 06 | TK-39 — Validator integration in pipeline
    // ═══════════════════════════════════════════════════

    private IDocumentEngine CreateEngineWithValidator()
    {
        var registry = new RendererRegistry();
        registry.Register(new TextRenderer());
        return new DocumentEngine(
            new DslParser(),
            new Evaluator(),
            new LayoutEngine(),
            registry,
            new DataValidator()
        );
    }

    [Fact]
    public void DocumentEngine_WithValidator_InvalidData_ReturnsErrors()
    {
        var engine = CreateEngineWithValidator();
        var dsl = """
        {
            "id": "val-001",
            "version": "1.0",
            "root": {
                "type": "text",
                "text": "Cliente: {{nombre}}"
            }
        }
        """;
        var data = new Dictionary<string, object>(); // campo "nombre" faltante

        var result = engine.Render(dsl, data, DefaultProfile());

        Assert.False(result.IsSuccessful);
        Assert.NotEmpty(result.Errors);
        Assert.Contains(result.Errors, e => e.Contains("nombre"));
        Assert.Equal("", result.Output?.ToString());
    }

    [Fact]
    public void DocumentEngine_WithValidator_ValidData_RendersProperly()
    {
        var engine = CreateEngineWithValidator();
        var dsl = """
        {
            "id": "val-002",
            "version": "1.0",
            "root": {
                "type": "text",
                "text": "Cliente: {{nombre}}"
            }
        }
        """;
        var data = new Dictionary<string, object> { { "nombre", "Juan" } };

        var result = engine.Render(dsl, data, DefaultProfile());

        Assert.True(result.IsSuccessful);
        var output = result.Output?.ToString() ?? "";
        Assert.Contains("Juan", output);
    }
}
