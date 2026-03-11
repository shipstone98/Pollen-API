using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.EntityFrameworkCore;

using Shipstone.Pollen.Api.Infrastructure.Data;
using Shipstone.Pollen.Api.Infrastructure.Entities;

namespace Shipstone.Pollen.Api.Infrastructure.DataTest.Mocks;

internal sealed class MockDataSource : IDataSource
{
    internal Action _saveAction;
    internal Func<IDataSet<UserEntity>> _usersFunc;
    internal Func<IDataSet<WeatherForecastEntity>> _weatherForecastsFunc;

    IDataSet<UserEntity> IDataSource.Users => this._usersFunc();

    IDataSet<WeatherForecastEntity> IDataSource.WeatherForecasts =>
        this._weatherForecastsFunc();

    public MockDataSource()
    {
        this._saveAction = () => throw new NotImplementedException();
        this._usersFunc = () => throw new NotImplementedException();
        this._weatherForecastsFunc = () => throw new NotImplementedException();
    }

    Task IDataSource.SaveAsync(CancellationToken cancellationToken)
    {
        this._saveAction();
        return Task.CompletedTask;
    }
}
