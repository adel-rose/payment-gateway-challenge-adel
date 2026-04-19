using PaymentGateway.Application.DTOs;
using PaymentGateway.Domain.Models;

namespace PaymentGateway.Application.Interfaces;

public interface IPaymentService
{
    Task<Payment> SavePaymentDetails(Card card, PaymentRequestDTO paymentRequestDto ,PaymentResponse paymentResponse, CancellationToken cancellationToken);
    Task<PaymentResponse> ProcessCardPayment(PaymentRequestDTO paymentRequest, CancellationToken cancellationToken);
    Task<Payment> RetrievePayment(Guid paymentId, CancellationToken cancellationToken);

}