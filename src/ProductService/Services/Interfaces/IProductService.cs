using ProductService.Models.DTOs;

namespace ProductService.Services.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllAsync(string? category, decimal? maxPrice);
    Task<ProductDto?> GetByIdAsync(Guid id);
    Task<ProductDto> CreateAsync(CreateProductRequest request);
    Task<bool> UpdateAsync(Guid id, CreateProductRequest request);
    Task<bool> DeleteAsync(Guid id);
    Task<bool> ReserveAsync(Guid id, int quantity);
}