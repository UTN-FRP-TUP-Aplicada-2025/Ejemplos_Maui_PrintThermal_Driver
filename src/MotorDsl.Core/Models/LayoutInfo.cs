namespace MotorDsl.Core.Models;

/// <summary>
/// Información de layout calculada para un nodo individual del documento.
/// Almacena posición, dimensiones y resultado de wrapping/alineación.
/// </summary>
public class LayoutInfo
{
    public int LineNumber { get; set; }
    public int ColumnNumber { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public string Alignment { get; set; } = "left";
    public bool IsWrapped { get; set; }
    public string WrappedText { get; set; } = "";
    public bool IsTruncated { get; set; }
    public Dictionary<string, object> DeviceMetadata { get; set; } = new();
}
