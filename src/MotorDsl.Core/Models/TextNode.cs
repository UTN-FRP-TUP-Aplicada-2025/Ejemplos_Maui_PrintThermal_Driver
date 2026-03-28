namespace MotorDsl.Core.Models;

/// <summary>
/// Represents a text content node in the document tree.
/// Contains plain text or data-bound text content.
/// 
/// Source: modelo-datos-logico_v1.0.md (Section 6)
/// </summary>
public class TextNode : DocumentNode
{
    /// <summary>
    /// Gets or sets the text content.
    /// Can be plain text or contain data binding expressions.
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    /// Gets or sets the path to data for dynamic binding.
    /// If null, the node contains static text.
    /// Example: "customer.name", "items[0].description"
    /// </summary>
    public string? BindPath { get; set; }

    /// <summary>
    /// Constructor for text nodes.
    /// </summary>
    /// <param name="text">Text content</param>
    /// <param name="bindPath">Optional data binding path</param>
    public TextNode(string text = "", string? bindPath = null) : base("text")
    {
        Text = text;
        BindPath = bindPath;
    }
}
