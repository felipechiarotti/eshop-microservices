using Discount.Grpc.Data;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Data;

public static class Extensions
{
    public static IApplicationBuilder UseMigration(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DiscountContext>();

        var pendingMigrations = dbContext.Database.GetPendingMigrations();
        if (pendingMigrations.Any())
            dbContext.Database.MigrateAsync();
        return app;
    }
}
