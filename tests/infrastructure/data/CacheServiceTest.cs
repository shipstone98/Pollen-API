using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using Shipstone.Pollen.Api.Core.Pollen;
using Shipstone.Pollen.Api.Core.Services;
using Shipstone.Pollen.Api.Infrastructure.Data;
using Shipstone.Pollen.Api.Infrastructure.Entities;

using Shipstone.Pollen.Api.Infrastructure.DataTest.Mocks;
using Shipstone.Pollen.Api.Test.Mocks;
using Shipstone.Test.Mocks;

namespace Shipstone.Pollen.Api.Infrastructure.DataTest;

public sealed class CacheServiceTest
{
    private readonly ICacheService _cache;
    private readonly MockCoordinateService _coordinate;
    private readonly MockDistributedCache _mockCache;

    public CacheServiceTest()
    {
        // Arrange
        ICollection<ServiceDescriptor> collection =
            new List<ServiceDescriptor>();

        MockServiceCollection services = new();
        services._addAction = collection.Add;
        services._getEnumeratorFunc = collection.GetEnumerator;
        services.AddPollenInfrastructureData();
        MockDistributedCache cache = new();
        services.AddSingleton<IDistributedCache>(cache);
        MockCoordinateService coordinate = new();
        services.AddSingleton<ICoordinateService>(coordinate);
        IServiceProvider provider = new MockServiceProvider(services);
        this._cache = provider.GetRequiredService<ICacheService>();
        this._coordinate = coordinate;
        this._mockCache = cache;
    }

    [Fact]
    public async Task TestAddAsync_Invalid()
    {
        // Act
        ArgumentException ex =
            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                this._cache.AddAsync(null!, CancellationToken.None));

        // Assert
        Assert.Equal("weatherForecast", ex.ParamName);
    }

    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("[")]
    [InlineData("]")]
    [InlineData("[]")]
    [Theory]
    public async Task TestAddAsync_Valid(String? s)
    {
        // Arrange
        DateOnly date = DateOnly.FromDateTime(DateTime.UtcNow);
        DistributedCacheEntryOptions? options = null;

        this._mockCache._getFunc = _ =>
            s is null ? null : Encoding.UTF8.GetBytes(s);

        this._mockCache._setAction = (_, _, o) => options = o;

        // Act
        await this._cache.AddAsync(
            new WeatherForecastEntity
            {
                Date = date
            },
            CancellationToken.None
        );

        // Assert
        DateTime expiration =
            date
                .AddDays(1)
                .ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);

        Assert.Equal(expiration, options?.AbsoluteExpiration);
    }

    [Fact]
    public async Task TestListAsync_Cached()
    {
        // Arrange
        DateOnly date = new(2001, 2, 3);
        const double LATITUDE = 0.12345;
        const double LONGITUDE = 0.6789;
        this._coordinate._normalizeFunc = (_, _) => (LATITUDE, LONGITUDE);

        this._mockCache._getFunc = _ =>
            Encoding.UTF8.GetBytes("[{\"Date\":\"2001-02-03\",\"Latitude\":0.12345,\"Longitude\":0.6789,\"PollenCategory\":\"VeryHigh\",\"PollenColor\":{\"a\":50,\"r\":100,\"g\":150,\"b\":200},\"PollenType\":\"Grass\"}]");

        // Act
        IAsyncEnumerable<WeatherForecastEntity> result =
            this._cache.ListAsync(date, 0, 0, CancellationToken.None);

        // Assert
        await using IAsyncEnumerator<WeatherForecastEntity> enumerator =
            result.GetAsyncEnumerator(TestContext.Current.CancellationToken);

        Assert.True(await enumerator.MoveNextAsync());
        WeatherForecastEntity weatherForecast = enumerator.Current;
        Assert.Equal(date, weatherForecast.Date);
        Assert.Equal(LATITUDE, weatherForecast.Latitude);
        Assert.Equal(LONGITUDE, weatherForecast.Longitude);
        Assert.Equal(PollenCategory.VeryHigh, weatherForecast.PollenCategory);
        Assert.Equal(50, weatherForecast.PollenColor.A);
        Assert.Equal(200, weatherForecast.PollenColor.B);
        Assert.Equal(150, weatherForecast.PollenColor.G);
        Assert.Equal(100, weatherForecast.PollenColor.R);
        Assert.Equal(PollenType.Grass, weatherForecast.PollenType);
        Assert.False(await enumerator.MoveNextAsync());
    }

    [Fact]
    public async Task TestListAsync_NotCached()
    {
        // Arrange
        DateOnly date = DateOnly.FromDateTime(DateTime.UtcNow);
        this._coordinate._normalizeFunc = (_, _) => (0, 0);
        this._mockCache._getFunc = _ => null;

        // Act
        IAsyncEnumerable<Object> result =
            this._cache.ListAsync(date, 0, 0, CancellationToken.None);

        // Assert
        await using IAsyncEnumerator<Object> enumerator =
            result.GetAsyncEnumerator(TestContext.Current.CancellationToken);

        Assert.False(await enumerator.MoveNextAsync());
    }
}
