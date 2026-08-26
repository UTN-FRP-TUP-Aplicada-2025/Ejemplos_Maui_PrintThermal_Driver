using MotorDsl.Bluetooth;
using MotorDsl.Core.Models;

namespace MotorDsl.Tests;

/// <summary>
/// Cubre la parte pura del manejo de status: parseo del byte DLE EOT n=1 a PrinterStatus y el
/// mapeo de PrinterHardwareException a PrintErrorType.Hardware.
/// <para>
/// El latch de degradacion del pacing (NextPacingDecision) se elimino junto con el sondeo por
/// bloque: ese sondeo intercalaba DLE EOT dentro de la carga util de un GS v 0 y destruia el
/// raster en firmwares que no filtran comandos de tiempo real. Ver 13-Impresion-Logo.
/// </para>
/// </summary>
public class StatusFlowControlTests
{
    // ── ParseStatusByte ──

    [Theory]
    [InlineData(0x12)] // bits fijos validos, online (bit3=0)
    [InlineData(0x16)]
    public void ParseStatusByte_ValidOnline_IsReady(int b)
        => Assert.Equal(PrinterStatus.Ready, BluetoothPrinterTransport.ParseStatusByte((byte)b));

    [Theory]
    [InlineData(0x1A)] // 0x12 | 0x08 (offline) -> Busy
    [InlineData(0x1E)] // bits fijos validos + offline
    public void ParseStatusByte_Offline_IsBusy(int b)
        => Assert.Equal(PrinterStatus.Busy, BluetoothPrinterTransport.ParseStatusByte((byte)b));

    [Theory]
    [InlineData(0x00)] // bits fijos invalidos -> basura/eco
    [InlineData(0xFF)]
    public void ParseStatusByte_InvalidFixedBits_IsUnknown(int b)
        => Assert.Equal(PrinterStatus.Unknown, BluetoothPrinterTransport.ParseStatusByte((byte)b));

    [Fact]
    public void ParseStatusByte_Null_Timeout_IsUnknown()
        => Assert.Equal(PrinterStatus.Unknown, BluetoothPrinterTransport.ParseStatusByte(null));

    // ── Mapeo de error de hardware ──

    [Fact]
    public void PrintError_FromException_MapsPrinterHardwareException_ToHardware()
    {
        var ex = new PrinterHardwareException("paper out");
        var error = PrintError.FromException(ex, attempt: 1, maxAttempts: 3);

        Assert.Equal(PrintErrorType.Hardware, error.Type);
        Assert.Contains("paper out", error.Message);
        Assert.Same(ex, error.InnerException);
    }

    [Fact]
    public async Task DefaultPrintErrorHandler_DoesNotRetry_OnHardware()
    {
        var handler = new MotorDsl.Core.Printing.DefaultPrintErrorHandler();
        var error = PrintError.FromException(new PrinterHardwareException("cover open"), 1, 3);

        var shouldRetry = await handler.HandleErrorAsync(error);

        Assert.False(shouldRetry); // sin papel / tapa abierta: no tiene sentido reintentar
    }
}
