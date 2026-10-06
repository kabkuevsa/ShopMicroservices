using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using OrderService.Models.DTOs;

namespace OrderService.Clients;

public class ProductServiceClient : IProductServiceClient
{
    private readonly HttpClient _http;
    private readonly ILogger<ProductServiceClient> _logger;

    public ProductServiceClient(HttpClient http, ILogger<ProductServiceClient> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<ProductDto?> GetProductAsync(Guid productId)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var response = await _http.GetAsync($"/api/products/{productId}");
            sw.Stop();

            _logger.LogInformation("ProductService ответил за {Elapsed} мс, статус {Status}",
                sw.ElapsedMilliseconds, (int)response.StatusCode);

            if (response.StatusCode == HttpStatusCode.NotFound) return null;
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ProductDto>();
        }
        catch (HttpRequestException ex)
        {
            sw.Stop();
            _logger.LogError(ex, "ProductService недоступен (после {Elapsed} мс)", sw.ElapsedMilliseconds);
            throw new InvalidOperationException("ProductService недоступен", ex);
        }
    }

    public async Task ReserveProductAsync(Guid productId, int quantity)
    {
        var response = await _http.PostAsync(
            $"/api/products/{productId}/reserve?quantity={quantity}", null);

        if (response.StatusCode == HttpStatusCode.BadRequest)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException(error);
        }

        response.EnsureSuccessStatusCode();
    }
}