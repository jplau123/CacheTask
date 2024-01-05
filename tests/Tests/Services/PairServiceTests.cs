using Moq;
using Domain.Interfaces;
using Application.Services;
using AutoFixture;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Configuration;


public class PairServiceTests
{
    private readonly Mock<IPairRepository> _testRepository;
    private readonly PairService _pairService;
    private readonly IFixture _fixture;
    private readonly IConfiguration _configuration;
    public PairServiceTests(IConfiguration configuration)
    {

        _testRepository = new Mock<IPairRepository>();
        _configuration = configuration;
        _pairService = new PairService(_testRepository.Object, _configuration);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task Delete_ExistingKey_DeletesKey()
    {
        var key = _fixture.Create<string>();
        var keyValue = new PairEntity { Key = key };

        var configuration = new Mock<IConfiguration>();
        

        _testRepository.Setup(repo => repo.GetByKey(key)).ReturnsAsync(keyValue);

        var pairService = new PairService(_testRepository.Object, configuration.Object);

        await pairService.Delete(key);

        _testRepository.Verify(repo => repo.Delete(key), Times.Once);
    }


    //[Fact]
    //public async Task Delete_NonExistingKey_ThrowsNotFoundException()
    //{
    //    var key = "nonExistingKey";
    //    var pairRepositoryMock = new Mock<IPairRepository>();
    //    pairRepositoryMock.Setup(repo => repo.GetByKey(key)).ReturnsAsync((YourValueType)null); 
    //    var yourClass = new PairService(pairRepositoryMock.Object);


    //    await Assert.ThrowsAsync<NotFoundException>(async () => await yourClass.Delete(key));
    //}
}
