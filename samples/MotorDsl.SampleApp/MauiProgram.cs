using Microsoft.Extensions.Logging;
using MotorDsl.Extensions;
using MotorDsl.SampleApp.Pages;
using MotorDsl.SampleApp.Services;

namespace MotorDsl.SampleApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // Motor DSL: Parser, Evaluator, LayoutEngine, RendererRegistry, DocumentEngine
        builder.Services.AddMotorDslEngine();

        // Servicios de la app
        builder.Services.AddSingleton<IThermalPrinterService, ThermalPrinterService>();
        builder.Services.AddTransient<MainPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
