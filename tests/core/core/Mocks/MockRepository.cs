using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Pollen.Api.Infrastructure.Data;

namespace Shipstone.Pollen.Api.CoreTest.Mocks;

internal sealed class MockRepository : IRepository
{
    internal Action _saveAction;
    internal Func<IUserRepository> _usersFunc;
    internal Func<IWeatherForecastRepository> _weatherForecastsFunc;

    IUserRepository IRepository.Users => this._usersFunc();

    IWeatherForecastRepository IRepository.WeatherForecasts =>
        this._weatherForecastsFunc();

    public MockRepository()
    {
        this._saveAction = () => throw new NotImplementedException();
        this._usersFunc = () => throw new NotImplementedException();
        this._weatherForecastsFunc = () => throw new NotImplementedException();
    }

    Task IRepository.SaveAsync(CancellationToken cancellationToken)
    {
        this._saveAction();
        return Task.CompletedTask;
    }
}
