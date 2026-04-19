using Shouldly;

namespace PaymentGateway.IntegrationTests;

public class HealthCheckTest : IClassFixture<PaymentGatewayApiWebAppFactory>
{
    
    private HttpClient _httpClient;

    public HealthCheckTest(PaymentGatewayApiWebAppFactory factory)
    {
        _httpClient = factory.CreateClient();
    }

    /// <summary>
    /// Tests actual Database and Mocked bank connectivity
    /// </summary>
    [Fact]
    public async Task Api_Must_Be_Healthy()
    {
        var response = await _httpClient.GetAsync("/health");
        response.EnsureSuccessStatusCode();
        
        var stringResponse = await response.Content.ReadAsStringAsync();
        stringResponse.ShouldBe("Healthy");
    }
}