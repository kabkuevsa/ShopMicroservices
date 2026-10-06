using OrderService.Models.DTOs;

namespace OrderService.Services.Interfaces;

public interface IOrderService
{
    Task<OrderDto> CreateAsync(CreateOrderRequest request);
    Task<IEnumerable<OrderDto>> GetAllAsync();
    Task<OrderDto?> GetByIdAsync(Guid id);
    Task<bool> CancelAsync(Guid id);
}