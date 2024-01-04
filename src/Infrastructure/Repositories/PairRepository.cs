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

    public async Task<PairEntity?> Create(PairEntity pair)
    {
        string sql = @"INSERT INTO pairs (key, value, expires_at, expiration_period_in_seconds) 
                        VALUES (@pairKey, @pairValue, @pairExpiresAt, @pairExpirationPeriod)
                        RETURNING id, key, value, expires_at AS expiresAt, expiration_period_in_seconds AS expirationPerionInSeconds";

        var parameters = new
        {
            pairKey = pair.Key,
            pairValue = pair.Value,
            pairExpiresAt = pair.ExpiresAt,
            pairExpirationPeriod = pair.ExpirationPeriodInSeconds
        };

        return await _connection.QueryFirstOrDefaultAsync<PairEntity?>(sql, parameters);
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

    public Task<int> UpdateEpiresAt(DateTimeOffset timestamp)
    {
        throw new NotImplementedException();
    }
}
