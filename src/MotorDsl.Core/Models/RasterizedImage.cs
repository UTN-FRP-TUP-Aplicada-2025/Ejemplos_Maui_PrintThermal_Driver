namespace MotorDsl.Core.Models;

/// <summary>
/// Resultado de rasterizar una imagen bitmap.
/// Contiene los bits en formato raster listo para ESC/POS GS v 0.
/// Sprint 08 | TK-61
/// </summary>
public class RasterizedImage
{
    public byte[] Bits { get; }
    public int WidthBytes { get; }
    public int HeightDots { get; }

    public RasterizedImage(byte[] bits, int widthBytes, int heightDots)
    {
        Bits = bits;
        WidthBytes = widthBytes;
        HeightDots = heightDots;
    }
}
