namespace PaymentGateway.Application.DTOs;

public class CardCreateDTO
{
    public string CardNumber { get; set; }
    public string LastFourDigits => CardNumber[^4..];
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
    public string Cvv { get; set; }
}