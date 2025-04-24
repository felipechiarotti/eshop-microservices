using BuildingBlocks;
using Microsoft.Extensions.DependencyInjection;

namespace Ordering.Application;
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        services.AddMediatr();


        return services;
    }
}
