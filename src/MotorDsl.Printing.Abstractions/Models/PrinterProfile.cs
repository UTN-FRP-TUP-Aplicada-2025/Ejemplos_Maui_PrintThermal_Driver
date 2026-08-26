namespace MotorDsl.Printing;

public class PrinterProfile
{
    public string Name { get; set; } = "default";
    public int LineDelayMs { get; set; } = 150;
    public int ByteDelayMs { get; set; } = 5;
    public int InitDelayMs { get; set; } = 100;
    public int FinalDelayMs { get; set; } = 500;
    public int QrDelayMs { get; set; } = 300;
    public int ImageDelayMs { get; set; } = 500;
    public int CutDelayMs { get; set; } = 500;
    public int InitCommandDelayMs { get; set; } = 300;

    /// <summary>
    /// Tope para la escritura de UN bloque. El control de flujo real lo aportan los creditos
    /// RFCOMM: cuando la impresora deja de otorgarlos, el write se bloquea. Este timeout es como
    /// se OBSERVA ese bloqueo — sin el, una impresora que deja de drenar cuelga el envio hasta que
    /// el enlace muere solo (medido: 106 s de bloqueo y ~4 min hasta la caida del ACL).
    /// Al vencer, el transport invalida la conexion y el reintento reconecta.
    /// </summary>
    public int WriteTimeoutMs { get; set; } = 10_000;

    public static PrinterProfile Thermal58mm => new()
    {
        Name = "thermal_58mm",
        LineDelayMs = 150,
        ByteDelayMs = 5,
        InitDelayMs = 100,
        FinalDelayMs = 500
    };

    // Perfil real: 58HB6-6101 (basado en self-test)
    public static PrinterProfile Real58HB6 => new()
    {
        Name = "58HB6-6101",
        LineDelayMs = 150,
        ByteDelayMs = 5,
        InitDelayMs = 100,
        FinalDelayMs = 500,
        QrDelayMs = 300,
        ImageDelayMs = 500,
        CutDelayMs = 500,
        InitCommandDelayMs = 300
    };
}
