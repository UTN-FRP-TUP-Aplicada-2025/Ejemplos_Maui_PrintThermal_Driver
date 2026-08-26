#if ANDROID
using Android.Bluetooth;
using Android.Content;
using Java.Util;
using AndroidBluetoothDevice = Android.Bluetooth.BluetoothDevice;
#endif
using MotorDsl.Core.Models;
using MotorDsl.Printing;

namespace MotorDsl.Bluetooth;

/// <summary>
/// Transport Bluetooth Classic SPP (Android).
/// iOS lanza PlatformNotSupportedException porque no soporta Bluetooth clasico SPP.
/// Solo escribe bytes — el retry y la orquestacion se manejan en ThermalPrinterService.
/// </summary>
public partial class BluetoothPrinterTransport : IThermalPrinterTransport
{
#if ANDROID
    private BluetoothSocket? _socket;
    private BluetoothAdapter? _bluetoothAdapter;
    private System.IO.Stream? _outputStream;
    // Stream de entrada usado por la deteccion de capacidades y el flow control por status.
    private System.IO.Stream? _inputStream;
    // InputStream Java subyacente: permite Available() para gatear los reads y no bloquear.
    // Null si no se pudo capturar; en ese caso el status degrada a pacing fijo.
    private Java.IO.InputStream? _javaInputStream;
    // Capacidades detectadas al conectar; null hasta detectar, Unknown() tras invalidar.
    private PrinterCapabilities? _capabilities;

    // Marca que el ultimo cierre derribo un enlace VIVO. BluetoothSocket.Close() vuelve
    // enseguida, pero el stack de Android sigue liberando RFCOMM/L2CAP/ACL por cientos de
    // ms: la proxima conexion tiene que esperar ese teardown (ver ConnectAsync).
    private bool _pendingLinkTeardown;

    // Espera tras derribar un enlace vivo, antes de abrir el socket nuevo. Medida en un
    // moto g42 contra una MTP-II: el RFCOMM viejo cierra a +35 ms del Close() y el ACL
    // recien cae a +276 ms. Sin esta espera la impresora, al procesar la desconexion del
    // canal anterior, tira el enlace entero (hciReason 19 = remote user terminated) y se
    // lleva puesta la conexion nueva, que queda a medio configurar.
    private const int LINK_TEARDOWN_SETTLE_MS = 600;

    // Intentos de apertura del socket RFCOMM dentro de UNA llamada a ConnectAsync. El
    // segundo intento cubre la variabilidad del teardown entre modelos de impresora.
    private const int CONNECT_MAX_ATTEMPTS = 2;

    // Backoff entre intentos de apertura.
    private const int CONNECT_RETRY_DELAY_MS = 800;
#endif

    private string? _lastDeviceAddress;
    private PrinterDevice? _currentDevice;

    public string Kind => "bluetooth";
    public bool IsConnected { get; private set; }
    public PrinterDevice? CurrentDevice => _currentDevice;

#if ANDROID
    public PrinterCapabilities? Capabilities => _capabilities;
#else
    public PrinterCapabilities? Capabilities => null;
#endif

    public BluetoothPrinterTransport()
    {
#if ANDROID
        _bluetoothAdapter = BluetoothAdapter.DefaultAdapter;
#endif
    }

    public Task<IReadOnlyList<PrinterDevice>> DiscoverAsync(CancellationToken ct = default)
    {
        var devices = new List<PrinterDevice>();

#if ANDROID
        if (_bluetoothAdapter == null)
            throw new Exception("Bluetooth no esta disponible en este dispositivo");

        if (!_bluetoothAdapter.IsEnabled)
            throw new Exception("Bluetooth esta desactivado. Por favor activalo.");

        if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.S)
        {
            var ctx = Android.App.Application.Context;
            var permStatus = ctx.CheckSelfPermission(Android.Manifest.Permission.BluetoothConnect);
            if (permStatus != Android.Content.PM.Permission.Granted)
            {
                throw new Exception(
                    "Permiso BLUETOOTH_CONNECT no otorgado. Reinicia la app y acepta los permisos.");
            }
        }

