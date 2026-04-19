using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Application.Interfaces;
using PaymentGateway.IntegrationTests.Mocks;

namespace PaymentGateway.IntegrationTests;
// following: https://learn.microsoft.com/en-us/aspnet/core/migration/50-to-60-samples?view=aspnetcore-6.0#test-with-webapplicationfactory-or-testserver
// and:       https://timdeschryver.dev/blog/how-to-test-your-csharp-web-api
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