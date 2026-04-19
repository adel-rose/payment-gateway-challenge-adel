using PaymentGateway.Application.DTOs;
using PaymentGateway.Application.Extensions;
using PaymentGateway.Application.Interfaces;
using PaymentGateway.Domain.Models;

namespace PaymentGateway.Application.Services;

public class CardService : ICardService
{
    private readonly ICardRepository _cardRepository;

    public CardService(ICardRepository cardRepository)
    {
        ArgumentNullException.ThrowIfNull(cardRepository);
        
        _cardRepository = cardRepository;
    }
    
    public async Task<Card> SaveCardDetailsAsync(PaymentRequestDTO paymentRequestDto, PaymentResponse paymentResponse, CancellationToken cancellationToken)
    {
        if (paymentRequestDto is null)
        {
            throw new ArgumentNullException(nameof(paymentRequestDto));
        }

        if (paymentResponse is null)
        {
            throw new ArgumentNullException(nameof(paymentResponse));
        }
        
        // Need to verify whehter card does not already exists
        var savedCard = await _cardRepository.RetrieveCardAsync(paymentRequestDto.CardNumber);

        if (savedCard is null)
        {
            var card = paymentResponse.ExtractCardDetails(paymentRequestDto);
        
            savedCard = await _cardRepository.SaveCardAsync(card, cancellationToken);
        }
        
        return savedCard;
    }
}