using System.Net;
using Microsoft.Extensions.Configuration;
using Moq;
using PaymentGateway.Application.DTOs;
using PaymentGateway.Application.Http;
using PaymentGateway.Application.Interfaces;
using PaymentGateway.Domain.Models;

namespace PaymentGateway.Tests.Fixture;

public class PaymentRequestFixture
{
    
    public Mock<IHttpClientService> HttpClientMock { get; set; }
    public Mock<IConfiguration> IConfigurationMock = new Mock<IConfiguration>();

    public PaymentRequestFixture()
    {
        HttpClientMock = new Mock<IHttpClientService>();
        
    }


    public void RegisterMocks(bool isSuccess)
    {
        HttpClientMock
            .Setup(x => x.PostAsync<IssuingPaymentRequestDTO, IssuingPaymentResponseDTO>(
                It.IsAny<string>(),
                It.IsAny<IssuingPaymentRequestDTO>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(HttpResponse(isSuccess));
    }

    public HttpResponseWrapper<IssuingPaymentResponseDTO> HttpResponse(bool isSuccess)
    {
        return new HttpResponseWrapper<IssuingPaymentResponseDTO>()
        {
            IsSuccess = isSuccess,
            Status = HttpStatusCode.OK,
            Response = new IssuingPaymentResponseDTO()
            {
                Authorized = isSuccess
            }
        };
    }
    
}