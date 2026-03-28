using Xunit;
using MotorDsl.Core.Contracts;
using MotorDsl.Core.Models;
using MotorDsl.Core.Evaluators;
using MotorDsl.Core.Parsers;

namespace MotorDsl.Tests;

/// <summary>
/// Unit tests for Evaluator implementation.
/// Tests data resolution, conditional evaluation, loop execution, and integration.
/// 
/// Based on:
/// - CU-02: Resolver Datos del Documento
/// - CU-15: Resolver Referencias a Datos en Plantilla
/// - CU-16: Ejecutar Iteraciones (Listas)
/// - CU-17: Evaluar Condiciones en Plantilla
/// - plan-iteracion_sprint-02_v1.0.md
/// </summary>
public class EvaluatorTests
{
    #region DataResolver — Simple Variable Resolution

    [Fact]
    public void ResolveVariable_SimpleField_ReturnsValue()
    {
        // CU-15 | CU-02 — Resolver referencia de datos simple
        // Arrange
        var data = new { nombre = "Juan" };
        IDataResolver resolver = new DataResolver();

        // Act
        var result = resolver.Resolve(data, "nombre");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Juan", result);
    }

    [Fact]
    public void ResolveVariable_NestedField_ReturnsValue()
    {
        // CU-15 — Resolver referencia de datos anidada (cliente.nombre)
        // Arrange
        var data = new
        {
            cliente = new { nombre = "María", apellido = "García" }
        };
        IDataResolver resolver = new DataResolver();

        // Act
        var result = resolver.Resolve(data, "cliente.nombre");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("María", result);
    }

    [Fact]
    public void ResolveVariable_DeeplyNestedField_ReturnsValue()
    {
        // CU-15 — Resolver referencia profundamente anidada
        // Arrange
        var data = new
        {
            empresa = new
            {
                cliente = new { direccion = new { ciudad = "Buenos Aires" } }
            }
        };
        var resolver = new DataResolver();

        // Act
        var result = resolver.Resolve(data, "empresa.cliente.direccion.ciudad");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Buenos Aires", result);
    }

