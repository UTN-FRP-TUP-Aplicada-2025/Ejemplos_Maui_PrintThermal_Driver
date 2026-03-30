using Microsoft.Extensions.Logging;
using MotorDsl.Core.Contracts;
using MotorDsl.Core.Models;
using MotorDsl.Core.Printing;
using MotorDsl.Extensions;
using MotorDsl.SampleApp.Pages;
using MotorDsl.SampleApp.Services;
using MotorDsl.SampleApp.Templates;

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
        builder.Services.AddMotorDslEngine()
            .AddTemplates(t =>
            {
                t.Add("ticket-venta", TicketDsl.Template);
            })
            .AddProfiles(p =>
            {
                p.Add(new DeviceProfile("thermal_58mm", 32, "escpos"));
            });

        // Servicios de la app
        builder.Services.AddSingleton<IPrintErrorHandler, DefaultPrintErrorHandler>();
        builder.Services.AddSingleton<IThermalPrinterService, ThermalPrinterService>();
        builder.Services.AddTransient<MainPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
