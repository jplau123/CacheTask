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

    public async Task<PairEntity?> GetByKey(string key)
    {
        string sql = @"SELECT id, key, value, expires_at AS ExpiresAt, expiration_period_in_seconds AS ExpirationPeriodInSeconds
                        FROM pairs
                        WHERE key = @key";
        return await _connection.QueryFirstOrDefaultAsync<PairEntity?>(sql, new { key });
    }

    public Task<int> UpdateEpiresAt(DateTimeOffset timestamp)
    {
        throw new NotImplementedException();
    }
}
