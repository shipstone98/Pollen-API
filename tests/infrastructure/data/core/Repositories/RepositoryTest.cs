using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using Shipstone.Extensions.Security;

using Shipstone.Pollen.Api.Infrastructure.Data;

using Shipstone.Pollen.Api.Infrastructure.DataTest.Mocks;
using Shipstone.Test.Mocks;

namespace Shipstone.Pollen.Api.Infrastructure.DataTest.Repositories;

public sealed class RepositoryTest
{
    private readonly MockDataSource _dataSource;
    private readonly IRepository _repository;

    public RepositoryTest()
    {
        ICollection<ServiceDescriptor> collection =
            new List<ServiceDescriptor>();

        MockServiceCollection services = new();
        services._addAction = collection.Add;
        services._getEnumeratorFunc = collection.GetEnumerator;
        services.AddPollenInfrastructureData();
        MockDataSource dataSource = new();
        services.AddSingleton<IDataSource>(dataSource);
        services.AddSingleton<INormalizationService, MockNormalizationService>();
        IServiceProvider provider = new MockServiceProvider(services);
        this._dataSource = dataSource;
        this._repository = provider.GetRequiredService<IRepository>();
    }

    [Fact]
    public void TestUsers_Get()
    {
        // Act and assert
        Assert.NotNull(this._repository.Users);
    }

    [Fact]
    public void TestWeatherForecasts_Get()
    {
        // Act and assert
        Assert.NotNull(this._repository.WeatherForecasts);
    }

    [Fact]
    public Task TestSaveAsync()
    {
        // Arrange
        this._dataSource._saveAction = () => { };

        // Act
        return this._repository.SaveAsync(CancellationToken.None);

        // Nothing to assert
    }
}
