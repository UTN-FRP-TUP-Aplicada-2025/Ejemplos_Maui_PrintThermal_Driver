namespace MotorDsl.Core.Models;

public class DeviceProfile
{
    public string Name { get; }
    public int Width { get; }
    public string RenderTarget { get; }
    // Nuevas propiedades opcionales (backward compatible)
    public int? CodePage { get; set; } = null;
    public byte[]? CodePageCommand { get; set; } = null;
    public int? BaudRate { get; set; } = null;
    public List<string>? SupportedBarcodes { get; set; } = null;

    private readonly Dictionary<string, object> _capabilities = new();

    public DeviceProfile(string name, int width, string renderTarget)
    {
        Name = name;
        Width = width;
        RenderTarget = renderTarget;
    }

    public void SetCapability(string key, object value)
        => _capabilities[key] = value;

    public object? GetCapability(string key)
        => _capabilities.TryGetValue(key, out var val) ? val : null;

    public bool HasCapability(string key)
        => _capabilities.ContainsKey(key);
}
