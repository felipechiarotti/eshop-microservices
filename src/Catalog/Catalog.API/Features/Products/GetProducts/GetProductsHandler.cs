﻿
namespace Catalog.API.Features.Products.GetProducts;

public record GetProductsQuery() : IQuery<GetProductsResult>;
public record GetProductsResult(IEnumerable<Product> Products);

public class GetProductsQueryHandler(IDocumentSession session) : IQueryHandler<GetProductsQuery, GetProductsResult>
{
    public async Task<GetProductsResult> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await session.Query<Product>().ToListAsync(cancellationToken);
        return new GetProductsResult(products);
    }
}
