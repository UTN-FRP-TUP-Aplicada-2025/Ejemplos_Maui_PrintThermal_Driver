namespace MotorDsl.Core.Models;

/// <summary>
/// Abstract base class for all document tree nodes.
/// Represents a node in the Abstract Syntax Tree (AST) of a document.
/// Each node can have children and properties.
/// 
/// Source: modelo-datos-logico_v1.0.md (Section 5)
/// Architecture: arquitectura-solucion_v1.0.md (Section 5.4)
/// </summary>
public abstract class DocumentNode
{
    /// <summary>
    /// Gets the type identifier for this node (e.g., "text", "container", "table", "conditional").
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// Gets or sets the child nodes of this node.
    /// Can be null for leaf nodes.
    /// </summary>
    public List<DocumentNode>? Children { get; set; }

    /// <summary>
    /// Gets or sets dynamic properties as key-value pairs.
    /// Allows extensibility without modifying the class.
    /// </summary>
    public Dictionary<string, object>? Properties { get; set; }

    /// <summary>
    /// Gets or sets style information for this node
    /// (e.g., alignment, spacing, formatting).
    /// </summary>
    public StyleDefinition? Style { get; set; }

    /// <summary>
    /// Constructor for document nodes.
    /// </summary>
    /// <param name="type">Node type identifier</param>
    protected DocumentNode(string type)
    {
        Type = type;
        Children = new List<DocumentNode>();
        Properties = new Dictionary<string, object>();
    }

    /// <summary>
    /// Adds a child node to this node.
    /// </summary>
    public void AddChild(DocumentNode child)
    {
        Children ??= new List<DocumentNode>();
        Children.Add(child);
    }

    /// <summary>
    /// Sets a dynamic property.
    /// </summary>
    public void SetProperty(string key, object value)
    {
        Properties ??= new Dictionary<string, object>();
        Properties[key] = value;
    }

    /// <summary>
    /// Gets a dynamic property.
    /// </summary>
    public object? GetProperty(string key)
    {
        return Properties?.TryGetValue(key, out var value) == true ? value : null;
    }
}

/// <summary>
/// Represents styling information for a document node.
/// Can include alignment, spacing, font properties, etc.
/// </summary>
public class StyleDefinition
{
    /// <summary>
    /// Gets or sets style attributes as key-value pairs.
    /// Examples: "align" -> "center", "color" -> "white", "width" -> "80"
    /// </summary>
    public Dictionary<string, object> Attributes { get; set; } = new();
}
