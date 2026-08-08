using System.Net.Sockets;
using MotorDsl.Printing;

namespace MotorDsl.Network;

/// <summary>
/// Transport de impresion por red: abre un socket TCP contra la impresora y le escribe los mismos
/// bytes ESC/POS que el transport Bluetooth envia por SPP. El puerto por defecto es el 9100
/// (RAW / JetDirect), estandar de facto de impresion cruda.
///
/// Es codigo 100% managed sobre System.Net.Sockets, sin una sola API de plataforma. Por eso el
/// paquete targetea net10.0 puro y funciona igual en Android, iOS y escritorio, a diferencia de
/// MotorDsl.Bluetooth, cuyo TFM de iOS es una cascara que lanza PlatformNotSupportedException
/// porque Apple no expone Bluetooth Classic SPP a apps de terceros.
///
/// Responsabilidad acotada, como todo IThermalPrinterTransport: solo escribir bytes en el medio.
/// El retry, la reconexion y la politica de error viven en ThermalPrinterService.
/// </summary>
public partial class NetworkPrinterTransport : IThermalPrinterTransport, IDisposable
{
    private readonly NetworkPrinterOptions _options;

    private TcpClient? _client;
    private NetworkStream? _stream;
    private PrinterDevice? _currentDevice;

    public NetworkPrinterTransport(NetworkPrinterOptions? options = null)
        => _options = options ?? new NetworkPrinterOptions();

    /// <summary>Discriminador de ruteo del servicio. Debe coincidir con PrinterDevice.Kind.</summary>
    public string Kind => "network";

    public bool IsConnected { get; private set; }

    public PrinterDevice? CurrentDevice => _currentDevice;

    /// <summary>
    /// Sin deteccion de capacidades en esta iteracion: se devuelve null, que es el valor por
    /// defecto del contrato. El sondeo por DLE EOT es posible sobre TCP y queda como extension;
    /// mientras tanto el envio usa pacing fijo, igual que el camino degradado de Bluetooth.
    /// </summary>
    public PrinterCapabilities? Capabilities => null;

    /// <summary>
    /// Devuelve los endpoints declarados en las opciones que sean sintacticamente validos.
    ///
    /// No hace barrido de subred: probar el puerto 9100 host por host es lento y se parece a un
    /// escaneo de puertos. Un endpoint invalido se descarta en silencio en vez de abortar el
    /// barrido, siguiendo el criterio del servicio de que una falla parcial no rompe el conjunto.
    /// </summary>
    public Task<IReadOnlyList<PrinterDevice>> DiscoverAsync(CancellationToken ct = default)
    {
        var devices = new List<PrinterDevice>();

        foreach (var entry in _options.KnownEndpoints)
        {
            ct.ThrowIfCancellationRequested();

            if (!TryParseEndpoint(entry, out var host, out var port)) continue;

            var id = FormatEndpoint(host, port);
            devices.Add(new PrinterDevice(id, host, Kind, IsPaired: true));
        }

        return Task.FromResult<IReadOnlyList<PrinterDevice>>(devices);
    }

    public async Task<bool> ConnectAsync(string deviceId, CancellationToken ct = default)
    {
        if (!TryParseEndpoint(deviceId, out var host, out var port))
            throw new ArgumentException($"Endpoint invalido: '{deviceId}'. Se espera host, host:puerto o [ipv6]:puerto.", nameof(deviceId));

        // Limpia cualquier socket previo antes de abrir uno nuevo (reconexion).
        InvalidateConnection();

        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(ct);
        timeout.CancelAfter(_options.ConnectTimeout);

        var client = new TcpClient();
        try
        {
            await client.ConnectAsync(host, port, timeout.Token);

            _client = client;
            _stream = client.GetStream();
            IsConnected = true;
            _currentDevice = new PrinterDevice(FormatEndpoint(host, port), host, Kind, IsPaired: true);

            return true;
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            // Vencio el timeout propio, no una cancelacion del llamador. Se traduce a
            // TimeoutException para que PrintError.FromException lo clasifique como Timeout
            // y el handler reintente.
            client.Dispose();
            IsConnected = false;
            throw new TimeoutException($"Timeout conectando a {host}:{port} tras {_options.ConnectTimeout.TotalSeconds:0.#}s.");
        }
        catch
        {
            // SocketException y IOException suben SIN envolver: ThermalPrinterService.ConnectAsync
            // las pasa por PrintError.FromException, que ya las clasifica como Connection. Si se
            // las envolviera en Exception quedarian como Unknown y se perderia esa precision.
            client.Dispose();
            IsConnected = false;
            throw;
        }
    }

    /// <summary>
    /// Cierra la conexion. NUNCA lanza: el codigo de limpieza suele llamarla sin try/catch, y una
    /// excepcion aca deja recursos colgados en el llamador.
    /// </summary>
    public Task DisconnectAsync()
    {
        InvalidateConnection();
        return Task.CompletedTask;
    }

    public async Task WriteBytesAsync(byte[] data, PrinterProfile profile, CancellationToken ct = default)
    {
        if (data is null) throw new ArgumentNullException(nameof(data));
        if (profile is null) throw new ArgumentNullException(nameof(profile));

        if (_stream is null || !IsConnected)
            throw new InvalidOperationException("No hay una impresora conectada");

        // Pacing de respaldo: UNA vez por bloque, no por byte, con piso de 1 ms. Sobre TCP el
        // control de flujo real lo aporta el protocolo, asi que estos delays son conservadores
        // y no imprescindibles como si lo son sobre SPP, que no tiene RTS/CTS.
        int pacingDelayMs = Math.Max(profile.ByteDelayMs, 1);

        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(ct);
        timeout.CancelAfter(_options.WriteTimeout);

        try
        {
            await Task.Delay(profile.InitDelayMs, timeout.Token);

            foreach (var bloque in TransportChunking.ChunkBuffer(data, _options.ChunkSize))
            {
                await _stream.WriteAsync(bloque.AsMemory(), timeout.Token);
                await _stream.FlushAsync(timeout.Token);
                await Task.Delay(pacingDelayMs, timeout.Token);
            }

            await Task.Delay(profile.FinalDelayMs, timeout.Token);
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            InvalidateConnection();
            throw new TimeoutException($"Timeout escribiendo {data.Length} bytes tras {_options.WriteTimeout.TotalSeconds:0.#}s.");
        }
        catch (Exception ex) when (ex is IOException or SocketException)
        {
            // Socket roto (impresora apagada, cable, red caida). Se invalida el estado para que
            // ReconnectInternalAsync del servicio reconecte en el proximo intento —solo reconecta
            // si IsConnected quedo en false— y se relanza SIN envolver, porque
            // PrintError.FromException ya mapea ambas a Connection, que si reintenta.
            InvalidateConnection();
            throw;
        }
    }

    /// <summary>
    /// Deja el transport en estado desconectado liberando socket y stream. Idempotente y silenciosa:
    /// se la llama tanto en la limpieza normal como en el camino de error.
    /// </summary>
    private void InvalidateConnection()
    {
        IsConnected = false;

        try { _stream?.Dispose(); } catch { /* swallow: ya estamos limpiando */ }
        try { _client?.Close(); } catch { /* swallow */ }
        try { _client?.Dispose(); } catch { /* swallow */ }

        _stream = null;
        _client = null;
        _currentDevice = null;
    }

    public void Dispose()
    {
        InvalidateConnection();
        GC.SuppressFinalize(this);
    }
}
