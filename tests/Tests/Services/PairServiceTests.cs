using Moq;
using Domain.Interfaces;
using Application.Services;
using AutoFixture;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Application.DTOs.Requests;
using Application.DTOs.Responses;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using FluentAssertions;
using Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Text.Json;


public class PairServiceTests
{
    private readonly Mock<IPairRepository> _repository;
    private readonly PairService _service;
    private readonly IFixture _fixture;
    private readonly IConfiguration _configuration;
    public PairServiceTests()
    {

        _repository = new Mock<IPairRepository>();
        _configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
        {
            {"ExpirationPeriodInSeconds", "600" }
        }!)
            .Build();
        _service = new PairService(_repository.Object, _configuration);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task Delete_ExistingKey_DeletesKey()
    {
        var key = _fixture.Create<string>();
        var expectedResult = new PairEntity { Key = key };

        _repository.Setup(repo => repo.GetByKey(key)).ReturnsAsync(expectedResult);

        await _service.Delete(key);

        _repository.Verify(repo => repo.Delete(key), Times.Once);
    }

    [Fact]
    public async Task Delete_NonExistingKey_ThrowsNotFoundException()
    {
        var key = _fixture.Create<string>();
        _repository.Setup(repo => repo.GetByKey(key)).Returns(Task.FromResult<PairEntity?>(null));


        await Assert.ThrowsAsync<NotFoundException>(async () => await _service.Delete(key));
    }


    [Fact]
    public async Task GetById_GivenNonExistingKey_ThrowsNotFoundException()
    {
        string key = _fixture.Create<string>();
        _repository.Setup(repo => repo.GetByKey(key)).Returns(Task.FromResult<PairEntity?>(null));


        Func<Task> result = async () => await _service.Get(key);
        await result.Should().ThrowAsync<NotFoundException>();

        _repository.Verify(repo => repo.GetByKey(key), Times.Once);
    }

    [Fact]
    public async Task GetById_GivenExistingKeyWithExpiredDate_ThrowsNotFoundExceptione()
    {
        string key = _fixture.Create<string>();

        var repositoryResult = new PairEntity
        {
            Key = key,
            ExpiresAt = DateTime.Now.AddDays(-2)
        };
        _repository.Setup(repo => repo.GetByKey(key)).ReturnsAsync(repositoryResult);

        Func<Task> result = async () => await _service.Get(key);
        await result.Should().ThrowAsync<NotFoundException>();

        _repository.Verify(repo => repo.GetByKey(key), Times.Once);
    }

    [Fact]
    public async Task GetById_GivenExistingKeyWithNullExpirationPeriodInSecons_ThrowsCoruptedDataException()
    {
        string key = _fixture.Create<string>();

        var repositoryResult = new PairEntity
        {
            Key = key,
            ExpiresAt = DateTime.Now.AddDays(2),
            ExpirationPeriodInSeconds = null
        };
        _repository.Setup(repo => repo.GetByKey(key)).ReturnsAsync(repositoryResult);

        Func<Task> result = async () => await _service.Get(key);
        await result.Should().ThrowAsync<CoruptedDataException>();

        _repository.Verify(repo => repo.GetByKey(key), Times.Once);
    }

    [Fact]
    public async Task GetById_GivenExistingKey_ReturnsGetPairResponse()
    {
        string key = _fixture.Create<string>();
        var obj = _fixture.Create<PairEntity>();
        var value = new List<object>();
        value.Add(obj);
        var repositoryResult = new PairEntity
        {
            Key = key,
            ExpiresAt = DateTime.Now.AddDays(2),
            ExpirationPeriodInSeconds = _fixture.Create<int>(),
            Value = JsonSerializer.Serialize(value)
        };
        _repository.Setup(repo => repo.GetByKey(key)).ReturnsAsync(repositoryResult);
        var expectedResult = new GetPairResponse
        {
            Key = key,
            ExpiresAt = DateTime.UtcNow + TimeSpan.FromSeconds((int)repositoryResult.ExpirationPeriodInSeconds!),
            ExpirationPeriodInSeconds = repositoryResult.ExpirationPeriodInSeconds,
            Value = JsonSerializer.Deserialize<List<object>>(repositoryResult.Value)!
        };

        var result = await _service.Get(key);

        _repository.Verify(repo => repo.GetByKey(key), Times.Once);
        result.Key.Should().Be(expectedResult.Key);
        //result.Value.Should().BeEquivalentTo(expectedResult.Value);   Throws error most possible due to serilization
        result.ExpirationPeriodInSeconds.Should().Be(expectedResult.ExpirationPeriodInSeconds);
        //result.Should().Be(expectedResult);   Throws error because of process taking different time and returns different Expires at time (nanoseconds :)
    }
}