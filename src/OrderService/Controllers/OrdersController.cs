using Microsoft.AspNetCore.Mvc;
using OrderService.Models.DTOs;
using OrderService.Services.Interfaces;

namespace OrderService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orders;

    public OrdersController(IOrderService orders)
    {
        _orders = orders;
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create(CreateOrderRequest request)
    {
        try
        {
            var order = await _orders.CreateAsync(request);
            return Ok(order);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetAll()
    {
        return Ok(await _orders.GetAllAsync());
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderDto>> GetById(Guid id)
    {
        try
        {
            var order = await _orders.GetByIdAsync(id);
            return order is null ? NotFound() : Ok(order);
        }
        catch (InvalidOperationException ex)
        {
            return StatusCode(503, new { error = ex.Message });
        }
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        try
        {
            var ok = await _orders.CancelAsync(id);
            return ok ? NoContent() : NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}