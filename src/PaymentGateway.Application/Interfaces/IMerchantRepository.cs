using PaymentGateway.Domain.Models;

namespace PaymentGateway.Application.Interfaces;

public interface IMerchantRepository
{
    Task<Merchant> FindByName(string name);
}