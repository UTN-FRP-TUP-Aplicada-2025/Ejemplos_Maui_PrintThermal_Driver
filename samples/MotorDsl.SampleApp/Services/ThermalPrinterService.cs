#if ANDROID
using Android.Bluetooth;
using Android.Content;
using Android.Runtime;
using Java.Util;
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

        await _outputStream.WriteAsync(data, 0, data.Length);
        await _outputStream.FlushAsync();
#else
        await Task.CompletedTask;
#endif
    }
}
