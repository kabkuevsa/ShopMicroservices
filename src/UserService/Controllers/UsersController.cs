using Microsoft.AspNetCore.Mvc;
using UserService.Models.DTOs;
using UserService.Services.Interfaces;

namespace UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _users;

    public UsersController(IUserService users)
    {
        _users = users;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        try
        {
            var result = await _users.RegisterAsync(request);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var result = await _users.LoginAsync(request);
        return result is null
            ? Unauthorized(new { error = "Неверный email или пароль" })
            : Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserDto>> GetById(Guid id)
    {
        var user = await _users.GetByIdAsync(id);
        return user is null ? NotFound() : Ok(user);
    }
}