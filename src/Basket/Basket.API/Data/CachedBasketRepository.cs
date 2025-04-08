using BuildingBlocks.Cache;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Basket.API.Data;

public class CachedBasketRepository(IBasketRepository basketRepository, IDistributedCache cache, CacheSettings settings) : IBasketRepository
{
    private const string ShoppingCart = "shoppingcart:";
    public async Task<bool> DeleteBasket(string userName, CancellationToken cancellationToken = default)
    {
        await basketRepository.DeleteBasket($"{ShoppingCart}{userName}", cancellationToken);
        await cache.RemoveAsync(userName, cancellationToken);
        return true;
    }

    public async Task<ShoppingCart> GetBasket(string userName, CancellationToken cancellationToken = default)
    {
        var cachedBasket = await cache.GetStringAsync($"{ShoppingCart}{userName}", cancellationToken);
        if (!string.IsNullOrEmpty(cachedBasket))
        {
            return JsonSerializer.Deserialize<ShoppingCart>(cachedBasket)!;
        }

        var shoppingCartEntity = await basketRepository.GetBasket(userName, cancellationToken);
        await cache.SetStringAsync($"{ShoppingCart}{userName}", JsonSerializer.Serialize(shoppingCartEntity), new() { AbsoluteExpirationRelativeToNow = settings.Duration }, cancellationToken);
        return shoppingCartEntity;
    }

    public async Task<ShoppingCart> StoreBasket(ShoppingCart cart, CancellationToken cancellationToken = default)
    {
        await basketRepository.StoreBasket(cart, cancellationToken);
        await cache.SetStringAsync($"{ShoppingCart}{cart.UserName}", JsonSerializer.Serialize(cart), new() { AbsoluteExpirationRelativeToNow = settings.Duration }, cancellationToken);
        return cart;
    }


}
