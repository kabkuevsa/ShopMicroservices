namespace OrderService.Models.DTOs;

public class CreateOrderRequest
{
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}