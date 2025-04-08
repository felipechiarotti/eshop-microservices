using BuildingBlocks.Behaviors;
using BuildingBlocks.Cache;
using BuildingBlocks.Exceptions.Handler;
using BuildingBlocks.Logging;
using Carter;
using FluentValidation;
using HealthChecks.UI.Client;
using Marten;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Reflection;

namespace BuildingBlocks;
public static class DependencyInjectionExtensions
{

    public static IHostApplicationBuilder AddCore(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog(SeriLogger.Configure);
        builder.Services.AddMediatr();
        builder.Services.AddSwagger();
        builder.Services.AddCustomHealthCheck(builder.Configuration);
        builder.Services.AddCarterWithAssemblies();
        builder.Services.AddHttpLogging(options =>
        {
            options.CombineLogs = true;
            options.LoggingFields =
                HttpLoggingFields.RequestQuery
                | HttpLoggingFields.RequestMethod
                | HttpLoggingFields.RequestPath
                | HttpLoggingFields.RequestBody
                | HttpLoggingFields.ResponseStatusCode
                | HttpLoggingFields.ResponseBody
                | HttpLoggingFields.Duration;
        });
        builder.Services.AddExceptionHandler<CustomExceptionHandler>();
        return builder;
    }

    public static IApplicationBuilder UseCore(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseHttpLogging();
        app.UseExceptionHandler(opt => { });
        app.UseCustomHealthCheck();
        app.MapCarter();
        return app;
    }
    public static IServiceCollection AddMediatr(this IServiceCollection services)
    {
        // Add your building blocks dependencies here
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(Assembly.GetEntryAssembly()!);
            config.AddOpenBehavior(typeof(LoggingBehavior<,>));
            config.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });
        services.AddValidatorsFromAssembly(Assembly.GetEntryAssembly()!);
        return services;
    }

    public static IServiceCollection AddSwagger(this IServiceCollection services, string description = "")
    {
        // Add your building blocks dependencies here
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = Assembly.GetEntryAssembly()!.FullName,
                Version = "v1",
                Description = description
            });
        });
        return services;
    }

    public static IServiceCollection AddMarten(this IServiceCollection services, IConfiguration configuration, Action<StoreOptions>? customOptions = null)
    {
        services.AddMarten(options =>
        {
            options.Connection(configuration.GetConnectionString("Database")!);
            if (customOptions is not null)
            {
                customOptions(options);
            }
        }).UseLightweightSessions();
        return services;
    }
    public static IServiceCollection AddCarterWithAssemblies(this IServiceCollection services)
    {
        services.AddCarter(configurator: config =>
        {

            var modules = Assembly.GetEntryAssembly()!.GetTypes()
            .Where(t => t.IsAssignableTo(typeof(ICarterModule))).ToArray();

            config.WithModules(modules);

        });
        return services;
    }

    public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConnectionString = configuration.GetConnectionString("Redis");
        if (string.IsNullOrEmpty(redisConnectionString))
            return services;

        var ttlSection = configuration.GetSection("Cache");
        var cacheSettings =
            ttlSection.Exists() ? ttlSection.Get<CacheSettings>()
                                : new CacheSettings { Duration = TimeSpan.FromMinutes(10) };

        services.AddSingleton(cacheSettings!);

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnectionString;
            options.InstanceName = Assembly.GetEntryAssembly()!.GetName().Name!.ToLower().Replace(".", "") + ":";
        });
        return services;
    }

    public static IServiceCollection AddCustomHealthCheck(this IServiceCollection services, IConfiguration configuration)
    {
        // Add your building blocks dependencies here
        var healthCheck = services.AddHealthChecks();
        var pgConnection = configuration.GetConnectionString("Database");
        var redisConnection = configuration.GetConnectionString("Redis");

        if (!string.IsNullOrEmpty(pgConnection))
            healthCheck.AddNpgSql(configuration.GetConnectionString("Database")!);

        if (!string.IsNullOrEmpty(redisConnection))
            healthCheck.AddRedis(redisConnection);

        return services;
    }

    public static IApplicationBuilder UseCustomHealthCheck(this IApplicationBuilder app)
    {
        // Add your building blocks dependencies here
        app.UseHealthChecks("/health",
            new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
        return app;
    }
}
