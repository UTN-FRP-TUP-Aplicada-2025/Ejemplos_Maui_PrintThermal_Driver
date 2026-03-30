using MotorDsl.Core.Models;

namespace MotorDsl.Core.Contracts;

/// <summary>
/// Contrato para rasterizar imágenes bitmap a formato imprimible.
/// La implementación convierte la fuente (base64, path, etc.) a bits raster.
/// Sprint 08 | TK-61
/// </summary>
public interface IBitmapRasterizer
{
    RasterizedImage Rasterize(string source, int widthPixels);
}
