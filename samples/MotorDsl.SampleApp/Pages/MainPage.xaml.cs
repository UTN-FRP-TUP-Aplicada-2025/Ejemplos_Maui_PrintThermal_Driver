using MotorDsl.Core.Contracts;
using MotorDsl.Core.Models;
using MotorDsl.SampleApp.Services;
using MotorDsl.SampleApp.Templates;

#if ANDROID
using Android;
using AndroidX.Core.App;
using AndroidX.Core.Content;
using Microsoft.Maui.ApplicationModel;
#endif

namespace MotorDsl.SampleApp.Pages;

public partial class MainPage : ContentPage
{
    private readonly IDocumentEngine _engine;
    private readonly IThermalPrinterService _printerService;
    private List<BluetoothDevice> _devices = new();

    public MainPage(IDocumentEngine engine, IThermalPrinterService printerService)
    {
        InitializeComponent();
        _engine = engine;
        _printerService = printerService;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

#if ANDROID
        await RequestBluetoothPermissions();
#endif
    }

#if ANDROID
    private async Task<bool> RequestBluetoothPermissions()
    {
        try
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            if (status != PermissionStatus.Granted)
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

            if (status != PermissionStatus.Granted)
            {
                await DisplayAlertAsync("Permisos Requeridos",
                    "Se necesitan permisos de ubicación para escanear dispositivos Bluetooth.", "OK");
                return false;
            }

            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.S)
            {
                var activity = Platform.CurrentActivity;
                string[] permissions = new[]
                {
                    Manifest.Permission.BluetoothScan,
                    Manifest.Permission.BluetoothConnect
                };

                foreach (var permission in permissions)
                {
                    if (ContextCompat.CheckSelfPermission(activity!, permission) != (int)Android.Content.PM.Permission.Granted)
                    {
                        ActivityCompat.RequestPermissions(activity!, permissions, 1);
                        break;
                    }
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", $"Error al solicitar permisos: {ex.Message}", "OK");
            return false;
        }
    }
#endif

    // ─── BT: Scan ───

    private async void OnScanClicked(object sender, EventArgs e)
    {
        try
        {
            ShowMessage("Escaneando dispositivos...");
            ScanButton.IsEnabled = false;

            _devices = await _printerService.ScanDevicesAsync();

            if (_devices.Any())
            {
                DevicePicker.ItemsSource = _devices.Select(d => d.ToString()).ToList();
                DevicePicker.IsEnabled = true;
                ConnectButton.IsEnabled = true;
                ShowMessage($"Se encontraron {_devices.Count} dispositivo(s) emparejado(s)");
            }
            else
            {
                ShowMessage("No se encontraron dispositivos Bluetooth emparejados");
                await DisplayAlertAsync("Información",
                    "No se encontraron dispositivos Bluetooth emparejados. " +
                    "Por favor, empareja tu impresora en la configuración de Bluetooth primero.", "OK");
            }
        }
        catch (Exception ex)
        {
            ShowMessage("Error al escanear");
            await DisplayAlertAsync("Error", $"Error al escanear dispositivos: {ex.Message}", "OK");
        }
        finally
        {
            ScanButton.IsEnabled = true;
        }
    }

    // ─── BT: Connect ───

    private async void OnConnectClicked(object sender, EventArgs e)
    {
        try
        {
            if (DevicePicker.SelectedIndex < 0)
            {
                await DisplayAlertAsync("Información", "Por favor, selecciona un dispositivo", "OK");
                return;
            }

            ShowMessage("Conectando...");
            ConnectButton.IsEnabled = false;

            var selectedDevice = _devices[DevicePicker.SelectedIndex];
            bool success = await _printerService.ConnectAsync(selectedDevice.Address);

            if (success)
            {
                UpdateConnectionStatus(true);
                ShowMessage($"Conectado a {selectedDevice.Name}");
            }
            else
            {
                ShowMessage("Error al conectar");
                await DisplayAlertAsync("Error", "No se pudo establecer la conexión", "OK");
            }
        }
        catch (Exception ex)
        {
            ShowMessage("Error de conexión");
            await DisplayAlertAsync("Error", $"Error al conectar: {ex.Message}", "OK");
        }
        finally
        {
            ConnectButton.IsEnabled = !_printerService.IsConnected;
        }
    }

    // ─── BT: Disconnect ───

