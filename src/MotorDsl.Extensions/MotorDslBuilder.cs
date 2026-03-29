using Microsoft.Extensions.DependencyInjection;
using MotorDsl.Core.Contracts;
using MotorDsl.Core.Providers;

namespace MotorDsl.Extensions;

/// <summary>
/// Fluent builder for configuring MotorDsl services.
/// Sprint 05 — TK-30
/// </summary>
public class MotorDslBuilder
{
    public IServiceCollection Services { get; }

    internal MotorDslBuilder(IServiceCollection services)
    {
        Services = services;
    }

    public MotorDslBuilder AddTemplates(Action<InMemoryTemplateProvider> configure)
    {
        var provider = new InMemoryTemplateProvider();
        configure(provider);
        Services.AddSingleton<ITemplateProvider>(provider);
        return this;
    }

    public MotorDslBuilder AddProfiles(Action<InMemoryDeviceProfileProvider> configure)
    {
        var provider = new InMemoryDeviceProfileProvider();
        configure(provider);
        Services.AddSingleton<IDeviceProfileProvider>(provider);
        return this;
    }
}
