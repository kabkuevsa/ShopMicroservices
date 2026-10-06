using Microsoft.EntityFrameworkCore;
using UserService.Data;
using UserService.Models.DTOs;
using UserService.Models.Entities;
using UserService.Services.Interfaces;

namespace UserService.Services.Implementations;

public class UserService : IUserService
{
    private readonly AppDbContext _db;

    public UserService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        if (await _db.Users.AnyAsync(u => u.Email == request.Email))
            throw new InvalidOperationException("Пользователь с таким email уже существует");

        var user = new User
        {
            Email = request.Email,
            FullName = request.FullName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return new AuthResponse
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName
        };
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user is null) return null;

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return null;

        return new AuthResponse
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName
        };
    }

    public async Task<UserDto?> GetByIdAsync(Guid id)
    {
        var user = await _db.Users.FindAsync(id);
        return user is null ? null : new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            CreatedAt = user.CreatedAt
        };
    }
}