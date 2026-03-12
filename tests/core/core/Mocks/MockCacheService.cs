using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Pollen.Api.Infrastructure.Data;
using Shipstone.Pollen.Api.Infrastructure.Entities;

namespace Shipstone.Pollen.Api.CoreTest.Mocks;

internal sealed class MockCacheService : ICacheService
{
    internal Action<WeatherForecastEntity> _addAction;
    internal Func<DateOnly, double, double, IAsyncEnumerable<WeatherForecastEntity>> _listFunc;

    public MockCacheService()
    {
        this._addAction = _ => throw new NotImplementedException();
        this._listFunc = (_, _, _) => throw new NotImplementedException();
    }

    Task ICacheService.AddAsync(
        WeatherForecastEntity weatherForecast,
        CancellationToken cancellationToken
    )
    {
        this._addAction(weatherForecast);
        return Task.CompletedTask;
    }

    IAsyncEnumerable<WeatherForecastEntity> ICacheService.ListAsync(
        DateOnly date,
        double latitude,
        double longitude,
        CancellationToken cancellationToken
    ) =>
        this._listFunc(date, latitude, longitude);
}
