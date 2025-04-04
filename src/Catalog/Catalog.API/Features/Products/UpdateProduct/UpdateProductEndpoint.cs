namespace Catalog.API.Features.Products.UpdateProduct;

public record UpdateProductRequest(Guid Id, IEnumerable<string> Category, string Name, string Description, decimal Price, string ImageUrl);
public record UpdateProductResponse(bool IsSuccess);
internal class UpdateProductEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPatch("/products", async (UpdateProductRequest request, ISender sender) =>
        {
            var result = await sender.Send(request.Adapt<UpdateProductCommand>());
            if (!result.IsSuccess)
                return Results.NotFound();

            var response = result.Adapt<UpdateProductResponse>();
            return Results.Ok(response);
        })
        .WithName("UpdateProduct")
        .Produces<UpdateProductResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .WithSummary("Update Product")
        .WithDescription("Update Product");
    }
}