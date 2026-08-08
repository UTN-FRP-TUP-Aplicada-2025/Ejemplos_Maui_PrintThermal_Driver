using MotorDsl.Printing;

namespace MotorDsl.Tests;

/// <summary>
/// Cubre TransportChunking, el chunker compartido que vive en Printing.Abstractions para que
/// cualquier transport lo consuma. Misma semantica que la copia interna de MotorDsl.Bluetooth
/// (ver ChunkBufferTests): parte por TAMANO y nunca por contenido, asi que los 0x0A intercalados
/// de un bitmap escpos se reensamblan exactos.
/// </summary>
public class TransportChunkingTests
{
    [Fact]
    public void Reensamblar_segmentos_reproduce_el_array_original()
    {
        var data = Enumerable.Range(0, 1000).Select(i => (byte)(i % 256)).ToArray();

        var reensamblado = TransportChunking.ChunkBuffer(data).SelectMany(s => s).ToArray();

        Assert.Equal(data, reensamblado);
    }

    [Fact]
    public void No_se_parte_por_LineFeed_con_0x0A_intercalados()
    {
        var data = new byte[700];
        for (int i = 0; i < data.Length; i++)
            data[i] = (i % 3 == 0) ? (byte)0x0A : (byte)(i % 256);

        var segmentos = TransportChunking.ChunkBuffer(data, 256).ToArray();

        Assert.Equal(data, segmentos.SelectMany(s => s).ToArray());
        // 700 / 256 -> 256 + 256 + 188, no una particion guiada por los 0x0A.
        Assert.Equal(new[] { 256, 256, 188 }, segmentos.Select(s => s.Count).ToArray());
    }

    [Fact]
    public void El_tamano_por_defecto_es_256()
    {
        Assert.Equal(256, TransportChunking.DefaultChunkSize);
        Assert.Equal(2, TransportChunking.ChunkBuffer(new byte[300]).Count());
    }

    [Fact]
    public void Los_segmentos_son_vistas_del_buffer_original_sin_copiar()
    {
        var data = new byte[10];

        var segmento = TransportChunking.ChunkBuffer(data, 4).First();

        Assert.Same(data, segmento.Array);
        Assert.Equal(0, segmento.Offset);
        Assert.Equal(4, segmento.Count);
    }

    [Fact]
    public void Array_vacio_no_produce_segmentos()
        => Assert.Empty(TransportChunking.ChunkBuffer([]));

    [Fact]
    public void Size_mayor_que_el_array_produce_un_unico_segmento()
    {
        var data = new byte[] { 1, 2, 3, 0x0A, 5 };

        var segmentos = TransportChunking.ChunkBuffer(data, 256).ToArray();

        Assert.Single(segmentos);
        Assert.Equal(data, segmentos[0].ToArray());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Size_invalido_lanza(int size)
        => Assert.Throws<ArgumentOutOfRangeException>(
            () => TransportChunking.ChunkBuffer([1, 2, 3], size).ToArray());

    [Fact]
    public void Data_null_lanza()
        => Assert.Throws<ArgumentNullException>(
            () => TransportChunking.ChunkBuffer(null!).ToArray());
}
