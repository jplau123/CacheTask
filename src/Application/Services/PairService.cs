using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Application.Services;

public class PairService : IPairService
{
    private readonly IPairRepository _pairRepository;
    private readonly IConfiguration _configuration;

    public PairService(
        IPairRepository pairRepository,
        IConfiguration configuration)
    {
        _pairRepository = pairRepository;
        _configuration = configuration;
    }

    public async Task<AppendPairResponse> Append(AppendPairRequest request)
    {
        var getResult = await _pairRepository.GetByKey(request.Key);

        TimeSpan expirationTimeSpan = (getResult is null) ? GetExpirationTimeSpanFromConfig() : GetExpirationTimeSpan(getResult.ExpirationPeriodInSeconds);

        PairEntity pairEntityRequest;

        if (getResult is null)
        {
            pairEntityRequest = new()
            {
                Key = request.Key,
                Value = SerializeJson(request.Value),
                ExpiresAt = DateTime.UtcNow + expirationTimeSpan,
                ExpirationPeriodInSeconds = (int)expirationTimeSpan.TotalSeconds
            };

            PairEntity pairEntityResponse = await _pairRepository.Create(pairEntityRequest)
                ?? throw new Exception("Failed to save pair entity.");

            return new AppendPairResponse()
            {
                Key = pairEntityResponse.Key,
                Value = DeserializeJson(pairEntityResponse.Value),
                ExpiresAt = pairEntityResponse.ExpiresAt,
                ExpirationPeriodInSeconds = pairEntityResponse.ExpirationPeriodInSeconds
            };
        }

        var listOfObjects = DeserializeJson(getResult.Value);

        listOfObjects.AddRange(request.Value);

        pairEntityRequest = new()
        {
            Id = getResult.Id,
            Key = request.Key,
            Value = SerializeJson(listOfObjects),
            ExpiresAt = DateTime.UtcNow + expirationTimeSpan,
            ExpirationPeriodInSeconds = (int)expirationTimeSpan.TotalSeconds
        };

        await _pairRepository.Update(pairEntityRequest);

        return new AppendPairResponse()
        {
            Key = getResult.Key,
            Value = listOfObjects,
            ExpiresAt = pairEntityRequest.ExpiresAt,
            ExpirationPeriodInSeconds = pairEntityRequest.ExpirationPeriodInSeconds
        };
    }

    public async Task<CreatePairResponse> Create(CreatePairRequest request)
    {
        var getResult = await _pairRepository.GetByKey(request.Key);

        TimeSpan expirationTimeSpan = GetExpirationTimeSpan(request.ExpirationPeriodInSeconds);

        PairEntity pairEntityRequest = new()
        {
            Key = request.Key,
            Value = SerializeJson(request.Value),
            ExpiresAt = DateTime.UtcNow + expirationTimeSpan,
            ExpirationPeriodInSeconds = (int)expirationTimeSpan.TotalSeconds
        };

        if (getResult is null)
        {
            var pairEntityResponse = await _pairRepository.Create(pairEntityRequest)
                ?? throw new Exception("Failed to save pair entity.");

            return new CreatePairResponse()
            {
                Key = pairEntityResponse.Key,
                Value = DeserializeJson(pairEntityResponse.Value),
                ExpiresAt = pairEntityResponse.ExpiresAt,
                ExpirationPeriodInSeconds = pairEntityResponse.ExpirationPeriodInSeconds
            };
        }

        pairEntityRequest.Id = getResult.Id;
        await _pairRepository.Update(pairEntityRequest);

        return new CreatePairResponse()
        {
            Key = pairEntityRequest.Key,
            Value = DeserializeJson(pairEntityRequest.Value),
            ExpiresAt = pairEntityRequest.ExpiresAt,
            ExpirationPeriodInSeconds = pairEntityRequest.ExpirationPeriodInSeconds
        };
    }

    public async Task Delete(string key)
    {
        var keyValue = await _pairRepository.GetByKey(key);

        if (keyValue == null)
        {
            throw new NotFoundException("Dont have this a key");
        }

        await _pairRepository.Delete(key);
    }

    public async Task<GetPairResponse> Get(string key)
    {
        var result = await _pairRepository.GetByKey(key)
            ?? throw new NotFoundException($"No key '{key}' exists.");

        if (result.ExpiresAt < DateTime.UtcNow)
        {
            await _pairRepository.Delete(key);
            throw new NotFoundException($"No key '{key}' exists.");
        }

        var newExpiresAt = DateTime.UtcNow + TimeSpan.FromSeconds((int)result.ExpirationPeriodInSeconds!);

        await _pairRepository.UpdateEpiresAt(result.Key, newExpiresAt);

        var value = DeserializeJson(result.Value);

        return new GetPairResponse
        {
            Key = result.Key,
            Value = value,
            ExpiresAt = newExpiresAt,
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

    private TimeSpan GetExpirationTimeSpanFromConfig()
    {
        string configString = _configuration["ExpirationPerionInSeconds"]
            ?? throw new ConfigException("ConfigException: ExpirationPeriodInSeconds could not be found.");

        if (!int.TryParse(configString, out int expirationTimespan))
            throw new ConfigException("ConfigException: ExpirationPeriodInSeconds can only have numbers.");

        return TimeSpan.FromSeconds(expirationTimespan);
    }

    private TimeSpan GetExpirationTimeSpan(int? expirationTimeStampProvided)
    {
        TimeSpan expirationTimeStampFromConfig = GetExpirationTimeSpanFromConfig();

        if (expirationTimeStampProvided == null)
            return expirationTimeStampFromConfig;

        if (expirationTimeStampProvided < 0)
            throw new ArgumentException("Given expiration period cannot be negative.");

        TimeSpan expirationTimeStamp = TimeSpan.FromSeconds((int)expirationTimeStampProvided);

        if (expirationTimeStamp < expirationTimeStampFromConfig)
            return expirationTimeStamp;

        return expirationTimeStampFromConfig;
    }
}
