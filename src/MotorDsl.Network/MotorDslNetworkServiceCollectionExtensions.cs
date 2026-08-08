using Microsoft.Extensions.DependencyInjection;
using MotorDsl.Printing;

namespace MotorDsl.Network;

public static class MotorDslNetworkServiceCollectionExtensions
{
    /// <summary>
    /// Registra el transport de red (Kind "network").
    ///
    /// Usa AddSingleton y no TryAdd, igual que AddBluetoothPrinterTransport: ThermalPrinterService
    /// recibe IEnumerable&lt;IThermalPrinterTransport&gt; y rutea por Kind, de modo que varios
    /// transports coexisten. Una app multiplataforma tipica registra este siempre y el Bluetooth
    /// solo bajo #if ANDROID.
    /// </summary>
    public static IServiceCollection AddNetworkPrinterTransport(
        this IServiceCollection services,
        Action<NetworkPrinterOptions>? configure = null)
    {
        var options = new NetworkPrinterOptions();
        configure?.Invoke(options);

        services.AddSingleton(options);
        services.AddSingleton<IThermalPrinterTransport>(sp => new NetworkPrinterTransport(options));
        return services;
    }
}
