using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Pollen.Api.Infrastructure.Entities;

namespace Shipstone.Pollen.Api.Infrastructure.Data;

/// <summary>
/// Represents a weather forecast repository.
/// </summary>
public interface IWeatherForecastRepository
{
    /// <summary>
    /// Asynchronously creates a weather forecast with the specified properties.
    /// </summary>
    /// <param name="weatherForecast">The weather forecast to create.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> that represents the asynchronous create operation.</returns>
    /// <exception cref="ArgumentNullException"><c><paramref name="weatherForecast" /></c> is <c>null</c>.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    Task CreateAsync(
        WeatherForecastEntity weatherForecast,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Asynchronously lists weather forecasts for the specified date and coordinates.
    /// </summary>
    /// <param name="date">The date of the weather forecasts to list.</param>
    /// <param name="latitude">The latitude coordinate of the weather forecasts to list.</param>
    /// <param name="longitude">The longitude coordinate of the weather forecasts to list.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task{TResult}" /> that represents the asynchronous list operation. The value of <see cref="Task{TResult}.Result" /> contains the listed weather forecasts.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><c><paramref name="latitude" /></c> is less than -90 (negative ninety) -or- <c><paramref name="latitude" /></c> is greater than 90 (ninety) -or- <c><paramref name="longitude" /></c> is less than -180 (negative one-hundred and eighty) -or- <c><paramref name="longitude" /></c> is equal to or greater than 180 (one-hundred and eighty).</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    Task<WeatherForecastEntity[]> ListAsync(
        DateOnly date,
        double latitude,
        double longitude,
        CancellationToken cancellationToken
    );
}
