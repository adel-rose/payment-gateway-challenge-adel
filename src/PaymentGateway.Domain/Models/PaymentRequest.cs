namespace PaymentGateway.Domain.Models;

public class PaymentRequest
{
    public string CardNumber { get; set; }
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public String Currency { get; set; }
    public int Amount { get; set; }
    public string Cvv { get; set; }
}