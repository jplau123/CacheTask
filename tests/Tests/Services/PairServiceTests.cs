using Moq;
using Domain.Interfaces;
using Application.Services;
using AutoFixture;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Domain.Exceptions;


public class PairServiceTests
{
    private readonly Mock<IPairRepository> _testRepository;
    private readonly PairService _pairService;
    private readonly IFixture _fixture;
    private readonly IConfiguration _configuration;
   
    public PairServiceTests()
    {
        _testRepository = new Mock<IPairRepository>();
        _fixture = new Fixture();
        _configuration = new ConfigurationBuilder().Build();
        _pairService = new PairService(_testRepository.Object, _configuration);
    }

    [Fact]
    public async Task Delete_ExistingKey_DeletesKey()
    {
        var key = _fixture.Create<string>();
        var keyValue = new PairEntity { Key = key };
        _testRepository.Setup(repo => repo.GetByKey(key)).ReturnsAsync(keyValue);
        await _pairService.Delete(key);
        _testRepository.Verify(repo => repo.Delete(key), Times.Once);
    }

    [Fact]
    public async Task Delete_NotExistingKeyGiven_ThrowsNotFoundException()
    {
        var key = _fixture.Create<string>();
        _testRepository.Setup(repo => repo.GetByKey(key)).Returns(Task.FromResult<PairEntity?>(null));
        await Assert.ThrowsAsync<NotFoundException>(() => _pairService.Delete(key));
    }
}
