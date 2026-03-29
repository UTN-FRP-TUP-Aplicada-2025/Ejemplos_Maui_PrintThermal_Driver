namespace MotorDsl.SampleApp.Services;

public class PrinterProfile
{
    public string Name { get; set; } = "default";

    // Delay entre líneas (ms base)
    public int LineDelayMs { get; set; } = 150;

    // Delay adicional por byte en la línea
    public int ByteDelayMs { get; set; } = 5;

    // Delay antes de empezar a enviar
    public int InitDelayMs { get; set; } = 100;

    // Delay final después del último comando
    public int FinalDelayMs { get; set; } = 500;

    // Delay para comandos especiales
    public int QrDelayMs { get; set; } = 300;
    public int ImageDelayMs { get; set; } = 500;
    public int CutDelayMs { get; set; } = 500;
    public int InitCommandDelayMs { get; set; } = 300;

    // Perfiles predefinidos
    public static PrinterProfile Thermal58mm => new()
    {
        Name = "thermal_58mm",
        LineDelayMs = 150,
        ByteDelayMs = 5,
        InitDelayMs = 100,
        FinalDelayMs = 500
    };

    public static PrinterProfile Thermal80mm => new()
    {
        Name = "thermal_80mm",
        LineDelayMs = 100,
        ByteDelayMs = 3,
        InitDelayMs = 50,
        FinalDelayMs = 300
    };

    public static PrinterProfile Fast => new()
    {
        Name = "fast",
        LineDelayMs = 50,
        ByteDelayMs = 1,
        InitDelayMs = 50,
        FinalDelayMs = 200
    };
}