    private async void OnDisconnectClicked(object sender, EventArgs e)
    {
        try
        {
            ShowMessage("Desconectando...");
            await _printerService.DisconnectAsync();
            UpdateConnectionStatus(false);
            ShowMessage("Desconectado");
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", $"Error al desconectar: {ex.Message}", "OK");
        }
    }

    // ─── Motor DSL: Preview (texto plano) ───

    private void OnPreviewClicked(object sender, EventArgs e)
    {
        try
        {
            ShowMessage("Generando vista previa...");

            var profile = new DeviceProfile("thermal_58mm", 32, "text");
            var result = _engine.Render(TicketDsl.Template, TicketDsl.GetSampleData(), profile);

            if (result.IsSuccessful)
            {
                OutputLabel.Text = result.Output?.ToString() ?? "(vacío)";
                ShowMessage("Vista previa generada");
            }
            else
            {
                OutputLabel.Text = "ERRORES:\n" + string.Join("\n", result.Errors);
                ShowMessage("Error al generar vista previa");
            }
        }
        catch (Exception ex)
        {
            ShowMessage("Error");
            OutputLabel.Text = $"Excepción: {ex.Message}";
        }
    }

    // ─── Motor DSL: Hex dump (ESC/POS) ───

    private void OnHexDumpClicked(object sender, EventArgs e)
    {
        try
        {
            ShowMessage("Generando ESC/POS...");

            var profile = new DeviceProfile("thermal_58mm", 32, "escpos");
            var result = _engine.Render(TicketDsl.Template, TicketDsl.GetSampleData(), profile);

            if (result.IsSuccessful && result.Output is byte[] bytes)
            {
                // Mostrar hex dump legible
                var hex = BitConverter.ToString(bytes).Replace("-", " ");
                var lines = new List<string>();
                for (int i = 0; i < hex.Length; i += 48) // 16 bytes = 48 chars con espacios
                {
                    lines.Add(hex.Substring(i, Math.Min(48, hex.Length - i)));
                }

                OutputLabel.Text = $"ESC/POS ({bytes.Length} bytes):\n\n{string.Join("\n", lines)}";
                ShowMessage($"ESC/POS generado: {bytes.Length} bytes");
            }
            else
            {
                OutputLabel.Text = "ERRORES:\n" + string.Join("\n", result.Errors);
                ShowMessage("Error al generar ESC/POS");
            }
        }
        catch (Exception ex)
        {
            ShowMessage("Error");
            OutputLabel.Text = $"Excepción: {ex.Message}";
        }
    }

    // ─── Motor DSL: Imprimir por BT ───

    private async void OnPrintClicked(object sender, EventArgs e)
    {
        try
        {
            ShowMessage("Imprimiendo ticket...");
            PrintButton.IsEnabled = false;

            var profile = new DeviceProfile("thermal_58mm", 32, "escpos");
            var result = _engine.Render(TicketDsl.Template, TicketDsl.GetSampleData(), profile);

            if (result.IsSuccessful && result.Output is byte[] bytes)
            {
                await _printerService.SendBytesAsync(bytes);
                ShowMessage($"Ticket impreso ({bytes.Length} bytes enviados)");
            }
            else
            {
                ShowMessage("Error al generar ticket");
                await DisplayAlertAsync("Error", "No se pudo generar el ticket ESC/POS", "OK");
            }
        }
        catch (Exception ex)
        {
            ShowMessage("Error al imprimir");
            await DisplayAlertAsync("Error", $"Error al imprimir: {ex.Message}", "OK");
        }
        finally
        {
            PrintButton.IsEnabled = _printerService.IsConnected;
        }
    }

    // ─── Helpers ───

    private void UpdateConnectionStatus(bool connected)
    {
        if (connected)
        {
            StatusLabel.Text = "Conectado";
            StatusLabel.TextColor = Colors.Green;
            DisconnectButton.IsEnabled = true;
            PrintButton.IsEnabled = true;
            ConnectButton.IsEnabled = false;
        }
        else
        {
            StatusLabel.Text = "Desconectado";
            StatusLabel.TextColor = Colors.Red;
            DisconnectButton.IsEnabled = false;
            PrintButton.IsEnabled = false;
            ConnectButton.IsEnabled = DevicePicker.SelectedIndex >= 0;
        }
    }

    private void ShowMessage(string message)
    {
        MessageLabel.Text = $"{DateTime.Now:HH:mm:ss} - {message}";
    }
}
