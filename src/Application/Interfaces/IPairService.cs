using Application.DTOs.Requests;
using Application.DTOs.Responses;

namespace Application.Interfaces;

public interface IPairService
{
    Task<CreatePairResponse> Create(CreatePairRequest request);
    Task<AppendPairResponse> Append(AppendPairRequest request);
    Task Delete(string key);
    Task<GetPairResponse> Get(string key);
}
