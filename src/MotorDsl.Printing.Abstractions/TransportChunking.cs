namespace MotorDsl.Printing;

/// <summary>
/// Utilidades puras compartidas por los transports para preparar la escritura al medio fisico.
/// Vive en Printing.Abstractions (net10.0) para que cualquier transport la consuma sin duplicarla
/// y para que se pueda testear directamente desde MotorDsl.Tests.
/// </summary>
public static class TransportChunking
{
    /// <summary>
    /// Tamano de bloque por defecto para la escritura al medio fisico. Puede bajarse a 128 para
    /// impresoras mas sensibles.
    /// </summary>
    public const int DefaultChunkSize = 256;

    /// <summary>
    /// Parte <paramref name="data"/> en segmentos contiguos de a lo sumo <paramref name="size"/>
    /// bytes, independiente del contenido: no se corta por LF (0x0A) ni por ningun byte
    /// particular. El output escpos-bitmap contiene 0x0A arbitrarios en los datos de pixeles, asi
    /// que partir por contenido corromperia la imagen.
    ///
    /// Reensamblar los segmentos en orden reproduce exactamente el array original. Es una vista
    /// (ArraySegment) sobre el buffer de entrada: no copia.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="data"/> es null.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="size"/> es menor o igual a cero.</exception>
    public static IEnumerable<ArraySegment<byte>> ChunkBuffer(byte[] data, int size = DefaultChunkSize)
    {
        if (data is null) throw new ArgumentNullException(nameof(data));
        if (size <= 0) throw new ArgumentOutOfRangeException(nameof(size), "El tamano de bloque debe ser mayor a cero");

        for (int offset = 0; offset < data.Length; offset += size)
        {
            int len = Math.Min(size, data.Length - offset);
            yield return new ArraySegment<byte>(data, offset, len);
        }
    }
}
