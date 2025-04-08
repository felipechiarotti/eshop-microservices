namespace Basket.API.Models;

public class ShoppingCart
{
    public ShoppingCart() { }
    public ShoppingCart(string userName)
    {
        UserName = userName;
    }
    public string UserName { get; set; } = default!;
    public List<ShoppingCartItem> Items { get; set; } = new();
    public decimal TotalPrice => Items.Sum(x => x.Price * x.Quantity);
    public int TotalItems => Items.Count;
}
