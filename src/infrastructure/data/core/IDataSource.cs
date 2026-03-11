using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.EntityFrameworkCore;

using Shipstone.Pollen.Api.Infrastructure.Entities;

namespace Shipstone.Pollen.Api.Infrastructure.Data;

/// <summary>
/// Represents a data source.
/// </summary>
public interface IDataSource
{
    /// <summary>
    /// Gets the user data set.
    /// </summary>
    /// <value>An <see cref="IDataSet{TEntity}" /> containing the users.</value>
    IDataSet<UserEntity> Users { get; }

    /// <summary>
    /// Gets the weather forecast data set.
    /// </summary>
    /// <value>An <see cref="IDataSet{TEntity}" /> containing the weather forecasts.</value>
    IDataSet<WeatherForecastEntity> WeatherForecasts { get; }

    /// <summary>
    /// Asynchronously saves all changes to the data source.
    /// </summary>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> that represents the asynchronous save operation.</returns>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    Task SaveAsync(CancellationToken cancellationToken);
}
