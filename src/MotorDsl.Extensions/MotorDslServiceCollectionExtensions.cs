using Microsoft.Extensions.DependencyInjection;
using MotorDsl.Core.Contracts;
using MotorDsl.Core.Engine;
using MotorDsl.Core.Evaluators;
using MotorDsl.Core.Layout;
using MotorDsl.Core.Validation;
using MotorDsl.Parser;
using MotorDsl.Rendering;

namespace MotorDsl.Extensions;

public static class MotorDslServiceCollectionExtensions
{
    public static MotorDslBuilder AddMotorDslEngine(this IServiceCollection services)
    {
        services.AddSingleton<IDslParser, DslParser>();
        services.AddSingleton<IEvaluator, Evaluator>();
        services.AddSingleton<ILayoutEngine, LayoutEngine>();
        services.AddSingleton<IRendererRegistry>(sp =>
        {
            var registry = new RendererRegistry();
            registry.Register(new TextRenderer());
            registry.Register(new EscPosRenderer());
            return registry;
        });
        services.AddSingleton<IDataValidator, DataValidator>();
        services.AddSingleton<ITemplateValidator, TemplateValidator>();
        services.AddSingleton<IProfileValidator, ProfileValidator>();
        services.AddSingleton<IDocumentEngine, DocumentEngine>();
        return new MotorDslBuilder(services);
    }
}
