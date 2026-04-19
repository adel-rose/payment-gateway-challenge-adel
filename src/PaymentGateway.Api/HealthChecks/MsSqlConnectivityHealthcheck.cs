using Dapper;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using PaymentGateway.Application.Interfaces;

namespace PaymentGateway.Api.HealthChecks;

public class MsSqlConnectivityHealthcheck : IHealthCheck
{
    private readonly IDapperDbConnection _dbConnection;
    private readonly Serilog.ILogger _logger;
    
    public MsSqlConnectivityHealthcheck(IDapperDbConnection dbConnetion, Serilog.ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(dbConnetion);
        ArgumentNullException.ThrowIfNull(logger);
        
        _dbConnection = dbConnetion;
        _logger = logger;
    }
    
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = _dbConnection.CreateConnection();

            var result = await connection.ExecuteScalarAsync<int>("SELECT 1");

            return result == 1
                ? HealthCheckResult.Healthy()
                : HealthCheckResult.Unhealthy("[HealthCheck] Database returned unexpected result.");
        }
        catch (Exception e)
        {
            _logger.Error(e, "[HealthCheck] Failed to connect to database");

            return HealthCheckResult.Unhealthy();
        } 
    }
}