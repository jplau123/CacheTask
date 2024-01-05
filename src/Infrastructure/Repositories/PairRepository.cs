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

    public async Task<PairEntity?> Create(PairEntity pair)
    {
        string sql = @"INSERT INTO pairs (key, value, expires_at, expiration_period_in_seconds) 
                        VALUES (@pairKey, @pairValue, @pairExpiresAt, @pairExpirationPeriod)
                        RETURNING id, key, value, expires_at AS expiresAt, expiration_period_in_seconds AS expirationPeriodInSeconds";

        var parameters = new
        {
            pairKey = pair.Key,
            pairValue = pair.Value,
            pairExpiresAt = pair.ExpiresAt,
            pairExpirationPeriod = pair.ExpirationPeriodInSeconds
        };

        var result = await _connection.QueryFirstOrDefaultAsync<PairEntity?>(sql, parameters);

        return result;
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

    public async Task<int> Update(PairEntity pair)
    {
        string sql = @"UPDATE pairs 
                        SET key = @pairKey, value = @pairValue, expires_at = @pairExpiresAt, expiration_period_in_seconds = @pairExpirationPeriod 
                        WHERE id = @pairId";

        var parameters = new
        {
            pairKey = pair.Key,
            pairValue = pair.Value,
            pairExpiresAt = pair.ExpiresAt,
            pairExpirationPeriod = pair.ExpirationPeriodInSeconds,
            pairId = pair.Id
        };

        return await _connection.ExecuteAsync(sql, parameters);
    }
    public async Task<PairEntity?> GetByKey(string key)
    {
        string sql = @"SELECT id, key, value, expires_at AS ExpiresAt, expiration_period_in_seconds AS ExpirationPeriodInSeconds
                        FROM pairs
                        WHERE key = @key";
        return await _connection.QueryFirstOrDefaultAsync<PairEntity?>(sql, new { key });
    }

    public async Task<int> UpdateEpiresAt(string key, DateTimeOffset timestamp)
    {
        string sql = @"UPDATE pairs SET expires_at = @pairExpiresAt WHERE key = @pairKey";

        var parameters = new
        {
            pairKey = key,
            pairExpiresAt = timestamp
        };

        return await _connection.ExecuteAsync(sql, parameters);
    }
}
