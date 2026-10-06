namespace OrderService.Models.Entities;

public class Order
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = "Pending";   // Pending | Paid | Cancelled
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}