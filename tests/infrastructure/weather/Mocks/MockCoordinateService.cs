using System;

using Shipstone.Pollen.Api.Core.Services;

namespace Shipstone.Pollen.Api.Infrastructure.WeatherTest.Mocks;

internal sealed class MockCoordinateService : ICoordinateService
{
    internal Func<double, double, (double Latitude, double Longitude)> _normalizeFunc;

    public MockCoordinateService() =>
        this._normalizeFunc = (_, _) => throw new NotImplementedException();

    void ICoordinateService.Normalize(
        ref double latitude,
        ref double longitude
    ) =>
        (latitude, longitude) = this._normalizeFunc(latitude, longitude);
}
