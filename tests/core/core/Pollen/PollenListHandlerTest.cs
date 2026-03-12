using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using Shipstone.Pollen.Api.Core;
using Shipstone.Pollen.Api.Core.Pollen;
using Shipstone.Pollen.Api.Infrastructure.Data;
using Shipstone.Pollen.Api.Infrastructure.Entities;
using Shipstone.Pollen.Api.Infrastructure.Weather;

using Shipstone.Pollen.Api.CoreTest.Mocks;
using Shipstone.Test.Mocks;

namespace Shipstone.Pollen.Api.CoreTest.Pollen;

public sealed class PollenListHandlerTest
{
    private readonly MockCacheService _cache;
    private readonly IPollenListHandler _handler;
    private readonly MockWeatherService _weather;

    public PollenListHandlerTest()
    {
        ICollection<ServiceDescriptor> collection =
            new List<ServiceDescriptor>();

        MockServiceCollection services = new();
        services._addAction = collection.Add;
        services._getEnumeratorFunc = collection.GetEnumerator;
        services.AddPollenCore();
        MockCacheService cache = new();
        services.AddSingleton<ICacheService>(cache);
        MockWeatherService weather = new();
        services.AddSingleton<IWeatherService>(weather);
        IServiceProvider provider = new MockServiceProvider(services);
        this._cache = cache;
        this._handler = provider.GetRequiredService<IPollenListHandler>();
        this._weather = weather;
    }

#region HandleAsync method
    [Fact]
    public async Task TestHandleAsync_Contains()
    {
#region Arrange
        // Arrange
        const double LATITUDE = 0.1234;
        const double LONGITUDE = 0.5678;
        const PollenCategory GRASS_POLLEN_CATEGORY = PollenCategory.Moderate;
        const PollenCategory TREE_POLLEN_CATEGORY = PollenCategory.VeryLow;
        const PollenCategory WEED_POLLEN_CATEGORY = PollenCategory.VeryHigh;
        const byte GRASS_POLLEN_COLOR_RED = 100;
        const byte GRASS_POLLEN_COLOR_BLUE = 150;
        const byte GRASS_POLLEN_COLOR_GREEN = 200;
        const byte TREE_POLLEN_COLOR_RED = 110;
        const byte TREE_POLLEN_COLOR_BLUE = 160;
        const byte TREE_POLLEN_COLOR_GREEN = 210;
        const byte WEED_POLLEN_COLOR_RED = 120;
        const byte WEED_POLLEN_COLOR_BLUE = 170;
        const byte WEED_POLLEN_COLOR_GREEN = 220;

        this._cache._listFunc = (d, lat, lng) =>
        {
            IEnumerable<WeatherForecastEntity> weatherForecasts =
                new WeatherForecastEntity[]
                {
                    new WeatherForecastEntity
                    {
                        Date = d,
                        Latitude = lat,
                        Longitude = lng,
                        PollenCategory = GRASS_POLLEN_CATEGORY,
                        PollenColor =
                            Color.FromArgb(
                                GRASS_POLLEN_COLOR_RED,
                                GRASS_POLLEN_COLOR_GREEN,
                                GRASS_POLLEN_COLOR_BLUE
                            ),
                        PollenType = PollenType.Grass
                    },
                    new WeatherForecastEntity
                    {
                        Date = d,
                        Latitude = lat,
                        Longitude = lng,
                        PollenCategory = TREE_POLLEN_CATEGORY,
                        PollenColor =
                            Color.FromArgb(
                                TREE_POLLEN_COLOR_RED,
                                TREE_POLLEN_COLOR_GREEN,
                                TREE_POLLEN_COLOR_BLUE
                            ),
                        PollenType = PollenType.Tree
                    },
                    new WeatherForecastEntity
                    {
                        Date = d,
                        Latitude = lat,
                        Longitude = lng,
                        PollenCategory = WEED_POLLEN_CATEGORY,
                        PollenColor =
                            Color.FromArgb(
                                WEED_POLLEN_COLOR_RED,
                                WEED_POLLEN_COLOR_GREEN,
                                WEED_POLLEN_COLOR_BLUE
                            ),
                        PollenType = PollenType.Weed
                    },
                };

            return new MockAsyncEnumerable<WeatherForecastEntity>(weatherForecasts);
        };
#endregion

        // Act
        IAsyncEnumerable<IPollen> result =
            this._handler.HandleAsync(
                LATITUDE,
                LONGITUDE,
                CancellationToken.None
            );

#region Assert
        // Assert
        ISet<PollenType> types = new HashSet<PollenType>
        {
            PollenType.Grass,
            PollenType.Tree,
            PollenType.Weed
        };

        await foreach (IPollen pollen in result)
        {
            switch (pollen.Type)
            {
                case PollenType.Grass:
                    pollen.AssertEqual(
                        PollenType.Grass,
                        GRASS_POLLEN_CATEGORY,
                        GRASS_POLLEN_COLOR_RED,
                        GRASS_POLLEN_COLOR_GREEN,
                        GRASS_POLLEN_COLOR_BLUE
                    );

                    break;

                case PollenType.Tree:
                    pollen.AssertEqual(
                        PollenType.Tree,
                        TREE_POLLEN_CATEGORY,
                        TREE_POLLEN_COLOR_RED,
                        TREE_POLLEN_COLOR_GREEN,
                        TREE_POLLEN_COLOR_BLUE
                    );

                    break;

                case PollenType.Weed:
                    pollen.AssertEqual(
                        PollenType.Weed,
                        WEED_POLLEN_CATEGORY,
                        WEED_POLLEN_COLOR_RED,
                        WEED_POLLEN_COLOR_GREEN,
                        WEED_POLLEN_COLOR_BLUE
                    );

                    break;
            }

            Assert.True(types.Remove(pollen.Type));
        }
#endregion
    }

