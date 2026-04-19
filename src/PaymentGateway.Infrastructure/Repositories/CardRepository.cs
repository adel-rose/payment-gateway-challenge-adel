using System.Data;
using Dapper;
using PaymentGateway.Application.Extensions;
using PaymentGateway.Application.Interfaces;
using PaymentGateway.Domain.Models;

namespace PaymentGateway.Infrastructure.Repositories;

public class CardRepository : ICardRepository
{
    private readonly IDapperDbConnection _dapperDbConnection;
    
    public CardRepository(IDapperDbConnection dapperDbConnection)
    {
        ArgumentNullException.ThrowIfNull(dapperDbConnection);
        
        _dapperDbConnection = dapperDbConnection;
    }
    
    public async Task<Card> SaveCardAsync(Card card, CancellationToken cancellationToken)
    {
        if (card is null)
        {
            throw new ArgumentNullException(nameof(card));
        }
        
        using (IDbConnection db = _dapperDbConnection.CreateConnection())
        {
            await db.ExecuteAsync(
                @"INSERT INTO Cards (Id, CardNumber, ExpiryMonth, ExpiryYear, Cvv)
                          VALUES (@Id, @CardNumber, @ExpiryMonth, @ExpiryYear, @Cvv)",
                new
                {
                    card.Id,
                    card.CardNumber,
                    card.ExpiryMonth,
                    card.ExpiryYear,
                    card.Cvv
                });

            return card;
        }
    }

    public async Task<Card> RetrieveCardAsync(string cardNumber)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
        {
            throw new ArgumentNullException(nameof(cardNumber));
        }
        
        using (IDbConnection db = _dapperDbConnection.CreateConnection())
        {
            var card = await db.QueryFirstOrDefaultAsync<Card>(
                @"SELECT * FROM Cards WHERE CardNumber = @cardNumber",
                new
                {
                    cardNumber = cardNumber.Mask()
                });

            return card;
        }
    }
}