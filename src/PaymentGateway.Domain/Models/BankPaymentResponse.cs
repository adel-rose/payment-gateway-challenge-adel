using System.Text.Json.Serialization;

namespace PaymentGateway.Domain.Models;

public class BankPaymentResponse
{
    [JsonPropertyName("authorized")]
    public bool Authorized { get; init; }

    [JsonPropertyName("authorization_code")]
    public string? AuthorizationCode { get; init; }
}