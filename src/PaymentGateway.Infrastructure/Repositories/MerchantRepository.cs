using System.Data;
using Dapper;
using PaymentGateway.Application.Exceptions;
using PaymentGateway.Application.Interfaces;
using PaymentGateway.Domain.Models;

namespace PaymentGateway.Infrastructure.Repositories;

public class MerchantRepository : IMerchantRepository
{
    private readonly IDapperDbConnection _dapperDbConnection;

    public MerchantRepository(IDapperDbConnection dapperDbConnection)
    {
        ArgumentNullException.ThrowIfNull(dapperDbConnection);
        
        _dapperDbConnection = dapperDbConnection;
    }
    public async Task<Merchant> FindByName(string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);
        
        using (IDbConnection db = _dapperDbConnection.CreateConnection())
        {
            var merchant = await db.QueryFirstOrDefaultAsync<Merchant>(
                "SELECT * FROM Merchants WHERE MerchantName = @MerchantName",
                new
                {
                    MerchantName = name
                });

            if (merchant is null)
            {
                throw new NotFoundException($"Merchant with name {name} was not found");
            }

            return merchant;
        }
    }
}