using Dapper;
using Domain.Entities;
using Domain.Interfaces;
using System.Data;

namespace Infrastructure.Repositories;

public class PairRepository : IPairRepository
{
    private readonly IDbConnection _connection;

    public PairRepository(IDbConnection connection)
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

    public async Task<int> Delete(string key)
    {
        string sql = "DELETE FROM pairs WHERE key=@Key;";
        var queryObject = new
        {
            Key = key,
        };
        return await _connection.ExecuteAsync(sql, queryObject); 
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
