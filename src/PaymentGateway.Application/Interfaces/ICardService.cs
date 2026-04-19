using PaymentGateway.Application.DTOs;
using PaymentGateway.Domain.Models;

namespace PaymentGateway.Application.Interfaces;

public interface ICardService
{
    Task<Card> SaveCardDetailsAsync(PaymentRequestDTO paymentRequestDto,PaymentResponse paymentResponse, CancellationToken cancellationToken);
}