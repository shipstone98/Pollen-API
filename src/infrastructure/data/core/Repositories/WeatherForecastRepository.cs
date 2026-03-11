using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Shipstone.EntityFrameworkCore;

using Shipstone.Pollen.Api.Infrastructure.Entities;

namespace Shipstone.Pollen.Api.Infrastructure.Data.Repositories;

internal sealed class WeatherForecastRepository : IWeatherForecastRepository
{
    private readonly IDataSource _dataSource;

    public WeatherForecastRepository(IDataSource dataSource)
    {
        ArgumentNullException.ThrowIfNull(dataSource);
        this._dataSource = dataSource;
    }

    Task IWeatherForecastRepository.CreateAsync(
        WeatherForecastEntity weatherForecast,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(weatherForecast);

        return this._dataSource.WeatherForecasts.SetStateAsync(
            weatherForecast,
            DataEntityState.Created,
            cancellationToken
        );
    }

#warning Not tested
    Task<WeatherForecastEntity[]> IWeatherForecastRepository.ListAsync(
        DateOnly date,
        double latitude,
        double longitude,
        CancellationToken cancellationToken
    ) =>
        this._dataSource.WeatherForecasts
            .AsNoTracking()
            .Where(wf =>
                date.Equals(wf.Date)
                && latitude == wf.Latitude
                && longitude == wf.Longitude
            )
            .ToArrayAsync(cancellationToken);
}
