using BuildingBlocks.Behaviors;
using BuildingBlocks.Exceptions.Handler;
using BuildingBlocks.Logging;
using Carter;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpLogging;
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

    public static IApplicationBuilder UseCore(this IApplicationBuilder app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseHttpLogging();
        app.UseExceptionHandler(opt => { });
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
        services.AddValidatorsFromAssembly(Assembly.GetEntryAssembly()!)
            .AddFluentValidationAutoValidation();
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
}
