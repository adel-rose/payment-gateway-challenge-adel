using System.Net;
using FluentNHibernate.Utils;
using Microsoft.Extensions.Configuration;
using Moq;
using PaymentGateway.Application.DTOs;
using PaymentGateway.Application.Exceptions;
using PaymentGateway.Application.Http;
using PaymentGateway.Application.Interfaces;
using PaymentGateway.Application.Services;
using PaymentGateway.Domain.Models;
using PaymentGateway.Tests.Fixture;

namespace PaymentGateway.Tests.Tests;

public class PaymentRequestTests : IClassFixture<PaymentRequestFixture>
{
    private readonly PaymentRequestFixture _paymentRequestFixture;

    public PaymentRequestTests(PaymentRequestFixture paymentRequestFixture)
    {
        _paymentRequestFixture = paymentRequestFixture;
    }
    
    [Theory]
    [InlineData(true)]
    public async Task
        WhenRecievePaymentRequest_GivenSuccessfulResponseFromIssuing_PaymentResponseShouldBeAuthorized(bool isSuccess)
    {
        // Arrange
        var sut = new IssuingBankService(_paymentRequestFixture.HttpClientMock.Object, _paymentRequestFixture.IConfigurationMock.Object);
        _paymentRequestFixture.RegisterMocks(isSuccess);
        
        // Act
        var result = await sut.ForwardPaymentRequest(new IssuingPaymentRequestDTO(), CancellationToken.None);
        
        // Assert
        Assert.NotNull(result);
        Assert.True(result.Authorized);
    }
    
    [Theory]
    [InlineData(false)]
    public async Task
        WhenRecievePaymentRequest_GivenUnsuccessfulResponseFromIssuing_BankCommunicationExceptionIsThrown(bool isSuccess)
    {
        // Arrange
        var sut = new IssuingBankService(_paymentRequestFixture.HttpClientMock.Object, _paymentRequestFixture.IConfigurationMock.Object);
        _paymentRequestFixture.RegisterMocks(isSuccess);
        
        // Assert
       await Assert.ThrowsAsync<BankCommunicationException>(() => sut.ForwardPaymentRequest(new IssuingPaymentRequestDTO(), CancellationToken.None));

    }
}