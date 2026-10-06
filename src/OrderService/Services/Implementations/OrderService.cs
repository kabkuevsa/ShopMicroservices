using Microsoft.EntityFrameworkCore;
using OrderService.Clients;
using OrderService.Data;
using OrderService.Models.DTOs;
using OrderService.Models.Entities;
using OrderService.Services.Interfaces;

namespace OrderService.Services.Implementations;

public class OrderService : IOrderService
{
    private readonly AppDbContext _db;
    private readonly IUserServiceClient _userClient;
    private readonly IProductServiceClient _productClient;

    public OrderService(AppDbContext db, IUserServiceClient userClient, IProductServiceClient productClient)
    {
        _db = db;
        _userClient = userClient;
        _productClient = productClient;
    }

    public async Task<OrderDto> CreateAsync(CreateOrderRequest r)
    {
        // 1. Проверяем пользователя
        var user = await _userClient.GetUserAsync(r.UserId)
            ?? throw new InvalidOperationException("Пользователь не найден");

        // 2. Проверяем товар
        var product = await _productClient.GetProductAsync(r.ProductId)
            ?? throw new InvalidOperationException("Товар не найден");

        if (product.Stock < r.Quantity)
            throw new InvalidOperationException("Недостаточно товара на складе");

        // 3. Резервируем товар
        await _productClient.ReserveProductAsync(r.ProductId, r.Quantity);

        // 4. Сохраняем заказ
        var order = new Order
        {
            UserId = r.UserId,
            ProductId = r.ProductId,
            Quantity = r.Quantity,
            TotalPrice = product.Price * r.Quantity,
            Status = "Pending"
        };

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();

        return new OrderDto
        {
            Id = order.Id,
            UserId = order.UserId,
            ProductId = order.ProductId,
            Quantity = order.Quantity,
            TotalPrice = order.TotalPrice,
            Status = order.Status,
            CreatedAt = order.CreatedAt,
            User = user,
            Product = product
        };
    }

    public async Task<IEnumerable<OrderDto>> GetAllAsync()
    {
        return await _db.Orders
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new OrderDto
            {
                Id = o.Id,
                UserId = o.UserId,
                ProductId = o.ProductId,
                Quantity = o.Quantity,
                TotalPrice = o.TotalPrice,
                Status = o.Status,
                CreatedAt = o.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<OrderDto?> GetByIdAsync(Guid id)
    {
        var order = await _db.Orders.FindAsync(id);
        if (order is null) return null;

        var user = await _userClient.GetUserAsync(order.UserId);
        var product = await _productClient.GetProductAsync(order.ProductId);

        return new OrderDto
        {
            Id = order.Id,
            UserId = order.UserId,
            ProductId = order.ProductId,
            Quantity = order.Quantity,
            TotalPrice = order.TotalPrice,
            Status = order.Status,
            CreatedAt = order.CreatedAt,
            User = user,
            Product = product
        };
    }

    public async Task<bool> CancelAsync(Guid id)
    {
        var order = await _db.Orders.FindAsync(id);
        if (order is null) return false;
        if (order.Status == "Cancelled")
            throw new InvalidOperationException("Заказ уже отменён");

        order.Status = "Cancelled";
        await _db.SaveChangesAsync();
        return true;
    }
}