using PaymentGateway.Domain.Models;

namespace PaymentGateway.Application.Interfaces;

public interface ICardRepository
{
    Task<Card> SaveCardAsync(Card card, CancellationToken cancellationToken);
    Task<Card> RetrieveCardAsync(string cardNumber);
}