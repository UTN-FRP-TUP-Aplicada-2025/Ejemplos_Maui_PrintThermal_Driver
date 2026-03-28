using Xunit;
using MotorDsl.Core.Contracts;
using MotorDsl.Core.Models;
using MotorDsl.Parser;

namespace MotorDsl.Tests;

/// <summary>
/// Unit tests for IDslParser implementation.
/// Tests DSL parsing, validation, and AST construction.
/// 
/// Based on:
/// - CU-01: Interpretar Plantilla DSL
/// - CU-13: Cargar Plantilla DSL
/// - CU-14: Validar Plantilla DSL
/// - RN-01: Validación de Esquema DSL
/// - RN-02: Tipos de Elementos Soportados
/// - Casos-prueba-referenciales_v1.0.md
/// </summary>
public class DslParserTests
{
    // Note: Implementation class DslParser will be created in PASO 2
    // Tests use only the IDslParser interface

    #region CP-001: Simple Text Document

    [Fact]
    public void Parse_SimpleTextDocument_ReturnsValidDocumentTemplate()
    {
        // CU-01 | RN-01 — Interpretar plantilla simple con texto
        // Arrange
        string json = @"{
            ""id"": ""doc1"",
            ""version"": ""1.0"",
            ""root"": {
                ""type"": ""text"",
                ""text"": ""Hola mundo""
            }
        }";

        IDslParser parser = new DslParser();

