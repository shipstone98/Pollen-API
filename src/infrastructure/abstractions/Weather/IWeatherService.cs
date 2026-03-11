using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Pollen.Api.Infrastructure.Entities;

namespace Shipstone.Pollen.Api.Infrastructure.Weather;

/// <summary>
/// Defines a method to list weather forecasts.
/// </summary>
public interface IWeatherService
{
    /// <summary>
    /// Asynchronously lists weather forecasts for the specified coordinates.
    /// </summary>
    /// <param name="latitude">The latitude coordinate of the weather forecasts to list.</param>
    /// <param name="longitude">The longitude coordinate of the weather forecasts to list.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Task{TResult}" /> that represents the asynchronous list operation. The value of <see cref="Task{TResult}.Result" /> contains the listed weather forecasts.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><c><paramref name="latitude" /></c> is less than -90 (negative ninety) -or- <c><paramref name="latitude" /></c> is greater than 90 (ninety) -or- <c><paramref name="longitude" /></c> is less than -180 (negative one-hundred and eighty) -or- <c><paramref name="longitude" /></c> is equal to or greater than 180 (one-hundred and eighty).</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    /// <exception cref="WeatherException">The weather forecasts could not be listed.</exception>
    Task<IEnumerable<WeatherForecastEntity>> ListForecastsAsync(
        double latitude,
        double longitude,
        CancellationToken cancellationToken
    );
}
