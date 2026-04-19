using System.Net;
using Microsoft.Extensions.Configuration;
using PaymentGateway.Application.DTOs;
using PaymentGateway.Application.Exceptions;
using PaymentGateway.Application.Extensions;
using PaymentGateway.Application.Http;
using PaymentGateway.Application.Interfaces;
using PaymentGateway.Domain.Models;

namespace PaymentGateway.Application.Services;

public class IssuingBankService : IIssuingBankService
{
    private readonly IHttpClientService _httpClientService;
    private readonly IConfiguration _configuration;

    public IssuingBankService(IHttpClientService httpClientService, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(httpClientService);
        
        _httpClientService = httpClientService;
        _configuration = configuration;
    }
    
    public async Task<IssuingPaymentResponseDTO> ForwardPaymentRequest(IssuingPaymentRequestDTO paymentRequest, CancellationToken cancellationToken)
    {
        var issuingResponse = await _httpClientService.PostAsync<IssuingPaymentRequestDTO, IssuingPaymentResponseDTO>(_configuration["IssuingBankUrl"],
            paymentRequest, cancellationToken);

        if (!issuingResponse.IsSuccess)
        {
            throw new BankCommunicationException($"Payment request was not completed successfully on issuing bank.");
        }
        
        return issuingResponse.Response;
    }
}