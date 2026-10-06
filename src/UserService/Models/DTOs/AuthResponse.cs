namespace UserService.Models.DTOs;

public class AuthResponse
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
}