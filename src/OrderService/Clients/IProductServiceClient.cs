using OrderService.Models.DTOs;

namespace OrderService.Clients;

public interface IProductServiceClient
{
    Task<ProductDto?> GetProductAsync(Guid productId);
    Task ReserveProductAsync(Guid productId, int quantity);
}