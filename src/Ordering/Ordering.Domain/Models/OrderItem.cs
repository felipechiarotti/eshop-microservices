namespace Ordering.Domain.Models;
public class OrderItem : Entity<OrderItemId>
{
    public OrderItem(OrderId orderId, ProductId productId, decimal price, int quantity)
    {
        Id = OrderItemId.Of(Guid.NewGuid());
        OrderId = orderId;
        ProductId = productId;
        Price = price;
        Quantity = quantity;
    }

    public OrderId OrderId { get; set; }
    public ProductId ProductId { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }


}
