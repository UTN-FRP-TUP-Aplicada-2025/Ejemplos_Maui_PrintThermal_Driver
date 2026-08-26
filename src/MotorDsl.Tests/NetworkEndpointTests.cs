using MotorDsl.Network;

namespace MotorDsl.Tests;

/// <summary>
/// Cubre NetworkPrinterTransport.TryParseEndpoint y FormatEndpoint: la parte pura del transport
/// de red, que interpreta el deviceId del contrato (un string libre) como host + puerto. Se testea
/// sin abrir sockets.
/// </summary>
public class NetworkEndpointTests
{
    // ── Formas validas ──

    [Fact]
    public void Host_solo_usa_el_puerto_9100_por_defecto()
    {
        Assert.True(NetworkPrinterTransport.TryParseEndpoint("192.168.1.50", out var host, out var port));

        Assert.Equal("192.168.1.50", host);
        Assert.Equal(9100, port);
    }

    [Fact]
    public void Host_con_puerto_explicito_lo_respeta()
    {
        Assert.True(NetworkPrinterTransport.TryParseEndpoint("192.168.1.50:9101", out var host, out var port));

        Assert.Equal("192.168.1.50", host);
        Assert.Equal(9101, port);
    }

    [Fact]
    public void Nombre_de_host_se_acepta_sin_resolver()
    {
        // La resolucion DNS la hace el socket, no el parser: aca solo se separa host de puerto.
        Assert.True(NetworkPrinterTransport.TryParseEndpoint("impresora-deposito:9100", out var host, out var port));

        Assert.Equal("impresora-deposito", host);
        Assert.Equal(9100, port);
    }

    [Fact]
    public void Ipv6_entre_corchetes_con_puerto()
    {
        Assert.True(NetworkPrinterTransport.TryParseEndpoint("[fe80::1]:9100", out var host, out var port));

        Assert.Equal("fe80::1", host);
        Assert.Equal(9100, port);
    }

    [Fact]
    public void Ipv6_entre_corchetes_sin_puerto_usa_el_default()
    {
        Assert.True(NetworkPrinterTransport.TryParseEndpoint("[fe80::1]", out var host, out var port));

        Assert.Equal("fe80::1", host);
        Assert.Equal(9100, port);
    }

    [Fact]
    public void Ipv6_sin_corchetes_se_acepta_entero_con_el_puerto_default()
    {
        // Sin corchetes los ':' del propio literal son ambiguos, asi que no se intenta separar puerto.
        Assert.True(NetworkPrinterTransport.TryParseEndpoint("fe80::1", out var host, out var port));

        Assert.Equal("fe80::1", host);
        Assert.Equal(9100, port);
    }

    [Fact]
    public void Se_recortan_los_espacios()
    {
        Assert.True(NetworkPrinterTransport.TryParseEndpoint("  192.168.1.50:9100  ", out var host, out var port));

        Assert.Equal("192.168.1.50", host);
        Assert.Equal(9100, port);
    }

    // ── Formas invalidas ──

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(":9100")]           // sin host
    [InlineData("192.168.1.50:")]   // puerto vacio
    [InlineData("192.168.1.50:abc")]// puerto no numerico
    [InlineData("192.168.1.50:0")]  // fuera de rango
    [InlineData("192.168.1.50:65536")]
    [InlineData("[]:9100")]         // corchetes vacios
    [InlineData("[nose]:9100")]     // no es una IPv6
    [InlineData("fe80::1::2")]      // IPv6 malformada
    public void Entradas_invalidas_devuelven_false(string? entrada)
        => Assert.False(NetworkPrinterTransport.TryParseEndpoint(entrada, out _, out _));

    // ── Ida y vuelta ──

    [Theory]
    [InlineData("192.168.1.50", 9100)]
    [InlineData("impresora-deposito", 9101)]
    [InlineData("fe80::1", 9100)]
    public void FormatEndpoint_es_la_inversa_de_TryParseEndpoint(string host, int port)
    {
        var canonico = NetworkPrinterTransport.FormatEndpoint(host, port);

        Assert.True(NetworkPrinterTransport.TryParseEndpoint(canonico, out var host2, out var port2));
        Assert.Equal(host, host2);
        Assert.Equal(port, port2);
    }

    [Fact]
    public void FormatEndpoint_pone_corchetes_solo_en_ipv6()
    {
        Assert.Equal("192.168.1.50:9100", NetworkPrinterTransport.FormatEndpoint("192.168.1.50", 9100));
        Assert.Equal("[fe80::1]:9100", NetworkPrinterTransport.FormatEndpoint("fe80::1", 9100));
    }
}
