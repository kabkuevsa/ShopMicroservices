using UserService.Models.DTOs;

namespace UserService.Services.Interfaces;

public interface IUserService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request);
    Task<AuthResponse?> LoginAsync(LoginRequest request);
    Task<UserDto?> GetByIdAsync(Guid id);
}