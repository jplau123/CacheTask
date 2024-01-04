using Dapper;
using Domain.Entities;
using Domain.Interfaces;
using System.Data;

namespace Infrastructure.Repositories;

public class ItemRepository : IPairRepository
{
    private readonly IDbConnection _connection;

    public ItemRepository(IDbConnection connection)
    {
        _connection = connection;
    }

    public Task<PairEntity> Append(PairEntity pair)
    {
        throw new NotImplementedException();
    }

    public Task<PairEntity> Create(PairEntity pair)
    {
        throw new NotImplementedException();
    }

    public Task<int> Delete(string key)
    {
        throw new NotImplementedException();
    }

    public async Task<int> DeleteOlderThan(DateTimeOffset timestamp)
    {
        string sql = "DELETE FROM pairs WHERE expires_at < @timestamp";

        return await _connection.ExecuteAsync(sql, new { timestamp });
    }

    public Task<PairEntity> GetByKey(string key)
    {
        throw new NotImplementedException();
    }

    public Task<int> UpdateEpiresAt(DateTimeOffset timestamp)
    {
        throw new NotImplementedException();
    }
}
