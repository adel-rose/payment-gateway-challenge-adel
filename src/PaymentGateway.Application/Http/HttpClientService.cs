using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using PaymentGateway.Application.Interfaces;

namespace PaymentGateway.Application.Http;

public class HttpClientService : IHttpClientService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<HttpClientService> _logger;

    public HttpClientService(HttpClient httpClient, ILogger<HttpClientService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
    
    public async Task<HttpResponseWrapper<TResponse>> PostAsync<TRequest, TResponse>(string url, TRequest request, CancellationToken cancellationToken = default)
    {
        // Must convert request data into json format before sending over a network
        
        var jsonContent = JsonSerializer.Serialize(request);
        
        // Building response body & header info
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        // Sending the post request
        var httpResponse = await _httpClient.PostAsync(url, content, cancellationToken);
        
        // Retrieve response data as json
        var responseBody = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
        
        return new HttpResponseWrapper<TResponse>()
        {
            IsSuccess = httpResponse.IsSuccessStatusCode,
            Status = httpResponse.StatusCode,
            Response = JsonSerializer.Deserialize<TResponse>(responseBody)
        };
    }
}