        try
        {
            var bondedDevices = _bluetoothAdapter.BondedDevices;

            if (bondedDevices != null && bondedDevices.Count > 0)
            {
                foreach (AndroidBluetoothDevice device in bondedDevices)
                {
                    var name = device.Name ?? "Dispositivo desconocido";
                    var addr = device.Address ?? "";
                    devices.Add(new PrinterDevice(addr, name, "bluetooth", IsPaired: true));
                }
            }
        }
        catch (Java.Lang.SecurityException)
        {
            return Task.FromResult<IReadOnlyList<PrinterDevice>>(new List<PrinterDevice>());
        }
#elif IOS
        throw new PlatformNotSupportedException("iOS no soporta Bluetooth Classic SPP. Usar BLE o impresion por red.");
#endif
        return Task.FromResult<IReadOnlyList<PrinterDevice>>(devices);
    }

    /// <summary>
    /// Abre el socket RFCOMM SPP contra <paramref name="deviceId"/>.
    /// <para>
    /// Si el cierre anterior derribo un enlace VIVO —reconectar sobre una conexion abierta,
    /// <see cref="DisconnectAsync"/>, o una escritura fallida que invalido la conexion—
    /// espera a que el stack lo libere antes de abrir el nuevo, y reintenta una vez ante
    /// fallo. Sin esa espera, la peticion de conexion sale ~6 ms despues del Close() y entra
    /// en carrera con el teardown: hay impresoras que responden tirando el ACL completo, que
    /// se lleva puesto el socket recien abierto. Era el fallo de la 2.a impresion consecutiva.
    /// </para>
    /// </summary>
    public async Task<bool> ConnectAsync(string deviceId, CancellationToken ct = default)
    {
#if ANDROID
        // Limpia cualquier socket/stream previo antes de abrir uno nuevo (reconexion).
        // IsConnected queda en false hasta confirmar la conexion mas abajo.
        InvalidateConnection();

        // Solo se paga cuando REALMENTE habia un enlace que derribar: la primera conexion
        // del proceso no espera nada.
        if (_pendingLinkTeardown)
        {
            _pendingLinkTeardown = false;
            await Task.Delay(LINK_TEARDOWN_SETTLE_MS, ct);
        }

        Exception? ultimoError = null;
        for (int intento = 1; intento <= CONNECT_MAX_ATTEMPTS; intento++)
        {
            if (intento > 1) await Task.Delay(CONNECT_RETRY_DELAY_MS, ct);

            try
            {
                return await AbrirSocketAsync(deviceId, ct);
            }
            catch (OperationCanceledException)
            {
                // Cancelacion del llamador: no es un fallo de conexion, no se reintenta.
                DiscardSocket();
                throw;
            }
            catch (Exception ex)
            {
                ultimoError = ex;
                // Descarta el socket a medio abrir SIN marcar teardown pendiente: no llego a
                // establecerse un enlace, y marcarlo agregaria una espera de mas.
                DiscardSocket();
            }
        }

        IsConnected = false;
        throw new Exception($"Error al conectar: {ultimoError!.Message}", ultimoError);
#elif IOS
        await Task.CompletedTask;
        throw new PlatformNotSupportedException("iOS no soporta Bluetooth Classic SPP. Usar BLE o impresion por red.");
#else
        await Task.CompletedTask;
        return false;
#endif
    }

#if ANDROID
    /// <summary>
    /// Un intento de apertura del socket RFCOMM: SDP + Connect + streams + deteccion de
    /// capacidades. No limpia nada al fallar: de eso se encarga <see cref="ConnectAsync"/>,
    /// que es quien decide si reintenta.
    /// </summary>
    private async Task<bool> AbrirSocketAsync(string deviceId, CancellationToken ct)
    {
        var device = _bluetoothAdapter!.GetRemoteDevice(deviceId)
            ?? throw new Exception("No se pudo encontrar el dispositivo");

        var uuid = UUID.FromString("00001101-0000-1000-8000-00805F9B34FB")!;
        _socket = device.CreateRfcommSocketToServiceRecord(uuid)!;

        await Task.Run(() => _socket.Connect(), ct);
        _outputStream = _socket.OutputStream;
        _inputStream = _socket.InputStream;
        // Capturamos el InputStream Java subyacente para poder usar Available() y leer SIN
        // bloquear (gating por bytes disponibles). Si no se puede, el status degrada solo.
        _javaInputStream = (_inputStream as Android.Runtime.InputStreamInvoker)?.BaseInputStream;

        IsConnected = true;
        _lastDeviceAddress = deviceId;
        _currentDevice = new PrinterDevice(deviceId, device.Name ?? deviceId, "bluetooth", IsPaired: true);

        // Deteccion de capacidades best-effort, tras tener socket + streams. NUNCA puede
        // hacer fallar la conexion: ante cualquier excepcion, queda en Unknown y se continua.
        try { _capabilities = await DetectCapabilitiesAsync(ct); }
        catch { _capabilities = PrinterCapabilities.Unknown(); }

        return true;
    }
#endif

    public async Task DisconnectAsync()
    {
#if ANDROID
        // Desconectar deja el enlace liberandose: si el llamador vuelve a conectar enseguida,
        // ConnectAsync tiene que esperar ese teardown igual que tras una reconexion.
        if (_socket != null) _pendingLinkTeardown = true;

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
            _currentDevice = null;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error al desconectar: {ex.Message}", ex);
        }
#elif IOS
        await Task.CompletedTask;
        throw new PlatformNotSupportedException("iOS no soporta Bluetooth Classic SPP. Usar BLE o impresion por red.");
#else
        await Task.CompletedTask;
