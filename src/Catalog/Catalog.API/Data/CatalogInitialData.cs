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
            var products = await Assembly.GetExecutingAssembly()
                .ReadEmbeddedFileAsync<IEnumerable<Product>>("Products.json");

            session.Store(products!);
            await session.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error populating initial data {Entity}", nameof(Product));
        }
    }
}
