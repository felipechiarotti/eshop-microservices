using BuildingBlocks;
using BuildingBlocks.Exceptions.Handler;
using Carter;

namespace Ordering.API;

public static class DependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCarterWithAssemblies();
        services.AddSwagger();
        services.AddHttpLogs();
        services.AddCustomHealthCheck(configuration);
        services.AddExceptionHandler<CustomExceptionHandler>();
        return services;
    }

    public static IApplicationBuilder UseApi(this WebApplication app)
    {
        app.MapCarter();
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseHttpLogging();
        app.UseCustomHealthCheck();
        app.UseExceptionHandler(opt => { });
        return app;
    }
}
