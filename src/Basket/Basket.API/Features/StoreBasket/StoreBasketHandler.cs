using Discount.Grpc;

namespace Basket.API.Features.StoreBasket;

public record StoreBasketCommand(ShoppingCart Cart) : ICommand<StoreBasketResult>;
public record StoreBasketResult(string UserName);

public class StoreBasketCommandValidator : AbstractValidator<StoreBasketCommand>
{
    public StoreBasketCommandValidator()
    {
        RuleFor(x => x.Cart).NotNull().WithMessage("Cart can not be null");
        RuleFor(x => x.Cart.UserName).NotEmpty().WithMessage("UserName is required");
    }
}
public class StoreBasketCommandHandler(IBasketRepository basketRepository, DiscountProtoService.DiscountProtoServiceClient discountService) : ICommandHandler<StoreBasketCommand, StoreBasketResult>
{
    public async Task<StoreBasketResult> Handle(StoreBasketCommand request, CancellationToken cancellationToken)
    {
        await GetAndApplyDiscounts(request.Cart.Items, cancellationToken);
        var basket = await basketRepository.StoreBasket(request.Cart, cancellationToken);
        return new StoreBasketResult(basket.UserName);
    }

    private async Task GetAndApplyDiscounts(IEnumerable<ShoppingCartItem> items, CancellationToken cancellationToken)
    {
        foreach (var item in items)
        {
            var coupon = await discountService.GetDiscountAsync(new GetDiscountRequest
            {
                ProductName = item.ProductName
            }, cancellationToken: cancellationToken);
            item.Price -= coupon.Amount;
        }
    }
}
