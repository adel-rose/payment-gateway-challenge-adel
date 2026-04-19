using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Application.Interfaces;
using PaymentGateway.IntegrationTests.Mocks;

namespace PaymentGateway.IntegrationTests;

/// <summary>
/// Spins up an in-memory version of our api.
/// Overrides one specific service registration.
/// </summary>
public class PaymentGatewayApiWebAppFactory : WebApplicationFactory<Program>
{
    // used to mock external dependencies
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.AddScoped<IIssuingBankService>(_ => new IssuingBankServiceMock().GetMock());
        });
    }
} 