    [Fact]
    public void ResolveVariable_NonexistentPath_ReturnsNull()
    {
        // CU-15 | FA-01 — Dato inexistente devuelve null
        // Arrange
        var data = new { nombre = "Juan" };
        var resolver = new DataResolver();

        // Act
        var result = resolver.Resolve(data, "apellido");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ResolveVariable_ArrayIndex_ReturnsElement()
    {
        // CU-16 — Acceso a elemento de array por índice
        // Arrange
        var data = new { items = new[] { "A", "B", "C" } };
        var resolver = new DataResolver();

        // Act
        var result = resolver.Resolve(data, "items[1]");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("B", result);
    }

    #endregion

    #region DataResolver — Collection Resolution

    [Fact]
    public void ResolveCollection_SimpleArray_ReturnsEnumerable()
    {
        // CU-16 — Resolver colección simple para iteración
        // Arrange
        var data = new { items = new[] { "item1", "item2", "item3" } };
        IDataResolver resolver = new DataResolver();

        // Act
        var result = resolver.ResolveCollection(data, "items");

        // Assert
        Assert.NotNull(result);
        var list = result.ToList();
        Assert.Equal(3, list.Count);
        Assert.Equal("item1", list[0]);
    }

    [Fact]
    public void ResolveCollection_ListOfObjects_ReturnsEnumerable()
    {
        // CU-16 — Resolver colección de objetos complejos
        // Arrange
        var data = new
        {
            ordenes = new[]
            {
                new { id = 1, nombre = "Orden 1" },
                new { id = 2, nombre = "Orden 2" }
            }
        };
        var resolver = new DataResolver();

        // Act
        var result = resolver.ResolveCollection(data, "ordenes");

        // Assert
        Assert.NotNull(result);
        var list = result.ToList();
        Assert.Equal(2, list.Count);
    }

    [Fact]
    public void ResolveCollection_EmptyCollection_ReturnsEmptyEnumerable()
    {
        // CU-16 | FA-02 — Colección vacía devuelve enumerable vacío
        // Arrange
        var data = new { items = new string[] { } };
        var resolver = new DataResolver();

        // Act
        var result = resolver.ResolveCollection(data, "items");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public void ResolveCollection_NonexistentCollection_ReturnsEmpty()
    {
        // CU-16 | FA-01 — Colección inexistente devuelve vacío
        // Arrange
        var data = new { items = new string[] { } };
        var resolver = new DataResolver();

        // Act
        var result = resolver.ResolveCollection(data, "nonexistent");

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    #endregion

    #region ConditionalNode Evaluation

    [Fact]
    public void EvaluateCondition_SimpleEquality_True()
    {
        // CU-17 | CA-02 — Condición de igualdad verdadera
        // Arrange
        var data = new { status = "activo" };
        var evaluator = new Evaluator();

        // Act
        var result = evaluator.EvaluateCondition("status == 'activo'", data);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void EvaluateCondition_SimpleEquality_False()
    {
        // CU-17 | CA-03 — Condición de igualdad falsa
        // Arrange
        var data = new { status = "inactivo" };
        var evaluator = new Evaluator();

        // Act
        var result = evaluator.EvaluateCondition("status == 'activo'", data);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void EvaluateCondition_NumericComparison_GreaterThan()
    {
        // CU-17 — Comparación numérica (mayor que)
        // Arrange
        var data = new { total = 100 };
        var evaluator = new Evaluator();

        // Act
        var result = evaluator.EvaluateCondition("total > 50", data);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void EvaluateCondition_NumericComparison_LessThan()
    {
        // CU-17 — Comparación numérica (menor que)
        // Arrange
        var data = new { total = 30 };
        var evaluator = new Evaluator();

        // Act
        var result = evaluator.EvaluateCondition("total > 50", data);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void EvaluateCondition_NotEqual()
    {
        // CU-17 — Comparación de desigualdad
        // Arrange
        var data = new { estado = "pendiente" };
        var evaluator = new Evaluator();

        // Act
        var result = evaluator.EvaluateCondition("estado != 'completado'", data);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void EvaluateCondition_AndLogic()
    {
        // CU-17 — Lógica AND
        // Arrange
        var data = new { status = "activo", cantidad = 10 };
        var evaluator = new Evaluator();

        // Act
        var result = evaluator.EvaluateCondition("status == 'activo' && cantidad > 5", data);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void EvaluateCondition_OrLogic()
    {
        // CU-17 — Lógica OR
        // Arrange
        var data = new { status = "inactivo", cantidad = 100 };
        var evaluator = new Evaluator();

        // Act
        var result = evaluator.EvaluateCondition("status == 'activo' || cantidad > 50", data);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void EvaluateCondition_InvalidExpression_ThrowsArgumentException()
    {
        // CU-17 | FA-02 — Expresión inválida lanza excepción
        // Arrange
        var data = new { status = "activo" };
        var evaluator = new Evaluator();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => 
            evaluator.EvaluateCondition("status === malformed", data));
    }

    #endregion

    #region Evaluate ConditionalNode

    [Fact]
    public void Evaluate_ConditionalNodeTrue_IncludesTrueBranch()
    {
        // CU-17 | CA-02 — Nodo condicional verdadero incluye rama true
        // Arrange
        var trueBranch = new TextNode("Activo");
        var falseBranch = new TextNode("Inactivo");
        var conditionalNode = new ConditionalNode("status == 'activo'", trueBranch, falseBranch);

        var data = new { status = "activo" };
        var evaluator = new Evaluator();

        // Act
        var result = evaluator.Evaluate(conditionalNode, data);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Root);
        // After evaluation, conditional should be replaced with its true branch
        Assert.IsType<TextNode>(result.Root);
        Assert.Equal("Activo", ((TextNode)result.Root).Text);
    }

    [Fact]
    public void Evaluate_ConditionalNodeFalse_IncludesFalseBranch()
    {
        // CU-17 | CA-03 — Nodo condicional falso incluye rama false
        // Arrange
        var trueBranch = new TextNode("Activo");
        var falseBranch = new TextNode("Inactivo");
        var conditionalNode = new ConditionalNode("status == 'activo'", trueBranch, falseBranch);

        var data = new { status = "inactivo" };
        var evaluator = new Evaluator();

        // Act
        var result = evaluator.Evaluate(conditionalNode, data);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Root);
        Assert.IsType<TextNode>(result.Root);
        Assert.Equal("Inactivo", ((TextNode)result.Root).Text);
    }

    [Fact]
    public void Evaluate_ConditionalNodeNoFalseBranch_ExcludesNodeWhenFalse()
    {
        // CU-17 — Nodo condicional sin rama false excluye cuando es falso
        // Arrange
        var trueBranch = new TextNode("Incluido");
        var conditionalNode = new ConditionalNode("show == true", trueBranch, null);

        var data = new { show = false };
        var evaluator = new Evaluator();

        // Act
        var result = evaluator.Evaluate(conditionalNode, data);

        // Assert
        Assert.NotNull(result);
        // Should be empty or null when condition fails and no false branch
        Assert.Null(result.Root);
    }

    #endregion

    #region Evaluate LoopNode

    [Fact]
    public void Evaluate_LoopNodeSimple_ExpandsItems()
    {
        // CU-16 | CA-01 — Loop simple expande items correctamente
        // Arrange
        var loopBody = new TextNode("{{item}}");
        var loopNode = new LoopNode("items", "item", loopBody);

        var data = new { items = new[] { "A", "B", "C" } };
        var evaluator = new Evaluator();

        // Act
        var result = evaluator.Evaluate(loopNode, data);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Root);
        // Loop should produce a container with 3 children (one for each item)
        Assert.IsType<ContainerNode>(result.Root);
        var container = (ContainerNode)result.Root;
        Assert.Equal(3, container.Children?.Count ?? 0);
    }

    [Fact]
    public void Evaluate_LoopNodeNested_PreservesOrder()
    {
        // CU-16 | CA-02 — Loop preserva orden de items
        // Arrange
        var loopBody = new TextNode("Item: {{item}}");
        var loopNode = new LoopNode("items", "item", loopBody);

        var data = new { items = new[] { "primero", "segundo", "tercero" } };
        var evaluator = new Evaluator();

        // Act
        var result = evaluator.Evaluate(loopNode, data);

        // Assert
        Assert.NotNull(result);
        var container = (ContainerNode)result.Root!;
        Assert.Equal(3, container.Children?.Count);
        // Each item should have resolved its variable
        Assert.IsType<TextNode>(container.Children?[0]);
    }

    [Fact]
    public void Evaluate_LoopNodeEmpty_ProducesEmptyContainer()
    {
        // CU-16 | CA-03 — Loop vacío produce contenedor vacío
        // Arrange
        var loopBody = new TextNode("{{item}}");
        var loopNode = new LoopNode("items", "item", loopBody);

        var data = new { items = new string[] { } };
        var evaluator = new Evaluator();

        // Act
        var result = evaluator.Evaluate(loopNode, data);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<ContainerNode>(result.Root);
        var container = (ContainerNode)result.Root;
        Assert.Empty(container.Children ?? new List<DocumentNode>());
    }

    [Fact]
    public void Evaluate_LoopNodeComplexObjects_ResolvesNestedProperties()
    {
        // CU-16 — Loop sobre objetos complejos resuelve propiedades anidadas
        // Arrange
        var loopBody = new TextNode("{{item.nombre}}: {{item.precio}}");
        var loopNode = new LoopNode("ordenes", "item", loopBody);

        var data = new
        {
            ordenes = new[]
            {
                new { nombre = "Producto A", precio = 100 },
                new { nombre = "Producto B", precio = 200 }
            }
        };
        var evaluator = new Evaluator();

        // Act
        var result = evaluator.Evaluate(loopNode, data);

        // Assert
        Assert.NotNull(result);
        var container = (ContainerNode)result.Root!;
        Assert.Equal(2, container.Children?.Count);
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void Evaluate_TextNodeWithVariableBinding_ResolvesVariable()
    {
        // CU-02 | CU-15 — TextNode resuelve variable
        // Arrange
        var textNode = new TextNode("Hola {{nombre}}", "nombre");
        var data = new { nombre = "Juan" };
        IDataResolver dataResolver = new DataResolver();
        var evaluator = new Evaluator(dataResolver);

        // Act
        var result = evaluator.Evaluate(textNode, data);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Root);
        Assert.IsType<TextNode>(result.Root);
        // The resolved text should have the variable replaced
        var resolvedText = ((TextNode)result.Root).Text;
        Assert.Contains("Juan", resolvedText);
    }

    [Fact]
    public void Evaluate_DSLToParserToEvaluator_CompleteIntegration()
    {
        // CU-02 | CU-15 | CU-16 | CU-17 — Integración completa: DSL → Parser → Evaluator
        // Arrange
        string dsl = @"{
            ""id"": ""test-doc"",
            ""version"": ""1.0"",
            ""root"": {
                ""type"": ""container"",
                ""layout"": ""vertical"",
                ""children"": [
                    {
                        ""type"": ""text"",
                        ""text"": ""Hola {{nombre}}""
                    },
                    {
                        ""type"": ""conditional"",
                        ""expression"": ""cantidad > 0"",
                        ""trueBranch"": {
                            ""type"": ""text"",
                            ""text"": ""Hay items""
                        }
                    },
                    {
                        ""type"": ""loop"",
                        ""source"": ""items"",
                        ""itemAlias"": ""item"",
                        ""body"": {
                            ""type"": ""text"",
                            ""text"": ""Item: {{item}}""
                        }
                    }
                ]
            }
        }";

        var data = new
        {
            nombre = "Carlos",
            cantidad = 5,
            items = new[] { "A", "B", "C" }
        };

        var parser = new DslParser();
        IDataResolver dataResolver = new DataResolver();
        var evaluator = new Evaluator(dataResolver);

        // Act
        var template = parser.Parse(dsl);
        var evaluated = evaluator.Evaluate(template.Root!, data);

        // Assert
        Assert.NotNull(evaluated);
        Assert.NotNull(evaluated.Root);
        Assert.IsType<ContainerNode>(evaluated.Root);
        var container = (ContainerNode)evaluated.Root;
        Assert.NotNull(container.Children);
        // Should have 3 children after evaluation
        Assert.True(container.Children.Count >= 2); // At least text, conditional, and loop expanded
    }

    [Fact]
    public void Evaluate_EvaluatedDocumentMetadata_ContainsOriginalTemplateInfo()
    {
        // CU-02 — EvaluatedDocument contiene metadatos de plantilla original
        // Arrange
        var textNode = new TextNode("test");
        var template = new DocumentTemplate("doc-id", "1.0", textNode);

        var data = new { };
        var evaluator = new Evaluator();

        // Act
        // Use EvaluateTemplate method to preserve template metadata
        var result = evaluator.EvaluateTemplate(template, data);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("doc-id", result.Id);
        Assert.Equal("1.0", result.Version);
    }

    [Fact]
    public void Evaluate_WithVariableResolutionWarnings_PopulatesWarnings()
    {
        // CU-02 | FA-01 — Dato inexistente genera warning
        // Arrange
        var textNode = new TextNode("Hola {{apellido}}");
        var data = new { nombre = "Juan" };
        var evaluator = new Evaluator();

        // Act
        var result = evaluator.Evaluate(textNode, data);

        // Assert
        Assert.NotNull(result);
        // Should add warning for unresolved variable
        Assert.NotEmpty(result.Warnings);
        Assert.Contains("apellido", result.Warnings[0]);
    }

    #endregion
}
