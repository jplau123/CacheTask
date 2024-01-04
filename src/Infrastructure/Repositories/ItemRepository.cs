using Dapper;
using Domain.Interfaces;
using System.Data;

namespace Infrastructure.Repositories;

public class ItemRepository : IItemRepository
{
    private readonly IDbConnection _connection;

    public ItemRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public async Task<int> DeleteOlderThan(DateTimeOffset timestamp)
    {
        string sql = "DELETE FROM pairs WHERE expires_at < @timestamp";

        return await _connection.ExecuteAsync(sql, new { timestamp });
    }
}
