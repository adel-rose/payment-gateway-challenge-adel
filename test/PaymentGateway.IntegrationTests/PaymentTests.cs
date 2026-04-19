using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Application.DTOs;
using PaymentGateway.Application.Interfaces;
using PaymentGateway.Domain.Enums;
using Shouldly;

namespace PaymentGateway.IntegrationTests;

public class PaymentTests : IClassFixture<PaymentGatewayApiWebAppFactory>, IDisposable
{
    private readonly IServiceScope _scope;
    
    private HttpClient _httpClient;

    private readonly IPaymentRepository _paymentRepository;

    private static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public PaymentTests(PaymentGatewayApiWebAppFactory factory)
    {
        _httpClient = factory.CreateClient();

        _scope = factory.Services.CreateScope();

        _paymentRepository = _scope.ServiceProvider.GetService<IPaymentRepository>() ?? throw new ArgumentOutOfRangeException(nameof(IPaymentRepository));
    }

    [Fact]
    public async Task CreatePayment_with_valid_card_is_approved()
    {
        var resquest = new PaymentRequestDTO
        {
            CardNumber = "4242424242424242",
            ExpiryMonth = 12,
            ExpiryYear = 2030,
            Cvv = "123",
            Amount = 1000,
            Currency = "USD"
        };
        
        var response = await _httpClient.PostAsync("/api/paymentgateway/processpayment", JsonContent.Create(resquest));
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync(CancellationToken.None);
        var responseDto = JsonSerializer.Deserialize<PaymentResponseDTO>(responseString, SerializerOptions);
        responseDto.ShouldNotBeNull();
        responseDto.Status.ShouldBe("Authorized");
        responseDto.Amount.ShouldBe(1000);
        responseDto.LastFourDigits.ShouldBe("4242");
    }

    [Fact]
    public async Task Approved_payment_must_be_store_in_db()
    {
        var resquest = new PaymentRequestDTO
        {
            CardNumber = "4242424242424242",
            ExpiryMonth = 12,
            ExpiryYear = 2030,
            Cvv = "123",
            Amount = 1000,
            Currency = "USD"
        };
        
        var response = await _httpClient.PostAsync("/api/paymentgateway/processpayment", JsonContent.Create(resquest));
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync(CancellationToken.None);
        var responseDto = JsonSerializer.Deserialize<PaymentResponseDTO>(responseString, SerializerOptions);

        var storedPayment = await _paymentRepository.RetrievePayment(responseDto.PaymentRequestId, CancellationToken.None);
        storedPayment.ShouldNotBeNull();
        storedPayment.AuthorizationCode.ShouldBe("00"); // we know this since the card used force this behavior
        storedPayment.Amount.ShouldBe(1000);
        storedPayment.Status.ShouldBe(PaymentStatus.Authorized);
    }

    [Fact]
    public async Task After_making_payment_it_can_be_retrieved()
    {
        var resquest = new PaymentRequestDTO
        {
            CardNumber = "4242424242424242",
            ExpiryMonth = 12,
            ExpiryYear = 2030,
            Cvv = "123",
            Amount = 1000,
            Currency = "USD"
        };
        
        var response = await _httpClient.PostAsync("/api/paymentgateway/processpayment", JsonContent.Create(resquest));
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync(CancellationToken.None);
        var responseDto = JsonSerializer.Deserialize<PaymentResponseDTO>(responseString, SerializerOptions);
        
        var payment = await _httpClient.GetAsync($"/api/paymentgateway/payment/{responseDto.PaymentRequestId}");
        payment.EnsureSuccessStatusCode();
        var paymentString = await payment.Content.ReadAsStringAsync(CancellationToken.None);
        var paymentDto = JsonSerializer.Deserialize<PaymentResponseDTO>(paymentString, SerializerOptions);
        
        paymentDto.ShouldNotBeNull();
        paymentDto.Amount.ShouldBe(1000);
        paymentDto.LastFourDigits.ShouldBe("4242");
    }

    public void Dispose()
    {
        _httpClient.Dispose();
        _scope.Dispose();
    }
}