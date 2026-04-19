using Microsoft.AspNetCore.Identity;
using PaymentGateway.Application.DTOs;
using PaymentGateway.Domain.Enums;
using PaymentGateway.Domain.Models;

namespace PaymentGateway.Application.Extensions;

public static class PaymentMappingExtentions
{
    public static PaymentRequest ToPaymentRequest(this PaymentRequestDTO dto)
    {
        return new PaymentRequest()
        {
            CardNumber = dto.CardNumber,
            ExpiryMonth = dto.ExpiryMonth,
            ExpiryYear = dto.ExpiryYear,
            Currency = dto.Currency,
            Amount = dto.Amount,
            Cvv = dto.Cvv
        };
    }
    
    
    public static Payment ToPayment(this PaymentRequestDTO dto, PaymentResponse paymentResponse)
    {
        return new Payment()
        {
            Currency = dto.Currency,
            Amount = dto.Amount,
            Status = paymentResponse.Status,
            AuthorizationCode = !string.IsNullOrEmpty(paymentResponse.AuthorizationCode) ? paymentResponse.AuthorizationCode : null
        };
    }
    
    public static IssuingPaymentRequestDTO ToIssuingPaymentRequestDto(this PaymentRequestDTO dto)
    {
        return new IssuingPaymentRequestDTO()
        {
            CardNumber = dto.CardNumber,
            ExpiryDate = string.Concat(Convert.ToString(dto.ExpiryMonth), "/", Convert.ToString(dto.ExpiryYear)),
            Currency = dto.Currency,
            Amount = dto.Amount,
            Cvv = dto.Cvv
        };
    }
    
    public static PaymentResponse ToPaymentResponse(this IssuingPaymentResponseDTO dto)
    {
        return new PaymentResponse()
        {
            Status = dto.Authorized ? PaymentStatus.Authorized : PaymentStatus.Declined,
            AuthorizationCode = dto.AuthorizationCode
        };
    }
    
    public static PaymentResponseDTO ToPaymentResponseDto(this PaymentResponse paymentResponse)
    {
        return new PaymentResponseDTO()
        {
            PaymentRequestId = paymentResponse.PaymentRequestId,
            Status = paymentResponse.Status.ToString(),
            LastFourDigits = paymentResponse.LastFourDigits,
            ExpiryMonth = paymentResponse.ExpiryMonth,
            ExpiryYear = paymentResponse.ExpiryYear,
            Currency = paymentResponse.Currency,
            Amount = paymentResponse.Amount
        };
    }
    
    public static PaymentResponseDTO ToPaymentResponseDto(this Payment payment)
    {
        return new PaymentResponseDTO()
        {
            PaymentRequestId = payment.Id,
            Status = payment.Status.ToString(),
            LastFourDigits = payment.Card.CardNumber[^4..],
            ExpiryMonth = payment.Card.ExpiryMonth,
            ExpiryYear = payment.Card.ExpiryYear,
            Currency = payment.Currency,
            Amount = payment.Amount
        };
    }
    

}