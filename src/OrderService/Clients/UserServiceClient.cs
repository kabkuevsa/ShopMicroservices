using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using OrderService.Models.DTOs;

namespace OrderService.Clients;

public class UserServiceClient : IUserServiceClient
{
    private readonly HttpClient _http;
    private readonly ILogger<UserServiceClient> _logger;

    public UserServiceClient(HttpClient http, ILogger<UserServiceClient> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<UserDto?> GetUserAsync(Guid userId)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var response = await _http.GetAsync($"/api/users/{userId}");
            sw.Stop();

            _logger.LogInformation("UserService ответил за {Elapsed} мс, статус {Status}",
                sw.ElapsedMilliseconds, (int)response.StatusCode);

            if (response.StatusCode == HttpStatusCode.NotFound) return null;
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<UserDto>();
        }
        catch (HttpRequestException ex)
        {
            sw.Stop();
            _logger.LogError(ex, "UserService недоступен (после {Elapsed} мс)", sw.ElapsedMilliseconds);
            throw new InvalidOperationException("UserService недоступен", ex);
        }
    }
}