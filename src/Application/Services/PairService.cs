using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces;
using Domain.Exceptions;
using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Application.Services;

public class PairService : IPairService
{
    private readonly IPairRepository _pairRepository;
    private readonly IConfiguration _configuration;

    public PairService(IPairRepository pairRepository, IConfiguration configuration)
    {
        _pairRepository = pairRepository;
        _configuration = configuration;
    }

    public Task<AppendPairResponse> Append(AppendPairRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<CreatePairResponse> Create(CreatePairRequest request)
    {
        throw new NotImplementedException();
    }

    public Task Delete(string key)
    {
        throw new NotImplementedException();
    }

    public async Task<GetPairResponse> Get(string key)
    {
        if (key is null)
            throw new BadRequestException("Key name should be entered.");

        var result = await _pairRepository.GetByKey(key);

        if (result is null)
        {
            throw new NotFoundException($"No key \"{key}\" exists.");
        }

        if (result.ExpiresAt < DateTime.UtcNow) {
            await _pairRepository.Delete(key);
            throw new NotFoundException($"No key \"{key}\" exists.");
        }

        result.ExpiresAt = DateTime.UtcNow + TimeSpan.FromSeconds((int)result.ExpirationPeriodInSeconds!);

        var value = DeserializeJson(result.Value);
        
        return new GetPairResponse
        {
            Key = result.Key,
            Value = value,
            ExpiresAt = result.ExpiresAt,
            ExpirationPeriodInSeconds = result.ExpirationPeriodInSeconds
        };
    }

    private static List<object> DeserializeJson(string value)
    {
        return JsonSerializer.Deserialize<List<object>>(value)
            ?? throw new Exception("Failed to deserialize a string.");
    }

    private static string SerializeJson(List<object> list)
    {
        return JsonSerializer.Serialize(list)
            ?? throw new Exception("Failed to serialize a list of objects to string.");
    }
}
