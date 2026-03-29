#if ANDROID
using Android.Bluetooth;
using Android.Content;
using Android.Runtime;
using Java.Util;
using Microsoft.Maui.ApplicationModel;
using AndroidBluetoothDevice = Android.Bluetooth.BluetoothDevice;
#endif

namespace MotorDsl.SampleApp.Services;

/// <summary>
/// Implementación del servicio de impresión térmica para Android.
/// Solo transporta bytes crudos — el formateo ESC/POS lo genera el motor DSL.
/// </summary>
public class ThermalPrinterService : IThermalPrinterService
{
#if ANDROID
    private BluetoothSocket? _socket;
    private BluetoothAdapter? _bluetoothAdapter;
    private System.IO.Stream? _outputStream;
#endif

    public bool IsConnected { get; private set; }

    public ThermalPrinterService()
    {
#if ANDROID
        _bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
#endif
    }

    public Task<List<BluetoothDevice>> ScanDevicesAsync()
    {
        var devices = new List<BluetoothDevice>();

#if ANDROID
        if (_bluetoothAdapter == null)
            throw new Exception("Bluetooth no está disponible en este dispositivo");

        if (!_bluetoothAdapter.IsEnabled)
            throw new Exception("Bluetooth está desactivado. Por favor actívalo.");

        // En Android 12+ se requiere BLUETOOTH_CONNECT para acceder a BondedDevices.
        if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.S)
        {
            var activity = Platform.CurrentActivity!;
            var permStatus = AndroidX.Core.Content.ContextCompat.CheckSelfPermission(activity,
                    Android.Manifest.Permission.BluetoothConnect);
            if (permStatus != (int)Android.Content.PM.Permission.Granted)
            {
                throw new Exception(
                    "Permiso BLUETOOTH_CONNECT no otorgado. Reiniciá la app y aceptá los permisos.");
            }
        }

        try
        {
            var bondedDevices = _bluetoothAdapter.BondedDevices;

            if (bondedDevices != null && bondedDevices.Count > 0)
            {
                foreach (AndroidBluetoothDevice device in bondedDevices)
                {
                    devices.Add(new BluetoothDevice
                    {
                        Name = device.Name ?? "Dispositivo desconocido",
                        Address = device.Address ?? "",
                        IsPaired = true
                    });
                }
            }
        }
        catch (Java.Lang.SecurityException)
        {
            return Task.FromResult(new List<BluetoothDevice>());
        }
#endif
        return Task.FromResult(devices);
    }

    public async Task<bool> ConnectAsync(string deviceAddress)
    {
#if ANDROID
        try
        {
            if (_socket != null && _socket.IsConnected)
                await DisconnectAsync();

            var device = _bluetoothAdapter!.GetRemoteDevice(deviceAddress)
                ?? throw new Exception("No se pudo encontrar el dispositivo");

            // UUID estándar para SPP (Serial Port Profile)
            var uuid = UUID.FromString("00001101-0000-1000-8000-00805F9B34FB")!;
            _socket = device.CreateRfcommSocketToServiceRecord(uuid)!;

            await Task.Run(() => _socket.Connect());
            _outputStream = _socket.OutputStream;

            IsConnected = true;
            return true;
        }
        catch (Exception ex)
        {
            IsConnected = false;
            throw new Exception($"Error al conectar: {ex.Message}", ex);
        }
#else
        await Task.CompletedTask;
        return false;
#endif
    }

    public async Task DisconnectAsync()
    {
#if ANDROID
        try
        {
            if (_outputStream != null)
            {
                await _outputStream.FlushAsync();
                _outputStream.Dispose();
                _outputStream = null;
            }

            if (_socket != null)
            {
                _socket.Close();
                _socket.Dispose();
                _socket = null;
            }

            IsConnected = false;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al desconectar: {ex.Message}", ex);
        }
#else
        await Task.CompletedTask;
#endif
    }

    public async Task SendBytesAsync(byte[] data)
    {
#if ANDROID
        if (_outputStream == null)
            throw new InvalidOperationException("No hay una impresora conectada");

        // Dar tiempo a la impresora para estar lista
        await Task.Delay(100);

        var lines = SplitByLineFeed(data);

        foreach (var line in lines)
        {
            await _outputStream.WriteAsync(line, 0, line.Length);
            await _outputStream.FlushAsync();
            int delayMs = GetDelayForLine(line);
            await Task.Delay(delayMs);
        }

        // Delay final para corte de papel
        await Task.Delay(500);
#else
        await Task.CompletedTask;
#endif
    }

    private static int GetDelayForLine(byte[] line)
    {
        if (line.Length == 0) return 20;
        if (line.Length >= 2 && line[0] == 0x1D && line[1] == 0x28) return 300;
        if (line.Length >= 2 && line[0] == 0x1D && line[1] == 0x76) return 500;
        if (line.Length >= 2 && line[0] == 0x1D && line[1] == 0x56) return 500;
        if (line.Length >= 2 && line[0] == 0x1B && line[1] == 0x40) return 300;

        // Texto normal: 150ms + 5ms por byte
        return 150 + (line.Length * 5);
    }

    private static List<byte[]> SplitByLineFeed(byte[] data)
    {
        var lines = new List<byte[]>();
        var current = new List<byte>();

        foreach (byte b in data)
        {
            current.Add(b);
            if (b == 0x0A) // LF
            {
                lines.Add(current.ToArray());
                current.Clear();
            }
        }

        if (current.Count > 0)
            lines.Add(current.ToArray());

        return lines;
    }
}
