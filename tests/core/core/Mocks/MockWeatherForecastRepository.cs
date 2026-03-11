using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Pollen.Api.Infrastructure.Data;
using Shipstone.Pollen.Api.Infrastructure.Entities;

namespace Shipstone.Pollen.Api.CoreTest.Mocks;

internal sealed class MockWeatherForecastRepository
    : IWeatherForecastRepository
{
    internal Action<WeatherForecastEntity> _createAction;
    internal Func<DateOnly, double, double, WeatherForecastEntity[]> _listFunc;

    internal MockWeatherForecastRepository()
    {
        this._createAction = _ => throw new NotImplementedException();
        this._listFunc = (_, _, _) => throw new NotImplementedException();
    }

    Task IWeatherForecastRepository.CreateAsync(
        WeatherForecastEntity weatherForecast,
        CancellationToken cancellationToken
    )
    {
        this._createAction(weatherForecast);
        return Task.CompletedTask;
    }

    Task<WeatherForecastEntity[]> IWeatherForecastRepository.ListAsync(
        DateOnly date,
        double latitude,
        double longitude,
        CancellationToken cancellationToken
    )
    {
        WeatherForecastEntity[] result =
            this._listFunc(date, latitude, longitude);

        return Task.FromResult(result);
    }
}
