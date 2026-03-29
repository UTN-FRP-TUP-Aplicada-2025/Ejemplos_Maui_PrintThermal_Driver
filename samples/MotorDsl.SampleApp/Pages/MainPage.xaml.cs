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
            // Android 12+ (API 31+): pedir BLUETOOTH_SCAN y BLUETOOTH_CONNECT
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.S)
            {
                var activity = Platform.CurrentActivity!;
                string[] btPermissions = new[]
                {
                    Manifest.Permission.BluetoothScan,
                    Manifest.Permission.BluetoothConnect
                };

                bool allGranted = btPermissions.All(p =>
                    ContextCompat.CheckSelfPermission(activity, p) == (int)Android.Content.PM.Permission.Granted);

                if (!allGranted)
                {
                    var tcs = new TaskCompletionSource<bool>();

                    // Usar Permissions.RequestAsync con permisos custom no es posible,
                    // así que pedimos via ActivityCompat y esperamos con un delay prudente
                    ActivityCompat.RequestPermissions(activity, btPermissions, 1);

                    // Esperar a que el usuario responda y re-verificar
                    await Task.Delay(3000);

                    allGranted = btPermissions.All(p =>
                        ContextCompat.CheckSelfPermission(activity, p) == (int)Android.Content.PM.Permission.Granted);
                }

                if (!allGranted)
                {
                    await DisplayAlertAsync("Permisos Requeridos",
                        "Se necesitan permisos de Bluetooth para escanear e imprimir.", "OK");
                    return false;
                }
            }
            else
            {
                // Android < 12: necesita permiso de ubicación para BT scan
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

                if (status != PermissionStatus.Granted)
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

                if (status != PermissionStatus.Granted)
                {
                    await DisplayAlertAsync("Permisos Requeridos",
                        "Se necesitan permisos de ubicación para escanear dispositivos Bluetooth.", "OK");
                    return false;
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
            Console.WriteLine("[UI] OnScanClicked iniciado");
            ShowMessage("Escaneando dispositivos...");
            ScanButton.IsEnabled = false;

#if ANDROID
            if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.S)
            {
                var connectStatus = await Permissions.CheckStatusAsync<Permissions.Bluetooth>();
                Console.WriteLine($"[UI] Permission check result: {connectStatus}");
                if (connectStatus != PermissionStatus.Granted)
                {
                    connectStatus = await Permissions.RequestAsync<Permissions.Bluetooth>();
                    Console.WriteLine($"[UI] Permission request result: {connectStatus}");
                    if (connectStatus != PermissionStatus.Granted)
                    {
                        ShowMessage("Se necesitan permisos de Bluetooth");
                        ScanButton.IsEnabled = true;
                        return;
                    }
                }
            }
#endif

            _devices = await _printerService.ScanDevicesAsync();
            Console.WriteLine($"[UI] Devices count: {_devices?.Count}");

            Console.WriteLine("[UI] Entrando a MainThread.InvokeOnMainThreadAsync");
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                Console.WriteLine($"[UI] Dentro de MainThread - devices: {_devices?.Count}");
                if (_devices.Any())
                {
                    DeviceList.ItemsSource = null;
                    DeviceList.ItemsSource = _devices.Select(d => d.ToString()).ToList();
                    DeviceList.IsVisible = true;
                    Console.WriteLine("[UI] DeviceList.ItemsSource seteado");
                    ShowMessage($"Se encontraron {_devices.Count} dispositivo(s) emparejado(s)");
                }
                else
                {
                    ShowMessage("No se encontraron dispositivos emparejados");
                }
            });
            Console.WriteLine("[UI] MainThread.InvokeOnMainThreadAsync completado");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[UI] EXCEPTION: {ex}");
            ShowMessage("Error al escanear");
            await DisplayAlertAsync("Error", $"Error al escanear dispositivos: {ex.Message}", "OK");
        }
        finally
        {
            ScanButton.IsEnabled = true;
        }
    }

    private void OnDeviceSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is string selected)
        {
            var idx = _devices.FindIndex(d => d.ToString() == selected);
            if (idx >= 0) ConnectButton.IsEnabled = true;
        }
    }

    // ─── BT: Connect ───

    private async void OnConnectClicked(object sender, EventArgs e)
    {
        try
        {
            var selected = DeviceList.SelectedItem as string;
            var selectedDevice = _devices.FirstOrDefault(d => d.ToString() == selected);
            if (selectedDevice == null)
            {
                await DisplayAlertAsync("Información", "Por favor, selecciona un dispositivo", "OK");
                return;
            }

            ShowMessage("Conectando...");
            ConnectButton.IsEnabled = false;
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
            Console.WriteLine("[PREVIEW] OnPreviewClicked iniciado");
            ShowMessage("Generando vista previa...");

            var profile = new DeviceProfile("thermal_58mm", 32, "text");
            Console.WriteLine("[PREVIEW] Profile creado, llamando Render...");
            var result = _engine.Render(TicketDsl.Template, TicketDsl.GetSampleData(), profile);
            Console.WriteLine($"[PREVIEW] Render completado. IsSuccessful={result.IsSuccessful}");

            if (result.IsSuccessful)
            {
                OutputLabel.Text = result.Output?.ToString() ?? "(vacío)";
                ShowMessage("Vista previa generada");
            }
            else
            {
                var errors = string.Join("\n", result.Errors);
                Console.WriteLine($"[PREVIEW] ERRORS: {errors}");
                OutputLabel.Text = "ERRORES:\n" + errors;
                ShowMessage("Error al generar vista previa");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[PREVIEW] EXCEPTION: {ex}");
            ShowMessage("Error");
            OutputLabel.Text = $"Excepción: {ex.Message}\n\nStackTrace:\n{ex.StackTrace}\n\nInner: {ex.InnerException?.Message}\n{ex.InnerException?.StackTrace}";
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
                await _printerService.SendBytesAsync(bytes, PrinterProfile.Thermal58mm);
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
            ConnectButton.IsEnabled = DeviceList.SelectedItem != null;
        }
    }

    private void ShowMessage(string message)
    {
        MessageLabel.Text = $"{DateTime.Now:HH:mm:ss} - {message}";
    }
}
