using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace MotorDsl.Network;

/// <summary>
/// Parte pura del transport: interpretacion del identificador de dispositivo. Vive separada del
/// ciclo de vida del socket para poder testearse sin abrir conexiones.
///
/// El contrato IThermalPrinterTransport define deviceId como string libre. En Bluetooth es una
/// MAC; aca es un endpoint de red, que se acepta en tres formas:
///   "192.168.1.50"        -> puerto por defecto 9100
///   "192.168.1.50:9100"   -> puerto explicito
///   "[fe80::1]:9100"      -> IPv6, con corchetes segun RFC 3986
/// Tambien se admite un nombre de host ("impresora-deposito") porque la resolucion la hace el
/// socket, no este parser.
/// </summary>
public partial class NetworkPrinterTransport
{
    /// <summary>
    /// Puerto RAW / JetDirect. Es el estandar de facto de impresion cruda por TCP y lo expone
    /// practicamente toda impresora termica con red.
    /// </summary>
    public const int DefaultPort = 9100;

    /// <summary>
    /// Interpreta un deviceId como endpoint de red. Devuelve false ante entrada invalida en vez
    /// de lanzar, para que el llamador decida como reportarlo.
    /// </summary>
    public static bool TryParseEndpoint(
        string? deviceId,
        [NotNullWhen(true)] out string? host,
        out int port)
    {
        host = null;
        port = DefaultPort;

        if (string.IsNullOrWhiteSpace(deviceId)) return false;

        var raw = deviceId.Trim();

        // IPv6 entre corchetes: "[fe80::1]" o "[fe80::1]:9100".
        if (raw.StartsWith('['))
        {
            int close = raw.IndexOf(']');
            if (close <= 1) return false;

            var inner = raw[1..close];
            if (!IPAddress.TryParse(inner, out _)) return false;

            var rest = raw[(close + 1)..];
            if (rest.Length == 0)
            {
                host = inner;
                return true;
            }

            if (rest[0] != ':' || !TryParsePort(rest[1..], out port)) return false;

            host = inner;
            return true;
        }

        // IPv6 sin corchetes: no se le puede separar el puerto sin ambiguedad, asi que se acepta
        // tal cual con el puerto por defecto.
        if (raw.Count(c => c == ':') > 1)
        {
            if (!IPAddress.TryParse(raw, out _)) return false;
            host = raw;
            return true;
        }

        int sep = raw.IndexOf(':');
        if (sep < 0)
        {
            host = raw;
            return true;
        }

        if (sep == 0) return false;

        if (!TryParsePort(raw[(sep + 1)..], out port)) return false;

        host = raw[..sep];
        return true;
    }

    private static bool TryParsePort(string text, out int port)
    {
        port = DefaultPort;
        if (!int.TryParse(text, out var parsed)) return false;
        if (parsed is < 1 or > 65535) return false;

        port = parsed;
        return true;
    }

    /// <summary>
    /// Normaliza host y puerto a la forma canonica usada como Id de PrinterDevice. Es la inversa
    /// de <see cref="TryParseEndpoint"/>: reparsear el resultado devuelve los mismos valores.
    /// </summary>
    public static string FormatEndpoint(string host, int port)
        => IPAddress.TryParse(host, out var ip) && ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6
            ? $"[{host}]:{port}"
            : $"{host}:{port}";
}
