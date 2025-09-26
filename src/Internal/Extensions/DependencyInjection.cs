using System.Reflection;
using Common.Extensions;
using Common.Services.Abstract;
using Common.Services.Instrumentation;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Internal.Extensions;

internal static class DependencyInjection
{
    public static IServiceCollection AddInternalServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions()
                .AddCommonServices(configuration);

        return services;
    }

    public static IHostApplicationBuilder ConfigureApiOptions(this IHostApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.DescribeAllParametersInCamelCase();
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Vanguard Payment Service",
                Version = "v1"
            });

            options.IncludeXmlComments(Path.Combine(
                AppContext.BaseDirectory,
                $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));


            if (!builder.Environment.IsDevelopment())
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.ApiKey,
                    Name = "Authorization",
                    BearerFormat = "JWT",
                    Scheme = "Bearer",
                    Description = "JWT Authorization header using Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and your token below.\r\n\r\n e.g: \"Bearer xyztoken\"",
                    In = ParameterLocation.Header,
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            }
        });

        return builder;
    }

    public static IHostApplicationBuilder ConfigureOpenTelemetry(this IHostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder, nameof(builder));

        var configuration = builder.Configuration;
        var resourceBuilder = ResourceBuilder.CreateDefault()
                                             .AddService("vanguard", serviceInstanceId: $"vanguard-{Environment.MachineName}")
                                             .AddTelemetrySdk();

        builder.Logging.AddConfiguration(configuration.GetSection("Logging"));
        builder.Logging.AddOpenTelemetry(options =>
        {
            options.SetResourceBuilder(resourceBuilder);
            options.IncludeFormattedMessage = true;
            options.IncludeScopes = true;
            options.ParseStateValues = true;
            options.AddOtlpExporter(otlpOptions =>
            {
                otlpOptions.Headers = $"x-otlp-api-key={configuration["Otlp:ApiKey"]}";
                otlpOptions.Protocol = OtlpExportProtocol.Grpc;
                otlpOptions.Endpoint = new Uri(configuration["Otlp:Endpoint"]);
            });
        });                        

        builder.Services
            .AddMetrics()
            .AddSingleton<IMeter, OpenTelemetryMeter>()
            .AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics.SetResourceBuilder(resourceBuilder)
                    .AddRuntimeInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddProcessInstrumentation()
                    .AddMeter(configuration["ServiceName"])
                    .AddOtlpExporter(otlpOptions =>
                    {
                        otlpOptions.Headers = $"x-otlp-api-key={configuration["Otlp:ApiKey"]}";
                        otlpOptions.Protocol = OtlpExportProtocol.Grpc;
                        otlpOptions.Endpoint = new Uri(configuration["Otlp:Endpoint"]);
                    });
            });

        return builder;
    }
}
