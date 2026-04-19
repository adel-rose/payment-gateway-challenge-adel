using PaymentGateway.Application.DTOs;
using PaymentGateway.Domain.Models;

namespace PaymentGateway.Application.Interfaces;

public interface IIssuingBankService
{
    Task<IssuingPaymentResponseDTO> ForwardPaymentRequest(IssuingPaymentRequestDTO paymentRequest, CancellationToken cancellationToken);
}