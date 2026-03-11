using System;

namespace Shipstone.Pollen.Api.Core.Services;

internal sealed class CoordinateService : ICoordinateService
{
    void ICoordinateService.Normalize(
        ref double latitude,
        ref double longitude
    )
    {
        if (Double.IsNaN(latitude))
        {
            throw new ArgumentException(
                $"{nameof (latitude)} is Double.NaN.",
                nameof (latitude)
            );
        }

        if (Double.IsNegativeInfinity(latitude))
        {
            throw new ArgumentException(
                $"{nameof (latitude)} is Double.NegativeInfinity.",
                nameof (latitude)
            );
        }

        if (Double.IsPositiveInfinity(latitude))
        {
            throw new ArgumentException(
                $"{nameof (latitude)} is Double.PositiveInfinity.",
                nameof (latitude)
            );
        }

        ArgumentOutOfRangeException.ThrowIfLessThan(latitude, -90);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(latitude, 90);

        if (Double.IsNaN(longitude))
        {
            throw new ArgumentException(
                $"{nameof (longitude)} is Double.NaN.",
                nameof (longitude)
            );
        }

        if (Double.IsNegativeInfinity(longitude))
        {
            throw new ArgumentException(
                $"{nameof (longitude)} is Double.NegativeInfinity.",
                nameof (longitude)
            );
        }

        if (Double.IsPositiveInfinity(longitude))
        {
            throw new ArgumentException(
                $"{nameof (longitude)} is Double.PositiveInfinity.",
                nameof (longitude)
            );
        }

        ArgumentOutOfRangeException.ThrowIfLessThan(longitude, -180);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(longitude, 180);
        MidpointRounding mode = MidpointRounding.AwayFromZero;
        latitude = Math.Round(latitude, 4, mode);
        longitude = Math.Round(longitude, 4, mode);

        if (longitude == 180)
        {
            longitude = 179.9999;
        }
    }
}
