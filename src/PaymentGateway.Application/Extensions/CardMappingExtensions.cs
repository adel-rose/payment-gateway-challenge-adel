using PaymentGateway.Application.DTOs;
using PaymentGateway.Domain.Models;

namespace PaymentGateway.Application.Extensions;

public static class CardMappingExtensions
{
    public static Card ToCard(this CardCreateDTO dto)
    {
        return new Card
        {
            CardNumber  = dto.CardNumber,
            ExpiryMonth = dto.ExpiryMonth,
            ExpiryYear  = dto.ExpiryYear,
            Cvv         = dto.Cvv
        };
    }
    public static Card ToCard(this PaymentRequestDTO dto)
    {
        return new Card
        {
            CardNumber  = dto.CardNumber,
            ExpiryMonth = dto.ExpiryMonth,
            ExpiryYear  = dto.ExpiryYear,
            Cvv         = dto.Cvv
        };
    }
    
    public static Card ExtractCardDetails(this PaymentResponse paymentResponse, PaymentRequestDTO paymentRequestDto)
    {
        return new Card()
        {
            CardNumber = paymentRequestDto.CardNumber.Mask(),
            ExpiryMonth = paymentResponse.ExpiryMonth,
            ExpiryYear = paymentResponse.ExpiryYear,
            Cvv = paymentRequestDto.Cvv
        };
    }
    
    public static CardCreateDTO ToCardCreateDto(this Card card)
    {
        return new CardCreateDTO
        {
            CardNumber  = card.CardNumber,
            ExpiryMonth = card.ExpiryMonth,
            ExpiryYear  = card.ExpiryYear,
            Cvv         = card.Cvv
        };
    }
    
    public static string Mask(this string cardNumber)
    {
        return string.Concat(
            new string('*', cardNumber.Length - 4),
            cardNumber[^4..]);
    }
}