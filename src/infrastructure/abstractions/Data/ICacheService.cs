using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Pollen.Api.Infrastructure.Entities;

namespace Shipstone.Pollen.Api.Infrastructure.Data;

/// <summary>
/// Defines methods to manipulate a cache.
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Asynchronously adds the specified weather forecast to the cache.
    /// </summary>
    /// <param name="weatherForecast">The weather forecast to add to the cache.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task" /> that represents the asynchronous add operation.</returns>
    /// <exception cref="ArgumentNullException"><c><paramref name="weatherForecast" /></c> is <c>null</c>.</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    Task AddAsync(
        WeatherForecastEntity weatherForecast,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Asynchronously lists specified weather forecast with the specified properties in the cache.
    /// </summary>
    /// <param name="date">The date of the weather forecasts to list.</param>
    /// <param name="latitude">The latitude coordinate of the weather forecasts to list.</param>
    /// <param name="longitude">The longitude coordinate of the weather forecasts to list.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}" /> that contains the listed weather forecasts.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><c><paramref name="latitude" /></c> is less than -90 (negative ninety) -or- <c><paramref name="latitude" /></c> is greater than 90 (ninety) -or- <c><paramref name="longitude" /></c> is less than -180 (negative one-hundred and eighty) -or- <c><paramref name="longitude" /></c> is equal to or greater than 180 (one-hundred and eighty).</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    IAsyncEnumerable<WeatherForecastEntity> ListAsync(
        DateOnly date,
        double latitude,
        double longitude,
        CancellationToken cancellationToken
    );
}