        // Act
        var result = parser.Parse(json);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("doc1", result.Id);
        Assert.Equal("1.0", result.Version);
        Assert.NotNull(result.Root);
        Assert.IsType<TextNode>(result.Root);
        Assert.Equal("Hola mundo", ((TextNode)result.Root).Text);
    }

    #endregion

    #region CP-002: Text with Data Binding

    [Fact]
    public void Parse_TextWithDataBinding_CapturesBindPath()
    {
        // CU-01 | RN-01 — Interpretar plantilla con binding de datos
        // Arrange
        string json = @"{
            ""id"": ""doc2"",
            ""version"": ""1.0"",
            ""root"": {
                ""type"": ""text"",
                ""text"": ""Hola {{nombre}}"",
                ""bindPath"": ""nombre""
            }
        }";

        IDslParser parser = new DslParser();

        // Act
        var result = parser.Parse(json);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<TextNode>(result.Root);
        var textNode = (TextNode)result.Root;
        Assert.Equal("Hola {{nombre}}", textNode.Text);
        Assert.Equal("nombre", textNode.BindPath);
    }

    #endregion

    #region RN-02: Supported Element Types

    [Fact]
    public void Parse_SupportedElementType_AcceptsText()
    {
        // RN-02 — Aceptar tipo de elemento "text"
        // Arrange
        string json = @"{
            ""id"": ""doc3"",
            ""version"": ""1.0"",
            ""root"": {
                ""type"": ""text"",
                ""text"": ""válido""
            }
        }";

        IDslParser parser = new DslParser();

        // Act & Assert
        var result = parser.Parse(json);
        Assert.NotNull(result);
        Assert.Equal("text", result.Root?.Type);
    }

    [Fact]
    public void Parse_SupportedElementType_AcceptsContainer()
    {
        // RN-02 — Aceptar tipo de elemento "container"
        // Arrange
        string json = @"{
            ""id"": ""doc4"",
            ""version"": ""1.0"",
            ""root"": {
                ""type"": ""container"",
                ""layout"": ""vertical"",
                ""children"": []
            }
        }";

        IDslParser parser = new DslParser();

        // Act
        var result = parser.Parse(json);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<ContainerNode>(result.Root);
        Assert.Equal("vertical", ((ContainerNode)result.Root).Layout);
    }

    [Fact]
    public void Parse_UnsupportedElementType_ThrowsArgumentException()
    {
        // RN-02 | CU-14 — Rechazar tipo de elemento no soportado
        // Arrange
        string json = @"{
            ""id"": ""doc5"",
            ""version"": ""1.0"",
            ""root"": {
                ""type"": ""unknowntype"",
                ""text"": ""no válido""
            }
        }";

        IDslParser parser = new DslParser();

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => parser.Parse(json));
        Assert.Contains("unknowntype", ex.Message);
        Assert.Contains("unsupported", ex.Message.ToLower());
    }

    #endregion

    #region RN-01: Schema Validation

    [Fact]
    public void Parse_InvalidJson_ThrowsArgumentException()
    {
        // CU-14 | RN-01 — JSON malformado debe lanzar excepción
        // Arrange
        string json = @"{ invalid json without quotes }";

        IDslParser parser = new DslParser();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => parser.Parse(json));
    }

    [Fact]
    public void Parse_MissingRootNode_ThrowsArgumentException()
    {
        // CU-14 | RN-01 — Estructura sin nodo raíz es inválida
        // Arrange
        string json = @"{
            ""id"": ""doc6"",
            ""version"": ""1.0""
        }";

        IDslParser parser = new DslParser();

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => parser.Parse(json));
        Assert.Contains("root", ex.Message.ToLower());
    }

    [Fact]
    public void Parse_MissingRequiredProperties_ThrowsArgumentException()
    {
        // CU-14 | RN-01 — Propiedades obligatorias deben estar presentes
        // Arrange
        string json = @"{
            ""root"": {
                ""type"": ""text""
            }
        }";

        IDslParser parser = new DslParser();

        // Act & Assert
        var ex = Assert.Throws<ArgumentException>(() => parser.Parse(json));
        Assert.Contains("id", ex.Message.ToLower());
    }

    #endregion

    #region Hierarchical Structure

    [Fact]
    public void Parse_NestedContainerNodes_BuildsCorrectHierarchy()
    {
        // CU-01 | RN-01 — Estructura jerárquica correcta
        // Arrange
        string json = @"{
            ""id"": ""doc7"",
            ""version"": ""1.0"",
            ""root"": {
                ""type"": ""container"",
                ""layout"": ""vertical"",
                ""children"": [
                    {
                        ""type"": ""text"",
                        ""text"": ""Texto 1""
                    },
                    {
                        ""type"": ""text"",
                        ""text"": ""Texto 2""
                    }
                ]
            }
        }";

        IDslParser parser = new DslParser();

        // Act
        var result = parser.Parse(json);

        // Assert
        Assert.NotNull(result.Root);
        Assert.IsType<ContainerNode>(result.Root);
        var container = (ContainerNode)result.Root;
        Assert.NotNull(container.Children);
        Assert.Equal(2, container.Children.Count);
        Assert.All(container.Children, child => Assert.Equal("text", child.Type));
    }

    #endregion

    #region Loop Nodes

    [Fact]
    public void Parse_LoopNode_CreatesLoopNodeWithSourceAndAlias()
    {
        // CP-05 | CU-01 — Iteración sobre colección
        // Arrange
        string json = @"{
            ""id"": ""doc8"",
            ""version"": ""1.0"",
            ""root"": {
                ""type"": ""loop"",
                ""source"": ""items"",
                ""itemAlias"": ""item"",
                ""body"": {
                    ""type"": ""text"",
                    ""text"": ""{{item.nombre}}""
                }
            }
        }";

        IDslParser parser = new DslParser();

        // Act
        var result = parser.Parse(json);

        // Assert
        Assert.NotNull(result.Root);
        Assert.IsType<LoopNode>(result.Root);
        var loopNode = (LoopNode)result.Root;
        Assert.Equal("items", loopNode.Source);
        Assert.Equal("item", loopNode.ItemAlias);
        Assert.NotNull(loopNode.Body);
    }

    #endregion

    #region Conditional Nodes

    [Fact]
    public void Parse_ConditionalNode_CreatesConditionalWithExpression()
    {
        // CP-03 | CU-01 — Nodo condicional verdadero
        // Arrange
        string json = @"{
            ""id"": ""doc9"",
            ""version"": ""1.0"",
            ""root"": {
                ""type"": ""conditional"",
                ""expression"": ""data.estado == 'activo'"",
                ""trueBranch"": {
                    ""type"": ""text"",
                    ""text"": ""Activo""
                }
            }
        }";

        IDslParser parser = new DslParser();

        // Act
        var result = parser.Parse(json);

        // Assert
        Assert.NotNull(result.Root);
        Assert.IsType<ConditionalNode>(result.Root);
        var conditionalNode = (ConditionalNode)result.Root;
        Assert.Equal("data.estado == 'activo'", conditionalNode.Expression);
        Assert.NotNull(conditionalNode.TrueBranch);
    }

    #endregion

    #region Table Nodes

    [Fact]
    public void Parse_TableNode_CreatesTableWithHeadersAndRows()
    {
        // RN-02 — Elemento de tipo "table" soportado
        // Arrange
        string json = @"{
            ""id"": ""doc10"",
            ""version"": ""1.0"",
            ""root"": {
                ""type"": ""table"",
                ""headers"": [""Nombre"", ""Edad""],
                ""rows"": [
                    [""Juan"", ""30""],
                    [""María"", ""25""]
                ]
            }
        }";

        IDslParser parser = new DslParser();

        // Act
        var result = parser.Parse(json);

        // Assert
        Assert.NotNull(result.Root);
        Assert.IsType<TableNode>(result.Root);
        var tableNode = (TableNode)result.Root;
        Assert.Equal(2, tableNode.Headers.Count);
        Assert.Equal(2, tableNode.Rows.Count);
    }

    #endregion

    #region Image Nodes

    [Fact]
    public void Parse_ImageNode_CreatesImageWithSourceAndDimensions()
    {
        // RN-02 — Elemento de tipo "image" soportado
        // Arrange
        string json = @"{
            ""id"": ""doc11"",
            ""version"": ""1.0"",
            ""root"": {
                ""type"": ""image"",
                ""source"": ""https://example.com/logo.png"",
                ""width"": 100,
                ""height"": 100,
                ""imageType"": ""png""
            }
        }";

        IDslParser parser = new DslParser();

        // Act
        var result = parser.Parse(json);

        // Assert
        Assert.NotNull(result.Root);
        Assert.IsType<ImageNode>(result.Root);
        var imageNode = (ImageNode)result.Root;
        Assert.Equal("https://example.com/logo.png", imageNode.Source);
        Assert.Equal(100, imageNode.Width);
        Assert.Equal(100, imageNode.Height);
    }

    #endregion

    #region Empty/Null Checks

    [Fact]
    public void Parse_NullInput_ThrowsArgumentException()
    {
        // CU-14 — Entrada nula es inválida
        // Arrange
        IDslParser parser = new DslParser();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => parser.Parse(null!));
    }

    [Fact]
    public void Parse_EmptyString_ThrowsArgumentException()
    {
        // CU-14 — Cadena vacía es inválida
        // Arrange
        IDslParser parser = new DslParser();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => parser.Parse(""));
    }

    [Fact]
    public void Parse_WhitespaceOnlyString_ThrowsArgumentException()
    {
        // CU-14 — Solo espacios en blanco es inválido
        // Arrange
        IDslParser parser = new DslParser();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => parser.Parse("   "));
    }

    #endregion

    #region DeviceProfile Tests

    [Fact]
    public void Parse_WithDeviceProfileMetadata_StoresInMetadata()
    {
        // CU-01 — Metadatos adicionales en plantilla
        // Arrange
        string json = @"{
            ""id"": ""doc12"",
            ""version"": ""1.0"",
            ""metadata"": {
                ""targetDevice"": ""thermal_80mm"",
                ""author"": ""test""
            },
            ""root"": {
                ""type"": ""text"",
                ""text"": ""documento""
            }
        }";

        IDslParser parser = new DslParser();

        // Act
        var result = parser.Parse(json);

        // Assert
        Assert.NotNull(result.Metadata);
        Assert.True(result.Metadata.ContainsKey("targetDevice"));
        Assert.Equal("thermal_80mm", result.Metadata["targetDevice"]);
    }

    #endregion

    #region Style Parsing

    [Fact]
    public void Parse_TextNodeWithStyle_CapturesAlignment()
    {
        // Arrange
        string json = @"{
            ""id"": ""style-1"",
            ""version"": ""1.0"",
            ""root"": {
                ""type"": ""text"",
                ""text"": ""Centrado"",
                ""style"": { ""align"": ""center"" }
            }
        }";

        IDslParser parser = new DslParser();

        // Act
        var result = parser.Parse(json);

        // Assert
        var textNode = Assert.IsType<TextNode>(result.Root);
        Assert.NotNull(textNode.Style);
        Assert.True(textNode.Style.Attributes.ContainsKey("align"));
        Assert.Equal("center", textNode.Style.Attributes["align"]);
    }

    [Fact]
    public void Parse_TextNodeWithStyle_CapturesBold()
    {
        // Arrange
        string json = @"{
            ""id"": ""style-2"",
            ""version"": ""1.0"",
            ""root"": {
                ""type"": ""text"",
                ""text"": ""Negrita"",
                ""style"": { ""bold"": true }
            }
        }";

        IDslParser parser = new DslParser();

        // Act
        var result = parser.Parse(json);

        // Assert
        var textNode = Assert.IsType<TextNode>(result.Root);
        Assert.NotNull(textNode.Style);
        Assert.True(textNode.Style.Attributes.ContainsKey("bold"));
        Assert.Equal(true, textNode.Style.Attributes["bold"]);
    }

    #endregion
}
