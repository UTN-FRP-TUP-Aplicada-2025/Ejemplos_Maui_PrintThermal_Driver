using System.Net;
using System.Net.Sockets;
using MotorDsl.Network;
using MotorDsl.Printing;

namespace MotorDsl.Tests;

/// <summary>
/// Cubre NetworkPrinterTransport de punta a punta contra un TcpListener local que hace de
/// impresora de red. Verifica lo que importa de un transport: que los bytes lleguen EXACTOS
/// (el chunking no debe corromper el payload) y que los errores suban con el tipo correcto para
/// que la politica de retry de ThermalPrinterService los clasifique bien.
/// </summary>
public class NetworkTransportTests
{
    /// <summary>Perfil sin esperas, para que los tests no paguen el pacing de produccion.</summary>
    private static PrinterProfile FastProfile => new()
    {
        Name = "test",
        InitDelayMs = 0,
        ByteDelayMs = 0,
        FinalDelayMs = 0
    };

    // ── Camino feliz: paridad byte a byte ──

    [Fact]
    public async Task Los_bytes_llegan_exactos_a_la_impresora()
    {
        await using var server = FakePrinterServer.Start();
        using var transport = new NetworkPrinterTransport();

        var payload = new byte[] { 0x1B, 0x40, (byte)'H', (byte)'i', 0x0A, 0x1D, 0x56, 0x00 };

        Assert.True(await transport.ConnectAsync(server.Endpoint));
        await transport.WriteBytesAsync(payload, FastProfile);

        Assert.Equal(payload, await server.WaitForBytesAsync(payload.Length));
    }

    [Fact]
    public async Task Payload_grande_con_0x0A_intercalados_se_reensambla_exacto()
    {
        // Caso critico: la salida escpos-bitmap lleva 0x0A arbitrarios en los datos de pixeles.
        // Si el chunking partiera por contenido, la imagen saldria corrupta.
        await using var server = FakePrinterServer.Start();
        using var transport = new NetworkPrinterTransport();

        var payload = new byte[5000];
        for (int i = 0; i < payload.Length; i++)
            payload[i] = (i % 3 == 0) ? (byte)0x0A : (byte)(i % 256);

        Assert.True(await transport.ConnectAsync(server.Endpoint));
        await transport.WriteBytesAsync(payload, FastProfile);

        Assert.Equal(payload, await server.WaitForBytesAsync(payload.Length));
    }

    [Fact]
    public async Task Dos_envios_consecutivos_se_concatenan_en_orden()
    {
        await using var server = FakePrinterServer.Start();
        using var transport = new NetworkPrinterTransport();

        Assert.True(await transport.ConnectAsync(server.Endpoint));
        await transport.WriteBytesAsync(new byte[] { 1, 2, 3 }, FastProfile);
        await transport.WriteBytesAsync(new byte[] { 4, 5, 6 }, FastProfile);

        Assert.Equal(new byte[] { 1, 2, 3, 4, 5, 6 }, await server.WaitForBytesAsync(6));
    }

    // ── Estado de la conexion ──

    [Fact]
    public async Task Conectar_expone_el_dispositivo_con_Kind_network()
    {
        await using var server = FakePrinterServer.Start();
        using var transport = new NetworkPrinterTransport();

        Assert.False(transport.IsConnected);
        Assert.Null(transport.CurrentDevice);

        Assert.True(await transport.ConnectAsync(server.Endpoint));

        Assert.True(transport.IsConnected);
        Assert.Equal("network", transport.Kind);
        Assert.Equal("network", transport.CurrentDevice!.Kind);
        Assert.Equal(server.Endpoint, transport.CurrentDevice.Id);
    }

    [Fact]
    public async Task Desconectar_limpia_el_estado_y_no_lanza_aunque_no_haya_conexion()
    {
        await using var server = FakePrinterServer.Start();
        using var transport = new NetworkPrinterTransport();

        // Sin conectar: no debe lanzar. El codigo de limpieza suele llamarla sin try/catch.
        await transport.DisconnectAsync();

        await transport.ConnectAsync(server.Endpoint);
        await transport.DisconnectAsync();

        Assert.False(transport.IsConnected);
        Assert.Null(transport.CurrentDevice);

        // Idempotente.
        await transport.DisconnectAsync();
    }

    // ── Errores y su clasificacion ──

