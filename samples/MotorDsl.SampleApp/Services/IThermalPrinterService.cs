namespace MotorDsl.SampleApp.Services;

/// <summary>
/// Interface simplificada para el servicio de impresión térmica por Bluetooth.
/// Solo transporta bytes — el formateo lo hace el motor DSL.
/// </summary>
public interface IThermalPrinterService
{
    bool IsConnected { get; }

    Task<List<BluetoothDevice>> ScanDevicesAsync();

    Task<bool> ConnectAsync(string deviceAddress);

    Task DisconnectAsync();

    /// <summary>
    /// Envía bytes crudos (ESC/POS) a la impresora conectada.
    /// </summary>
    Task SendBytesAsync(byte[] data, PrinterProfile? profile = null);
}

/// <summary>
/// Representa un dispositivo Bluetooth emparejado.
/// </summary>
public class BluetoothDevice
{
    public string Name { get; set; } = "";
    public string Address { get; set; } = "";
    public bool IsPaired { get; set; }

    public override string ToString() => $"{Name} ({Address})";
}
