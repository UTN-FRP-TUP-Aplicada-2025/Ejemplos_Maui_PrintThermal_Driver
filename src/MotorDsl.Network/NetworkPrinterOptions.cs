namespace MotorDsl.Network;

/// <summary>
/// Configuracion del transport de red.
///
/// El descubrimiento por red no tiene un equivalente al barrido Bluetooth: no existe una lista de
/// "dispositivos emparejados" que consultar. Esta primera iteracion resuelve el caso real con
/// endpoints declarados por la app —tipicamente uno solo, persistido tras que el usuario lo cargo
/// una vez—. El descubrimiento automatico por mDNS queda como extension futura y no cambia el
/// contrato: <c>DiscoverAsync</c> seguira devolviendo PrinterDevice con Kind "network".
/// </summary>
public sealed class NetworkPrinterOptions
{
    /// <summary>
    /// Endpoints conocidos, en cualquiera de las formas que acepta
    /// <see cref="NetworkPrinterTransport.TryParseEndpoint"/>. Los invalidos se descartan
    /// silenciosamente en el descubrimiento: una entrada mal cargada no debe romper el barrido.
    /// </summary>
    public IList<string> KnownEndpoints { get; } = new List<string>();

    /// <summary>Timeout de establecimiento de conexion. Vencido, se lanza TimeoutException (reintenta).</summary>
    public TimeSpan ConnectTimeout { get; set; } = TimeSpan.FromSeconds(5);

    /// <summary>Timeout de escritura sobre el socket ya establecido.</summary>
    public TimeSpan WriteTimeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Tamano de bloque de escritura. El valor por defecto replica el del transport Bluetooth por
    /// consistencia de comportamiento; en TCP no es una restriccion del medio, ya que el control
    /// de flujo real lo aporta el propio protocolo.
    /// </summary>
    public int ChunkSize { get; set; } = MotorDsl.Printing.TransportChunking.DefaultChunkSize;

    /// <summary>Agrega un endpoint conocido y devuelve las opciones, para encadenar.</summary>
    public NetworkPrinterOptions AddPrinter(string endpoint)
    {
        if (!string.IsNullOrWhiteSpace(endpoint)) KnownEndpoints.Add(endpoint.Trim());
        return this;
    }
}
