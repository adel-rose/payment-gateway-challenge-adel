using PaymentGateway.Domain.Enums;

namespace PaymentGateway.Domain.Models;

public class Payment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public PaymentStatus Status { get; set; }
    public string Currency { get; set; }
    public int Amount { get; set; }
    public string? AuthorizationCode { get; set; } // Returned by acquiring bank
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Card Card { get; set; }
}