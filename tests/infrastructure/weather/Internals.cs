using System;
using Xunit;

using Shipstone.Pollen.Api.Core.Pollen;
using Shipstone.Pollen.Api.Infrastructure.Entities;

namespace Shipstone.Pollen.Api.Infrastructure.WeatherTest;

internal static class Internals
{
    internal static void AssertEqual(
        this WeatherForecastEntity weatherForecast,
        DateTime created,
        double latitude,
        double longitude,
        PollenCategory pollenCategory,
        PollenType pollenType,
        int pollenColorRed,
        int pollenColorGreen,
        int pollenColorBlue
    )
    {
        DateOnly date = DateOnly.FromDateTime(created);
        Assert.Equal(created, weatherForecast.Created);
        Assert.Equal(DateTimeKind.Utc, weatherForecast.Created.Kind);
        Assert.Equal(date, weatherForecast.Date);
        Assert.Equal(latitude, weatherForecast.Latitude);
        Assert.Equal(longitude, weatherForecast.Longitude);
        Assert.Equal(pollenCategory, weatherForecast.PollenCategory);
        Assert.Equal(Byte.MaxValue, weatherForecast.PollenColor.A);
        Assert.Equal(pollenColorBlue, weatherForecast.PollenColor.B);
        Assert.Equal(pollenColorGreen, weatherForecast.PollenColor.G);
        Assert.Equal(pollenColorRed, weatherForecast.PollenColor.R);
        Assert.Equal(pollenType, weatherForecast.PollenType);
        Assert.Equal(created, weatherForecast.Updated);
        Assert.Equal(DateTimeKind.Utc, weatherForecast.Updated.Kind);
    }
}
