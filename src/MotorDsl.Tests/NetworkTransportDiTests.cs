using Microsoft.Extensions.DependencyInjection;
using MotorDsl.Network;
using MotorDsl.Printing;

namespace MotorDsl.Tests;

/// <summary>
/// Cubre el registro en DI de AddNetworkPrinterTransport. Lo que se verifica no es que resuelva
/// —eso seria trivial— sino que CONVIVA con otros transports: ThermalPrinterService recibe
/// IEnumerable&lt;IThermalPrinterTransport&gt; y rutea por Kind, asi que un registro que desplace a
/// los demas romperia el escenario multiplataforma (red en iOS, bluetooth en Android).
/// </summary>
public class NetworkTransportDiTests
{
    [Fact]
    public void Registra_un_transport_con_Kind_network()
    {
        var provider = new ServiceCollection()
            .AddNetworkPrinterTransport()
            .BuildServiceProvider();

        var transport = provider.GetRequiredService<IThermalPrinterTransport>();

        Assert.IsType<NetworkPrinterTransport>(transport);
        Assert.Equal("network", transport.Kind);
    }

    [Fact]
    public void Convive_con_otro_transport_sin_desplazarlo()
    {
        var services = new ServiceCollection();
        services.AddSingleton<IThermalPrinterTransport, FakeTransport>();
        services.AddNetworkPrinterTransport();

        var transports = services.BuildServiceProvider()
            .GetRequiredService<IEnumerable<IThermalPrinterTransport>>()
            .ToList();

        Assert.Equal(2, transports.Count);
        Assert.Contains(transports, t => t.Kind == "fake");
        Assert.Contains(transports, t => t.Kind == "network");
    }

    [Fact]
    public async Task La_configuracion_llega_al_transport()
    {
        var provider = new ServiceCollection()
            .AddNetworkPrinterTransport(o => o
                .AddPrinter("192.168.1.50")
                .AddPrinter("192.168.1.51:9101"))
            .BuildServiceProvider();

        var options = provider.GetRequiredService<NetworkPrinterOptions>();
        Assert.Equal(2, options.KnownEndpoints.Count);

        var devices = await provider.GetRequiredService<IThermalPrinterTransport>().DiscoverAsync();

        Assert.Equal(["192.168.1.50:9100", "192.168.1.51:9101"], devices.Select(d => d.Id));
    }

    [Fact]
    public void El_transport_se_registra_como_singleton()
    {
        var provider = new ServiceCollection()
            .AddNetworkPrinterTransport()
            .BuildServiceProvider();

        Assert.Same(
            provider.GetRequiredService<IThermalPrinterTransport>(),
            provider.GetRequiredService<IThermalPrinterTransport>());
    }
}
