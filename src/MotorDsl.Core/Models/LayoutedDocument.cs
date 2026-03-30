namespace MotorDsl.Core.Models;

/// <summary>
/// Representa el documento después de que se ha aplicado el cálculo de layout.
/// Es la representación intermedia entre el Evaluator y el Renderer.
/// </summary>
public class LayoutedDocument
{
    public DocumentNode? Root { get; set; }
    public Dictionary<string, LayoutInfo> NodeLayoutInfo { get; set; } = new();
    public int TotalWidth { get; set; }
    public int TotalHeight { get; set; }
    public DeviceProfile? DeviceProfile { get; set; }
    public Dictionary<string, object> LayoutMetadata { get; set; } = new();
    public List<string> Warnings { get; } = new();
}
