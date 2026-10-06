using OrderService.Models.DTOs;

namespace OrderService.Clients;

public interface IUserServiceClient
{
    Task<UserDto?> GetUserAsync(Guid userId);
}