using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Application.Interfaces;

namespace Application.Services
{
    public class PairService : IPairService
    {
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

        public Task<GetPairResponse> Get(string key)
        {
            throw new NotImplementedException();
        }
    }
}
