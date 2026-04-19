using System.Text.Json.Serialization;

namespace PaymentGateway.Domain.Models;

public class BankPaymentRequest
{
    [JsonPropertyName("card_number")]
    public string CardNumber { get; init; }

    [JsonPropertyName("expiry_date")]
    public string ExpiryDate { get; init; }     // Formatted as "MM/yyyy"

    [JsonPropertyName("currency")]
    public string Currency { get; init; }

    [JsonPropertyName("amount")]
    public int Amount { get; init; }

    [JsonPropertyName("cvv")]
    public string Cvv { get; init; }
}