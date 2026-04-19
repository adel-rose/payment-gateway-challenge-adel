using Microsoft.AspNetCore.Http.HttpResults;
using PaymentGateway.Application.DTOs;
using PaymentGateway.Application.Exceptions;
using PaymentGateway.Application.Extensions;
using PaymentGateway.Application.Interfaces;
using PaymentGateway.Domain.Models;

namespace PaymentGateway.Application.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IIssuingBankService _issuingBankService;

    public PaymentService(IPaymentRepository paymentRepository, IIssuingBankService issuingBankService)
    {
        _paymentRepository = paymentRepository;
        _issuingBankService = issuingBankService;
    }
    
    public async Task<Payment> SavePaymentDetails(Card card, PaymentRequestDTO paymentRequestDto ,PaymentResponse paymentResponse, CancellationToken cancellationToken)
    {
        if (card is null)
        {
            throw new ArgumentNullException($"Parameter of type {card.GetType()} was null");
        }
        
        var payment = paymentRequestDto.ToPayment(paymentResponse);
        
        // Attaching the payment id to the payment response and card used for the payment to the payment
        paymentResponse.PaymentRequestId = payment.Id;
        payment.Card = card;
        
        return await _paymentRepository.SavePaymentAsync(payment, cancellationToken);
    }

    public async Task<PaymentResponse> ProcessCardPayment(PaymentRequestDTO paymentRequestDto, CancellationToken cancellationToken)
    {
        var issuingBankPaymentRequest = paymentRequestDto.ToIssuingPaymentRequestDto();
        
        var issuingPaymentResponse = await _issuingBankService.ForwardPaymentRequest(issuingBankPaymentRequest, cancellationToken);
        
        return _buildPaymentResponse(issuingPaymentResponse, paymentRequestDto);
    }

    public async Task<Payment> RetrievePayment(Guid paymentId, CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.RetrievePayment(paymentId, cancellationToken);

        if (payment is null)
            throw new NotFoundException($"Payment id {paymentId} was not found");

        return payment;
    }

    private PaymentResponse _buildPaymentResponse(IssuingPaymentResponseDTO issuingPaymentResponse, PaymentRequestDTO paymentRequestDto)
    {
        var paymentResponse = issuingPaymentResponse.ToPaymentResponse();
        paymentResponse.Amount = paymentRequestDto.Amount;
        paymentResponse.Currency = paymentRequestDto.Currency;
        paymentResponse.LastFourDigits = paymentRequestDto.CardNumber[^4..];
        paymentResponse.ExpiryMonth = paymentRequestDto.ExpiryMonth;
        paymentResponse.ExpiryYear = paymentRequestDto.ExpiryYear;

        return paymentResponse;
    }
}