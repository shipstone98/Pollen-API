using System;

namespace Shipstone.Pollen.Api.Core.Services;

/// <summary>
/// Defines a method to normalize coordinates.
/// </summary>
public interface ICoordinateService
{
    /// <summary>
    /// Normalizes the specified coordinates.
    /// </summary>
    /// <param name="latitude">The latitude coordinate to normalize. This value is passed by reference.</param>
    /// <param name="longitude">The longitude coordinate to normalize. This value is passed by reference.</param>
    /// <exception cref="ArgumentException"><c><paramref name="latitude" /></c> is <see cref="Double.NaN" /> -or- <c><paramref name="latitude" /></c> is <see cref="Double.NegativeInfinity" /> -or- <c><paramref name="latitude" /></c> is <see cref="Double.PositiveInfinity" /> -or- <c><paramref name="longitude" /></c> is <see cref="Double.NaN" /> -or- <c><paramref name="longitude" /></c> is <see cref="Double.NegativeInfinity" /> -or- <c><paramref name="longitude" /></c> is <see cref="Double.PositiveInfinity" />.</exception> 
    /// <exception cref="ArgumentOutOfRangeException"><c><paramref name="latitude" /></c> is less than -90 (negative ninety) -or- <c><paramref name="latitude" /></c> is greater than 90 (ninety) -or- <c><paramref name="longitude" /></c> is less than -180 (negative one-hundred and eighty) -or- <c><paramref name="longitude" /></c> is equal to or greater than 180 (one-hundred and eighty).</exception> 
    void Normalize(ref double latitude, ref double longitude);
}
