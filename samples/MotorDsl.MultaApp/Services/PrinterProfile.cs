namespace MotorDsl.MultaApp.Services;

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

    public static PrinterProfile Thermal58mm => new()
    {
        Name = "thermal_58mm",
        LineDelayMs = 150,
        ByteDelayMs = 5,
        InitDelayMs = 100,
        FinalDelayMs = 500
    };
}
