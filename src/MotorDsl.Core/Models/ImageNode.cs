namespace MotorDsl.Core.Models;

/// <summary>
/// Represents an image node for including images or QR codes in documents.
/// 
/// Source: modelo-datos-logico_v1.0.md (Section 3 - Tipos de nodos derivados)
/// Architects: QR support mentioned in vision-producto_v1.0.md
/// </summary>
public class ImageNode : DocumentNode
{
    /// <summary>
    /// Gets or sets the image source (URL, file path, or base64 encoded data).
    /// </summary>
    public string Source { get; set; }

    /// <summary>
    /// Gets or sets the image width in pixels or device units.
    /// </summary>
    public int? Width { get; set; }

    /// <summary>
    /// Gets or sets the image height in pixels or device units.
    /// </summary>
    public int? Height { get; set; }

    /// <summary>
    /// Gets or sets the image type (e.g., "png", "jpeg", "qrcode").
    /// </summary>
    public string? ImageType { get; set; }

    /// <summary>
    /// Constructor for image nodes.
    /// </summary>
    /// <param name="source">Image source (URL, path, or encoded data)</param>
    /// <param name="width">Width in pixels</param>
    /// <param name="height">Height in pixels</param>
    /// <param name="imageType">Image type identifier</param>
    public ImageNode(string source, int? width = null, int? height = null, string? imageType = null)
        : base("image")
    {
        Source = source;
        Width = width;
        Height = height;
        ImageType = imageType;
    }
}
