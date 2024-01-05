using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces;
using Domain.Exceptions;
using Domain.Interfaces;
using Microsoft.Extensions.Configuration;

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

    public async Task Delete(string key)
    {
       var keyValue =await _pairRepository.GetByKey(key);
        if (keyValue == null)
        {
            throw new NotFoundException("Dont have this a key");
        }
        await _pairRepository.Delete(key);
    }

    public Task<GetPairResponse> Get(string key)
    {
        throw new NotImplementedException();
    }

    
}
