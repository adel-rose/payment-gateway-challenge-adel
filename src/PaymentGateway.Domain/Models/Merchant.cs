namespace PaymentGateway.Domain.Models;

public class Merchant
{
    public Guid Id { get; set; }
    public string MerchantName { get; set; }
    public string APIKey { get; set; }
    public DateTime CreatedAt { get; set; }
}