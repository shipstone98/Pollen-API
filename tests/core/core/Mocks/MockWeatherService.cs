using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Pollen.Api.Infrastructure.Entities;
using Shipstone.Pollen.Api.Infrastructure.Weather;

namespace Shipstone.Pollen.Api.CoreTest.Mocks;

internal sealed class MockWeatherService : IWeatherService
{
    internal Func<double, double, IEnumerable<WeatherForecastEntity>> _listForecastsAsync;

    public MockWeatherService() =>
        this._listForecastsAsync = (_, _) =>
            throw new NotImplementedException();

    Task<IEnumerable<WeatherForecastEntity>> IWeatherService.ListForecastsAsync(
        double latitude,
        double longitude,
        CancellationToken cancellationToken
    )
    {
        IEnumerable<WeatherForecastEntity> result =
            this._listForecastsAsync(latitude, longitude);

        return Task.FromResult(result);
    }
}