    [Fact]
    public async Task Escribir_sin_conexion_lanza_InvalidOperationException()
    {
        using var transport = new NetworkPrinterTransport();

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => transport.WriteBytesAsync(new byte[] { 1 }, FastProfile));
    }

    [Fact]
    public async Task Conectar_a_un_puerto_cerrado_lanza_SocketException_sin_envolver()
    {
        // SocketException sin envolver es lo que hace que PrintError.FromException la clasifique
        // como Connection y el handler reintente. Envolverla en Exception la degradaria a Unknown.
        var puertoLibre = FakePrinterServer.ReserveClosedPort();
        using var transport = new NetworkPrinterTransport();

        await Assert.ThrowsAsync<SocketException>(
            () => transport.ConnectAsync($"127.0.0.1:{puertoLibre}"));

        Assert.False(transport.IsConnected);
    }

    [Fact]
    public async Task Endpoint_invalido_lanza_ArgumentException()
    {
        using var transport = new NetworkPrinterTransport();

        await Assert.ThrowsAsync<ArgumentException>(() => transport.ConnectAsync("192.168.1.50:0"));
    }

    [Fact]
    public async Task Si_la_impresora_desaparece_a_mitad_del_envio_el_error_sube_sin_envolver()
    {
        var server = FakePrinterServer.Start();
        using var transport = new NetworkPrinterTransport();

        Assert.True(await transport.ConnectAsync(server.Endpoint));

        // La impresora se apaga: se cierra el socket aceptado y el listener.
        await server.DisposeAsync();

        // Payload grande para llenar el buffer del kernel y forzar el fallo de escritura.
        var payload = new byte[2 * 1024 * 1024];

        var ex = await Record.ExceptionAsync(() => transport.WriteBytesAsync(payload, FastProfile));

        Assert.NotNull(ex);
        Assert.True(ex is IOException or SocketException,
            $"Se esperaba IOException o SocketException sin envolver, llego {ex!.GetType().Name}");

        // Critico: ThermalPrinterService.ReconnectInternalAsync solo reconecta si IsConnected
        // quedo en false. Sin esto el transport nunca se recupera.
        Assert.False(transport.IsConnected);
    }

    // ── Descubrimiento ──

    [Fact]
    public async Task Discover_devuelve_los_endpoints_configurados_y_descarta_los_invalidos()
    {
        var options = new NetworkPrinterOptions()
            .AddPrinter("192.168.1.50")
            .AddPrinter("impresora-deposito:9101")
            .AddPrinter(":::invalido:::");

        using var transport = new NetworkPrinterTransport(options);

        var devices = await transport.DiscoverAsync();

        Assert.Equal(2, devices.Count);
        Assert.All(devices, d => Assert.Equal("network", d.Kind));
        Assert.Equal("192.168.1.50:9100", devices[0].Id);
        Assert.Equal("impresora-deposito:9101", devices[1].Id);
    }

    [Fact]
    public async Task Discover_sin_endpoints_configurados_devuelve_vacio()
    {
        using var transport = new NetworkPrinterTransport();

        Assert.Empty(await transport.DiscoverAsync());
    }

    // ── Capacidades y NV: defaults del contrato ──

    [Fact]
    public async Task Capabilities_es_null_y_NV_degrada_sin_lanzar()
    {
        using var transport = new NetworkPrinterTransport();

        Assert.Null(transport.Capabilities);

        // Se apoya en las implementaciones default de IThermalPrinterTransport.
        IThermalPrinterTransport t = transport;
        var resultado = await t.ProvisionLogoAsync(new byte[] { 1 }, keycode: 32);

        Assert.False(resultado.Success);
        Assert.False(await t.IsLogoProvisionedAsync(32));
        await t.ClearLogoAsync(32);
    }

    /// <summary>
    /// Impresora de red simulada: escucha en 127.0.0.1 con puerto efimero y acumula todo lo que
    /// recibe. Reemplaza al hardware en los tests de escritura.
    /// </summary>
    private sealed class FakePrinterServer : IAsyncDisposable
    {
        private readonly TcpListener _listener;
        private readonly CancellationTokenSource _cts = new();
        private readonly List<byte> _received = [];
        private readonly Lock _gate = new();
        private readonly Task _loop;

        private FakePrinterServer(TcpListener listener)
        {
            _listener = listener;
            _loop = Task.Run(AcceptAndReadAsync);
        }

        public string Endpoint { get; private init; } = string.Empty;

        public static FakePrinterServer Start()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;

            return new FakePrinterServer(listener) { Endpoint = $"127.0.0.1:{port}" };
        }

        /// <summary>Abre y cierra un listener para obtener un puerto donde con certeza no escucha nadie.</summary>
        public static int ReserveClosedPort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        private async Task AcceptAndReadAsync()
        {
            try
            {
                using var client = await _listener.AcceptTcpClientAsync(_cts.Token);
                await using var stream = client.GetStream();

                var buffer = new byte[8192];
                while (!_cts.IsCancellationRequested)
                {
                    int read = await stream.ReadAsync(buffer, _cts.Token);
                    if (read == 0) break;

                    lock (_gate) _received.AddRange(buffer.AsSpan(0, read).ToArray());
                }
            }
            catch (OperationCanceledException) { /* cierre normal */ }
            catch (SocketException) { /* cierre normal */ }
            catch (IOException) { /* cierre normal */ }
        }

        /// <summary>Espera hasta juntar <paramref name="expectedLength"/> bytes o vencer el timeout.</summary>
        public async Task<byte[]> WaitForBytesAsync(int expectedLength, int timeoutMs = 5000)
        {
            var limite = Environment.TickCount64 + timeoutMs;

            while (Environment.TickCount64 < limite)
            {
                lock (_gate)
                {
                    if (_received.Count >= expectedLength) return [.. _received];
                }
                await Task.Delay(10);
            }

            lock (_gate)
            {
                throw new TimeoutException(
                    $"Se esperaban {expectedLength} bytes y llegaron {_received.Count} en {timeoutMs} ms.");
            }
        }

        public async ValueTask DisposeAsync()
        {
            await _cts.CancelAsync();
            try { _listener.Stop(); } catch { /* ya cerrado */ }
            try { await _loop; } catch { /* ya cancelado */ }
            _cts.Dispose();
        }
    }
}
