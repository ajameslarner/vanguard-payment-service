using System.Diagnostics.CodeAnalysis;
using Common.Factories;
using Common.Factories.Abstract;
using Common.Services;
using Common.Services.Abstract;
using Common.Services.Instrumentation;
using Common.Strategies;
using Common.Strategies.Abstract;
using Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Extensions;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static IServiceCollection AddCommonServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddBaseServices(configuration)
                .AddScoped<IPaymentStrategy, BacsPaymentStrategy>()
                .AddScoped<IPaymentStrategy, FasterPaymentsStrategy>()
                .AddScoped<IPaymentStrategy, ChapsPaymentStrategy>()
                .AddScoped<IAccountService, AccountService>()
                .AddScoped<IPaymentService, PaymentService>()
                .AddScoped<IMeter, OpenTelemetryMeter>()
                .AddTransient<IPaymentStrategyFactory, PaymentStrategyFactory>();

        return services;
    }

    private static IServiceCollection AddBaseServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(opt => opt.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()))
                .AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies())
                .AddInfrastructure(configuration);

        return services;
    }
}