#endif
    }

    public async Task WriteBytesAsync(byte[] data, PrinterProfile profile, CancellationToken ct = default)
    {
#if ANDROID
        if (_outputStream == null)
            throw new InvalidOperationException("No hay una impresora conectada");

        // Chunking de TAMANO FIJO sobre todo el buffer, independiente del contenido.
        // El output escpos-bitmap contiene 0x0A arbitrarios en los datos de pixeles, asi
        // que no se puede partir por LF. Puede bajarse a 128 para impresoras mas sensibles.
        const int CHUNK_SIZE = 256;

        // Pacing de fallback acotado: se aplica UNA vez por bloque (no por byte), con
        // un piso minimo de 1ms. Nada de delays gigantes derivados del tamano del buffer.
        int fallbackDelayMs = Math.Max(profile.ByteDelayMs, 1);

        // Flow control por status SOLO si la impresora reporto feedback en la deteccion (fase 2).
        // LIMITE HONESTO: DLE EOT da liveness/papel/error en tiempo real, NO bytes libres del
        // buffer. No es flow control perfecto: gatea bloques por online/listo, detecta caidas
        // antes de escribir a ciegas y falla claro ante papel/tapa. Donde no hay feedback (o si
        // el status se vuelve ilegible) degrada al pacing fijo de fase 1, sin abortar.
        bool statusGating = Capabilities?.StatusFeedback == CapabilitySupport.Supported;
        bool degradedPacing = false;

        try
        {
            // Fast-fail de hardware antes del primer bloque (papel/tapa). Lanza
            // PrinterHardwareException (clasificada como Hardware -> el handler NO reintenta).
            // Va dentro del try: una IOException aca invalida+reconecta como en fase 1; la
            // PrinterHardwareException no es IOException, asi que sube limpia sin invalidar.
            if (statusGating)
                await CheckHardwareFastFailAsync(ct);

            await Task.Delay(profile.InitDelayMs, ct);

            int chunkIndex = 0;
            foreach (var bloque in ChunkBuffer(data, CHUNK_SIZE))
            {
                bool applyFixedPacing = true;

                if (statusGating && (chunkIndex % POLL_EVERY_N_CHUNKS == 0))
                {
                    // Una vez degradado no se vuelve a consultar el status en ESTE envio.
                    PrinterStatus outcome = degradedPacing
                        ? PrinterStatus.Unknown
                        : await WaitUntilReadyAsync(ct);
                    (degradedPacing, applyFixedPacing) = NextPacingDecision(degradedPacing, outcome);
                }

                await _outputStream!.WriteAsync(bloque.Array!, bloque.Offset, bloque.Count, ct);
                await _outputStream.FlushAsync(ct);

                if (applyFixedPacing)
                    await Task.Delay(fallbackDelayMs, ct);

                chunkIndex++;
            }

            await Task.Delay(profile.FinalDelayMs, ct);
        }
        catch (System.IO.IOException)
        {
            // Socket roto (ej. "Broken pipe" por overrun de la impresora): invalida el
            // estado para que ReconnectInternalAsync del servicio reconecte en el proximo
            // intento, y relanza la excepcion original SIN envolver.
            InvalidateConnection();
            throw;
        }
        catch (Java.IO.IOException)
        {
            InvalidateConnection();
            throw;
        }
#elif IOS
        await Task.CompletedTask;
        throw new PlatformNotSupportedException("iOS no soporta Bluetooth Classic SPP. Usar BLE o impresion por red.");
#else
        await Task.CompletedTask;
#endif
    }

#if ANDROID
    /// <summary>
    /// Invalida la conexion al fallar una escritura: marca IsConnected=false, descarta
    /// streams y socket (con guardas null y swallow de errores internos, no lanza) y deja
    /// todo en null. Asi ReconnectInternalAsync del servicio detecta IsConnected==false y
    /// vuelve a conectar en el siguiente intento del retry.
    /// <para>
    /// A diferencia de <see cref="DiscardSocket"/>, si habia un socket deja marcado
    /// <c>_pendingLinkTeardown</c>: el enlace no queda liberado al volver de <c>Close()</c> y
    /// la proxima conexion tiene que esperarlo.
    /// </para>
    /// </summary>
    private void InvalidateConnection()
    {
        if (_socket != null) _pendingLinkTeardown = true;
        DiscardSocket();
    }

    /// <summary>
    /// Descarta streams y socket sin marcar teardown pendiente. Es el cierre de un socket que
    /// <b>no llego a establecer enlace</b> (intento de conexion fallido): no hay nada que el
    /// stack tenga que liberar, asi que no corresponde penalizar al proximo intento.
    /// </summary>
    private void DiscardSocket()
    {
        IsConnected = false;

        try { _outputStream?.Dispose(); } catch { /* swallow: ya estamos limpiando */ }
        try { _inputStream?.Dispose(); } catch { /* swallow */ }
        try { _socket?.Close(); } catch { /* swallow */ }
        try { _socket?.Dispose(); } catch { /* swallow */ }

        _outputStream = null;
        _inputStream = null;
        _javaInputStream = null;
        _socket = null;
        _capabilities = PrinterCapabilities.Unknown();
    }
#endif
}
