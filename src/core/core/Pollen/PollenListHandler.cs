using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

using Shipstone.Utilities.Linq;

using Shipstone.Pollen.Api.Core.Services;
using Shipstone.Pollen.Api.Infrastructure.Data;
using Shipstone.Pollen.Api.Infrastructure.Entities;
using Shipstone.Pollen.Api.Infrastructure.Weather;

namespace Shipstone.Pollen.Api.Core.Pollen;

internal sealed class PollenListHandler : IPollenListHandler
{
    private readonly ICoordinateService _coordinate;
    private readonly IRepository _repository;
    private readonly IWeatherService _weather;

    public PollenListHandler(
        IRepository repository,
        ICoordinateService coordinate,
        IWeatherService weather
    )
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(coordinate);
        ArgumentNullException.ThrowIfNull(weather);
        this._coordinate = coordinate;
        this._repository = repository;
        this._weather = weather;
    }

    private async IAsyncEnumerable<WeatherForecastEntity> CreateAsync(
        double latitude,
        double longitude,
        [EnumeratorCancellation] CancellationToken cancellationToken
    )
    {
        IEnumerable<WeatherForecastEntity> weatherForecasts =
            await this._weather.ListForecastsAsync(
                latitude,
                longitude,
                cancellationToken
            );

        if (!weatherForecasts.Any())
        {
            yield break;
        }

        foreach (WeatherForecastEntity weatherForecast in weatherForecasts)
        {
            await this._repository.WeatherForecasts.CreateAsync(
                weatherForecast,
                cancellationToken
            );

            yield return weatherForecast;
        }

        await this._repository.SaveAsync(cancellationToken);
    }

    private async IAsyncEnumerable<IPollen> HandleAsync(
        double latitude,
        double longitude,
        [EnumeratorCancellation] CancellationToken cancellationToken
    )
    {
        DateOnly date = DateOnly.FromDateTime(DateTime.UtcNow.Date);

        IEnumerable<WeatherForecastEntity> weatherForecasts =
            await this._repository.WeatherForecasts.ListAsync(
                date,
                latitude,
                longitude,
                cancellationToken
            );
        
        if (!weatherForecasts.Any())
        {
            weatherForecasts =
                await this
                    .CreateAsync(latitude, longitude, cancellationToken)
                    .ToListAsync(cancellationToken);
        }

        foreach (WeatherForecastEntity weatherForecast in weatherForecasts)
        {
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
