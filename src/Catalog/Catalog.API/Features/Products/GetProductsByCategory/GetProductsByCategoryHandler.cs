namespace Catalog.API.Features.Products.GetProductsByCategory;

public record GetProductsByCategoryQuery(string Category) : IQuery<GetProductsByCategoryResult>;
public record GetProductsByCategoryResult(IEnumerable<Product> Product);

public class GetProductsByCategoryQueryHandler(IDocumentSession session) : IQueryHandler<GetProductsByCategoryQuery, GetProductsByCategoryResult>
{
    public async Task<GetProductsByCategoryResult> Handle(GetProductsByCategoryQuery request, CancellationToken cancellationToken)
    {
        var products = await session.Query<Product>()
            .Where(p => p.Category.Contains(request.Category))
            .ToListAsync(cancellationToken);

        return new GetProductsByCategoryResult(products);
    }
}
