using Microsoft.AspNetCore.Mvc;
using ProductService.Models.DTOs;
using ProductService.Services.Interfaces;

namespace ProductService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _products;

    public ProductsController(IProductService products)
    {
        _products = products;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll(
        [FromQuery] string? category,
        [FromQuery] decimal? maxPrice)
    {
        return Ok(await _products.GetAllAsync(category, maxPrice));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductDto>> GetById(Guid id)
    {
        var product = await _products.GetByIdAsync(id);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create(CreateProductRequest request)
    {
        var created = await _products.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, CreateProductRequest request)
    {
        var ok = await _products.UpdateAsync(id, request);
        return ok ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var ok = await _products.DeleteAsync(id);
        return ok ? NoContent() : NotFound();
    }

    [HttpPost("{id:guid}/reserve")]
    public async Task<IActionResult> Reserve(Guid id, [FromQuery] int quantity)
    {
        try
        {
            var ok = await _products.ReserveAsync(id, quantity);
            return ok ? NoContent() : NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}