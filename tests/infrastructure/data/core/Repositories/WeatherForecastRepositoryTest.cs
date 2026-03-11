using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using Shipstone.Pollen.Api.Infrastructure.Data;
using Shipstone.Pollen.Api.Infrastructure.Entities;

using Shipstone.Pollen.Api.Infrastructure.DataTest.Mocks;
using Shipstone.Test.Mocks;

namespace Shipstone.Pollen.Api.Infrastructure.DataTest.Repositories;

public sealed class WeatherForecastRepositoryTest
{
    private readonly MockDataSource _dataSource;
    private readonly IWeatherForecastRepository _repository;

    public WeatherForecastRepositoryTest()
    {
        ICollection<ServiceDescriptor> collection =
            new List<ServiceDescriptor>();

        MockServiceCollection services = new();
        services._addAction = collection.Add;
        services._getEnumeratorFunc = collection.GetEnumerator;
        services.AddPollenInfrastructureData();
        MockDataSource dataSource = new();
        services.AddSingleton<IDataSource>(dataSource);
        IServiceProvider provider = new MockServiceProvider(services);
        this._dataSource = dataSource;
        this._repository = provider.GetRequiredService<IWeatherForecastRepository>();
    }

    [Fact]
    public async Task TestCreateAsync_Invalid()
    {
        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._repository.CreateAsync(null!, CancellationToken.None));

        // Assert
        Assert.Equal("weatherForecast", ex.ParamName);
    }

    [Fact]
    public Task TestCreateAsync_Valid()
    {
        // Arrange
        WeatherForecastEntity weatherForecast = new();

        this._dataSource._weatherForecastsFunc = () =>
        {
            IQueryable<WeatherForecastEntity> query =
                Array
                    .Empty<WeatherForecastEntity>()
                    .AsQueryable();

            MockDataSet<WeatherForecastEntity> dataSet = new(query);
            dataSet._setStateAction = (_, _) => { };
            return dataSet;
        };

        // Act
        return this._repository.CreateAsync(weatherForecast, CancellationToken.None);

        // Nothing to assert
    }
}
