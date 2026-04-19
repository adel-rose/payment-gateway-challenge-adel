using PaymentGateway.Domain.Enums;

namespace PaymentGateway.Application.DTOs;

public class PaymentResponseDTO
{
    public Guid PaymentRequestId { get; set; }
    public string Status { get; set; }
    public string LastFourDigits { get; set; }
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public string Currency { get; set; }
    public int Amount { get; set; }
}