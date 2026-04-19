using System.Net.Sockets;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace PaymentGateway.Api.HealthChecks;

public class BankSimulatorHealthCheck : IHealthCheck
{
    private readonly Serilog.ILogger _logger;
    private readonly IConfiguration _configuration;


    public BankSimulatorHealthCheck(Serilog.ILogger logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }
    
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        try
        {
            var uri = new Uri(_configuration["IssuingBankUrl"]);
            
            using var tcpClient = new TcpClient();
            await tcpClient.ConnectAsync(uri.Host, uri.Port, cancellationToken);

            return HealthCheckResult.Healthy();
        }
        catch (Exception e)
        {
            _logger.Error(e, "[HealthCheck] Failed to connect to bank simulator");
            return HealthCheckResult.Unhealthy();
        }

    }
}