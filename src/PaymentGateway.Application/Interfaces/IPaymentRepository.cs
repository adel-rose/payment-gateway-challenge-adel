using PaymentGateway.Domain.Models;

namespace PaymentGateway.Application.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Payment> SavePaymentAsync(Payment payment, CancellationToken cancellationToken);
        Task<Payment> RetrievePayment(Guid paymentId, CancellationToken cancellationToken);
    }
}
