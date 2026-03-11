using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Shipstone.Pollen.Api.Infrastructure.Data.Repositories;

internal sealed class Repository : IRepository
{
    private readonly IDataSource _dataSource;
    private readonly IServiceProvider _provider;

    IUserRepository IRepository.Users =>
        this._provider.GetRequiredService<IUserRepository>();

    IWeatherForecastRepository IRepository.WeatherForecasts =>
        this._provider.GetRequiredService<IWeatherForecastRepository>();

    public Repository(IServiceProvider provider, IDataSource dataSource)
    {
        ArgumentNullException.ThrowIfNull(provider);
        ArgumentNullException.ThrowIfNull(dataSource);
        this._dataSource = dataSource;
        this._provider = provider;
    }

    Task IRepository.SaveAsync(CancellationToken cancellationToken) =>
        this._dataSource.SaveAsync(cancellationToken);
}
