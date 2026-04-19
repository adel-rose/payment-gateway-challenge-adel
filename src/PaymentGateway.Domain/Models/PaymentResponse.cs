using PaymentGateway.Domain.Enums;

namespace PaymentGateway.Domain.Models;

public class PaymentResponse
{ 
    public Guid PaymentRequestId { get; set; }
    public PaymentStatus Status { get; set; }
    public string AuthorizationCode { get; set; }
    public string LastFourDigits { get; set; }
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public string Currency { get; set; }
    public int Amount { get; set; }
}