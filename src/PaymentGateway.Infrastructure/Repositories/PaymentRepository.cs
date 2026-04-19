using Dapper;
using PaymentGateway.Application.Interfaces;
using System.Data;
using Microsoft.Extensions.Logging;
using PaymentGateway.Application.Exceptions;
using PaymentGateway.Domain.Models;

namespace PaymentGateway.Infrastructure.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly IDapperDbConnection _dapperDbConnection;
        
        public PaymentRepository(IDapperDbConnection dapperDbConnection, ILogger<string> logger)
        {
            ArgumentNullException.ThrowIfNull(dapperDbConnection);
            
            _dapperDbConnection = dapperDbConnection;
        }
        
        public async Task<Payment> SavePaymentAsync(Payment payment, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(payment);
            
            using (IDbConnection db = _dapperDbConnection.CreateConnection())
            {
                 await db.ExecuteAsync(
                    @"INSERT INTO Payments (Id,Status, Currency, Amount, AuthorizationCode, CreatedAt, CardId)
                          VALUES (@Id, @Status, @Currency, @Amount, @AuthorizationCode, @CreatedAt, @CardId)",
                    new
                    {
                        payment.Id,
                        Status = (int)payment.Status,
                        payment.Currency,
                        payment.Amount,
                        payment.AuthorizationCode,
                        payment.CreatedAt,
                        CardId = payment.Card.Id
                    });
                 
                 return payment;
            }
        }

        public async Task<Payment> RetrievePayment(Guid paymentId, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(paymentId);
            
            using (IDbConnection db = _dapperDbConnection.CreateConnection())
            {
                var multi = await db.QueryMultipleAsync(
                    @"SELECT * FROM Payments WHERE Id = @paymentId;
                          SELECT * FROM Cards c INNER JOIN Payments p ON c.ID = p.CardId AND p.Id = @paymentId",
                    new { paymentId });

                var payment = await multi.ReadFirstOrDefaultAsync<Payment>();

                if (payment is null)
                {
                    throw new NotFoundException($"Not payment history found for payment id {paymentId}");
                }
                
                var card = await multi.ReadFirstOrDefaultAsync<Card>();
                
                payment.Card = card;

                return payment;
            }
        }
    }
}
