using System;
using System.Collections.Generic;
using System.Threading;

namespace Shipstone.Pollen.Api.Core.Pollen;

/// <summary>
/// Defines a method to handle list pollen.
/// </summary>
public interface IPollenListHandler
{
    /// <summary>
    /// Asynchronously lists pollen for the specified coordinates.
    /// </summary>
    /// <param name="latitude">The latitude coordinate of the weather forecasts to list.</param>
    /// <param name="longitude">The longitude coordinate of the weather forecasts to list.</param>
    /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}" /> containing the listed pollen.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><c><paramref name="latitude" /></c> is less than -90 (negative ninety) -or- <c><paramref name="latitude" /></c> is greater than 90 (ninety) -or- <c><paramref name="longitude" /></c> is less than -180 (negative one-hundred and eighty) -or- <c><paramref name="longitude" /></c> is equal to or greater than 180 (one-hundred and eighty).</exception>
    /// <exception cref="OperationCanceledException">The cancellation token was canceled.</exception>
    IAsyncEnumerable<IPollen> HandleAsync(
        double latitude,
        double longitude,
        CancellationToken cancellationToken
    );
}
