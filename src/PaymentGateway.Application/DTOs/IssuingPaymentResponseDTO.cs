using System.Text.Json.Serialization;

namespace PaymentGateway.Application.DTOs;

public class IssuingPaymentResponseDTO
{
    [JsonPropertyName("authorized")]
    public bool Authorized { get; set; }

    [JsonPropertyName("authorization_code")]
    public string AuthorizationCode { get; set; }
}