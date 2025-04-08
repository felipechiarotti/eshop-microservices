namespace Basket.API.Features.DeleteBasket;

public record DeleteBasketResponse(string UserName);
public class DeleteBasketEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/basket/{userName}", async (string userName, ISender sender) =>
        {
            var result = await sender.Send(new DeleteBasketCommand(userName));
            var response = result.Adapt<DeleteBasketResponse>();
            return Results.Ok(response);
        })
       .WithName("DeleteBasket")
       .Produces<DeleteBasketEndpoint>(StatusCodes.Status200OK)
       .ProducesProblem(StatusCodes.Status400BadRequest)
       .WithSummary("Delete Basket")
       .WithDescription("Delete Basket");
    }
}
