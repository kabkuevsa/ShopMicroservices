using Microsoft.EntityFrameworkCore;
using ProductService.Data;
using ProductService.Models.DTOs;
using ProductService.Models.Entities;
using ProductService.Services.Interfaces;

namespace ProductService.Services.Implementations;

public class ProductService : IProductService
{
    private readonly AppDbContext _db;

    public ProductService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<ProductDto>> GetAllAsync(string? category, decimal? maxPrice)
    {
        var query = _db.Products.AsQueryable();

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(p => p.Category == category);

        if (maxPrice.HasValue)
            query = query.Where(p => p.Price <= maxPrice.Value);

        return await query
            .OrderBy(p => p.Name)
            .Select(p => ToDto(p))
            .ToListAsync();
    }

    public async Task<ProductDto?> GetByIdAsync(Guid id)
    {
        var product = await _db.Products.FindAsync(id);
        return product is null ? null : ToDto(product);
    }

    public async Task<ProductDto> CreateAsync(CreateProductRequest r)
    {
        var product = new Product
        {
            Name = r.Name,
            Description = r.Description,
            Category = r.Category,
            Price = r.Price,
            Stock = r.Stock
        };

        _db.Products.Add(product);
        await _db.SaveChangesAsync();
        return ToDto(product);
    }

    public async Task<bool> UpdateAsync(Guid id, CreateProductRequest r)
    {
        var product = await _db.Products.FindAsync(id);
        if (product is null) return false;

        product.Name = r.Name;
        product.Description = r.Description;
        product.Category = r.Category;
        product.Price = r.Price;
        product.Stock = r.Stock;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var product = await _db.Products.FindAsync(id);
        if (product is null) return false;

        _db.Products.Remove(product);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ReserveAsync(Guid id, int quantity)
    {
        var product = await _db.Products.FindAsync(id);
        if (product is null) return false;
        if (product.Stock < quantity)
            throw new InvalidOperationException("Недостаточно товара на складе");

        product.Stock -= quantity;
        await _db.SaveChangesAsync();
        return true;
    }

    private static ProductDto ToDto(Product p) => new()
    {
        Id = p.Id,
        Name = p.Name,
        Description = p.Description,
        Category = p.Category,
        Price = p.Price,
        Stock = p.Stock,
        CreatedAt = p.CreatedAt
    };
}