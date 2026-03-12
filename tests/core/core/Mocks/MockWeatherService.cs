using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Pollen.Api.Infrastructure.Entities;
using Shipstone.Pollen.Api.Infrastructure.Weather;

namespace Shipstone.Pollen.Api.CoreTest.Mocks;

internal sealed class MockWeatherService : IWeatherService
{
    internal Func<double, double, WeatherForecastEntity[]> _listForecastsAsync;

    public MockWeatherService() =>
        this._listForecastsAsync = (_, _) =>
            throw new NotImplementedException();

    Task<WeatherForecastEntity[]> IWeatherService.ListForecastsAsync(
        double latitude,
        double longitude,
        CancellationToken cancellationToken
    )
    {
        WeatherForecastEntity[] result =
            this._listForecastsAsync(latitude, longitude);

        return Task.FromResult(result);
    }
}
