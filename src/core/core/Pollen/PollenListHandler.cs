using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

using Shipstone.Utilities.Linq;
using Shipstone.Utilities.Threading.Tasks;

using Shipstone.Pollen.Api.Core.Services;
using Shipstone.Pollen.Api.Infrastructure.Data;
using Shipstone.Pollen.Api.Infrastructure.Entities;
using Shipstone.Pollen.Api.Infrastructure.Weather;

namespace Shipstone.Pollen.Api.Core.Pollen;

internal sealed class PollenListHandler : IPollenListHandler
{
    private readonly ICacheService _cache;
    private readonly ICoordinateService _coordinate;
    private readonly IWeatherService _weather;

    public PollenListHandler(
        ICacheService cache,
        ICoordinateService coordinate,
        IWeatherService weather
    )
    {
        ArgumentNullException.ThrowIfNull(cache);
        ArgumentNullException.ThrowIfNull(coordinate);
        ArgumentNullException.ThrowIfNull(weather);
        this._cache = cache;
        this._coordinate = coordinate;
        this._weather = weather;
    }

    private async IAsyncEnumerable<IPollen> HandleAsync(
        double latitude,
        double longitude,
        [EnumeratorCancellation] CancellationToken cancellationToken
    )
    {
        DateOnly date = DateOnly.FromDateTime(DateTime.UtcNow);

        IAsyncEnumerable<WeatherForecastEntity> weatherForecasts =
            this._cache.ListAsync(
                date,
                latitude,
                longitude,
                cancellationToken
            );

        if (await weatherForecasts.AnyAsync(cancellationToken))
        {
            await foreach (WeatherForecastEntity weatherForecast in weatherForecasts)
            {
                yield return new Pollen(weatherForecast);
            }

            yield break;
        }

        weatherForecasts =
            this._weather
                .ListForecastsAsync(latitude, longitude, cancellationToken)
                .AsAsyncEnumerable();

        await foreach (WeatherForecastEntity weatherForecast in weatherForecasts)
        {
            await this._cache.AddAsync(weatherForecast, cancellationToken);
            yield return new Pollen(weatherForecast);
        }
    }

    IAsyncEnumerable<IPollen> IPollenListHandler.HandleAsync(
        double latitude,
        double longitude,
        CancellationToken cancellationToken
    )
    {
        this._coordinate.Normalize(ref latitude, ref longitude);
        return this.HandleAsync(latitude, longitude, cancellationToken);
    }
}
