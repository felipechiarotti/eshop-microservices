using BuildingBlocks.Util;
using Marten.Schema;
using System.Reflection;
using System.Text.Json;

namespace Catalog.API.Data;

public class CatalogInitialData(ILogger<CatalogInitialData> logger) : IInitialData
{
    public async Task Populate(IDocumentStore store, CancellationToken cancellation)
    {
        try
        {
            using var session = store.LightweightSession();

            if (await session.Query<Product>().AnyAsync())
                return;
            var productsJson = await Assembly.GetExecutingAssembly()
                .ReadEmbeddedFileAsync("Products.json");

            var products = JsonSerializer.Deserialize<List<Product>>(productsJson);
            session.Store<Product>(products);
            await session.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error populating initial data {Entity}", nameof(Product));
        }
    }
}
