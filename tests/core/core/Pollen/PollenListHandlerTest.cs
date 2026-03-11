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
    private readonly IPollenListHandler _handler;
    private readonly MockRepository _repository;
    private readonly MockWeatherService _weather;

    public PollenListHandlerTest()
    {
        ICollection<ServiceDescriptor> collection =
            new List<ServiceDescriptor>();

        MockServiceCollection services = new();
        services._addAction = collection.Add;
        services._getEnumeratorFunc = collection.GetEnumerator;
        services.AddPollenCore();
        MockRepository repository = new();
        services.AddSingleton<IRepository>(repository);
        MockWeatherService weather = new();
        services.AddSingleton<IWeatherService>(weather);
        IServiceProvider provider = new MockServiceProvider(services);
        this._handler = provider.GetRequiredService<IPollenListHandler>();
        this._repository = repository;
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
        const long GRASS_POLLEN_ID = 12345;
        const long TREE_POLLEN_ID = 23456;
        const long WEED_POLLEN_ID = 34567;
        DateTime now = DateTime.UtcNow;
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

        this._repository._weatherForecastsFunc = () =>
        {
            MockWeatherForecastRepository weatherForecasts = new();

            weatherForecasts._listFunc = (d, lat, lng) =>
                new WeatherForecastEntity[]
                {
                    new WeatherForecastEntity
                    {
                        Created = now,
                        Date = d,
                        Id = GRASS_POLLEN_ID,
                        Latitude = lat,
                        Longitude = lng,
                        PollenCategory = GRASS_POLLEN_CATEGORY,
                        PollenColor =
                            Color.FromArgb(
                                GRASS_POLLEN_COLOR_RED,
                                GRASS_POLLEN_COLOR_GREEN,
                                GRASS_POLLEN_COLOR_BLUE
                            ),
                        PollenType = PollenType.Grass,
                        Updated = now
                    },
                    new WeatherForecastEntity
                    {
                        Created = now,
                        Date = d,
                        Id = TREE_POLLEN_ID,
                        Latitude = lat,
                        Longitude = lng,
                        PollenCategory = TREE_POLLEN_CATEGORY,
                        PollenColor =
                            Color.FromArgb(
                                TREE_POLLEN_COLOR_RED,
                                TREE_POLLEN_COLOR_GREEN,
                                TREE_POLLEN_COLOR_BLUE
                            ),
                        PollenType = PollenType.Tree,
                        Updated = now
                    },
                    new WeatherForecastEntity
                    {
                        Created = now,
                        Date = d,
                        Id = WEED_POLLEN_ID,
                        Latitude = lat,
                        Longitude = lng,
                        PollenCategory = WEED_POLLEN_CATEGORY,
                        PollenColor =
                            Color.FromArgb(
                                WEED_POLLEN_COLOR_RED,
                                WEED_POLLEN_COLOR_GREEN,
                                WEED_POLLEN_COLOR_BLUE
                            ),
                        PollenType = PollenType.Weed,
                        Updated = now
                    },
                };

            return weatherForecasts;
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
                        GRASS_POLLEN_ID,
                        PollenType.Grass,
                        GRASS_POLLEN_CATEGORY,
                        GRASS_POLLEN_COLOR_RED,
                        GRASS_POLLEN_COLOR_GREEN,
                        GRASS_POLLEN_COLOR_BLUE
                    );

                    break;

                case PollenType.Tree:
                    pollen.AssertEqual(
                        TREE_POLLEN_ID,
                        PollenType.Tree,
                        TREE_POLLEN_CATEGORY,
                        TREE_POLLEN_COLOR_RED,
                        TREE_POLLEN_COLOR_GREEN,
                        TREE_POLLEN_COLOR_BLUE
                    );

                    break;

                case PollenType.Weed:
                    pollen.AssertEqual(
                        WEED_POLLEN_ID,
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
        this._repository._weatherForecastsFunc = () =>
        {
            MockWeatherForecastRepository weatherForecasts = new();

            weatherForecasts._listFunc = (_, _, _) =>
                Array.Empty<WeatherForecastEntity>();

            return weatherForecasts;
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

        this._repository._weatherForecastsFunc = () =>
        {
            MockWeatherForecastRepository weatherForecasts = new();

            weatherForecasts._listFunc = (_, _, _) =>
                Array.Empty<WeatherForecastEntity>();

            long id = 0;
            weatherForecasts._createAction = wf => wf.SetId(++ id);
            return weatherForecasts;
        };

        this._weather._listForecastsAsync = (lat, lng) =>
            new WeatherForecastEntity[]
            {
                new WeatherForecastEntity
                {
                    Created = now,
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
                    PollenType = PollenType.Grass,
                    Updated = now
                },
                new WeatherForecastEntity
                {
                    Created = now,
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
                    PollenType = PollenType.Tree,
                    Updated = now
                },
                new WeatherForecastEntity
                {
                    Created = now,
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
                    PollenType = PollenType.Weed,
                    Updated = now
                },
            };

        this._repository._saveAction = () => { };
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
                        1,
                        PollenType.Grass,
                        GRASS_POLLEN_CATEGORY,
                        GRASS_POLLEN_COLOR_RED,
                        GRASS_POLLEN_COLOR_GREEN,
                        GRASS_POLLEN_COLOR_BLUE
                    );

                    break;

                case PollenType.Tree:
                    pollen.AssertEqual(
                        1,
                        PollenType.Tree,
                        TREE_POLLEN_CATEGORY,
                        TREE_POLLEN_COLOR_RED,
                        TREE_POLLEN_COLOR_GREEN,
                        TREE_POLLEN_COLOR_BLUE
                    );

                    break;

                case PollenType.Weed:
                    pollen.AssertEqual(
                        1,
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