    [Fact]
    public async Task TestHandleAsync_NotContains_Empty()
    {
        // Arrange
        this._cache._listFunc = (_, _, _) =>
        {
            IEnumerable<WeatherForecastEntity> weatherForecasts =
                Array.Empty<WeatherForecastEntity>();

            return new MockAsyncEnumerable<WeatherForecastEntity>(weatherForecasts);
        };

        this._weather._listForecastsAsync = (_, _) =>
            Array.Empty<WeatherForecastEntity>();

        // Act
        IAsyncEnumerable<IPollen> result =
            this._handler.HandleAsync(0, 0, CancellationToken.None);

        // Assert
        await using IAsyncEnumerator<IPollen> enumerator =
            result.GetAsyncEnumerator();

        Assert.False(await enumerator.MoveNextAsync());
    }

    [Fact]
    public async Task TestHandleAsync_NotContains_NotEmpty()
    {
#region Arrange
        // Arrange
        const double LATITUDE = 0.1234;
        const double LONGITUDE = 0.5678;
        DateTime now = DateTime.UtcNow;
        DateOnly date = DateOnly.FromDateTime(now);
        const PollenCategory GRASS_POLLEN_CATEGORY = PollenCategory.Moderate;
        const PollenCategory TREE_POLLEN_CATEGORY = PollenCategory.VeryLow;
        const PollenCategory WEED_POLLEN_CATEGORY = PollenCategory.VeryHigh;
        const byte GRASS_POLLEN_COLOR_RED = 100;
        const byte GRASS_POLLEN_COLOR_BLUE = 150;
        const byte GRASS_POLLEN_COLOR_GREEN = 200;
        const byte TREE_POLLEN_COLOR_RED = 110;
        const byte TREE_POLLEN_COLOR_BLUE = 160;
        const byte TREE_POLLEN_COLOR_GREEN = 210;
        const byte WEED_POLLEN_COLOR_RED = 120;
        const byte WEED_POLLEN_COLOR_BLUE = 170;
        const byte WEED_POLLEN_COLOR_GREEN = 220;

        this._cache._listFunc = (_, _, _) =>
        {
            IEnumerable<WeatherForecastEntity> weatherForecasts =
                Array.Empty<WeatherForecastEntity>();

            return new MockAsyncEnumerable<WeatherForecastEntity>(weatherForecasts);
        };

        this._weather._listForecastsAsync = (lat, lng) =>
            new WeatherForecastEntity[]
            {
                new WeatherForecastEntity
                {
                    Date = date,
                    Latitude = lat,
                    Longitude = lng,
                    PollenCategory = GRASS_POLLEN_CATEGORY,
                    PollenColor =
                        Color.FromArgb(
                            GRASS_POLLEN_COLOR_RED,
                            GRASS_POLLEN_COLOR_GREEN,
                            GRASS_POLLEN_COLOR_BLUE
                        ),
                    PollenType = PollenType.Grass
                },
                new WeatherForecastEntity
                {
                    Date = date,
                    Latitude = lat,
                    Longitude = lng,
                    PollenCategory = TREE_POLLEN_CATEGORY,
                    PollenColor =
                        Color.FromArgb(
                            TREE_POLLEN_COLOR_RED,
                            TREE_POLLEN_COLOR_GREEN,
                            TREE_POLLEN_COLOR_BLUE
                        ),
                    PollenType = PollenType.Tree
                },
                new WeatherForecastEntity
                {
                    Date = date,
                    Latitude = lat,
                    Longitude = lng,
                    PollenCategory = WEED_POLLEN_CATEGORY,
                    PollenColor =
                        Color.FromArgb(
                            WEED_POLLEN_COLOR_RED,
                            WEED_POLLEN_COLOR_GREEN,
                            WEED_POLLEN_COLOR_BLUE
                        ),
                    PollenType = PollenType.Weed
                },
            };

        this._cache._addAction = _ => { };
#endregion

        // Act
        IAsyncEnumerable<IPollen> result =
            this._handler.HandleAsync(
                LATITUDE,
                LONGITUDE,
                CancellationToken.None
            );

#region Assert
        // Assert
        ISet<PollenType> types = new HashSet<PollenType>
        {
            PollenType.Grass,
            PollenType.Tree,
            PollenType.Weed
        };

        await foreach (IPollen pollen in result)
        {
            switch (pollen.Type)
            {
                case PollenType.Grass:
                    pollen.AssertEqual(
                        PollenType.Grass,
                        GRASS_POLLEN_CATEGORY,
                        GRASS_POLLEN_COLOR_RED,
                        GRASS_POLLEN_COLOR_GREEN,
                        GRASS_POLLEN_COLOR_BLUE
                    );

                    break;

                case PollenType.Tree:
                    pollen.AssertEqual(
                        PollenType.Tree,
                        TREE_POLLEN_CATEGORY,
                        TREE_POLLEN_COLOR_RED,
                        TREE_POLLEN_COLOR_GREEN,
                        TREE_POLLEN_COLOR_BLUE
                    );

                    break;

                case PollenType.Weed:
                    pollen.AssertEqual(
                        PollenType.Weed,
                        WEED_POLLEN_CATEGORY,
                        WEED_POLLEN_COLOR_RED,
                        WEED_POLLEN_COLOR_GREEN,
                        WEED_POLLEN_COLOR_BLUE
                    );

                    break;
            }

            Assert.True(types.Remove(pollen.Type));
        }
#endregion
    }
#endregion
}
