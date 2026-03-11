using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using Shipstone.Pollen.Api.Core;
using Shipstone.Pollen.Api.Core.Services;

using Shipstone.Test.Mocks;

namespace Shipstone.Pollen.Api.CoreTest.Services;

public sealed class CoordinateServiceTest
{
    private readonly ICoordinateService _coordinate;

    public CoordinateServiceTest()
    {
        ICollection<ServiceDescriptor> collection =
            new List<ServiceDescriptor>();

        MockServiceCollection services = new();
        services._addAction = collection.Add;
        services._getEnumeratorFunc = collection.GetEnumerator;
        services.AddPollenCore();
        IServiceProvider provider = new MockServiceProvider(services);
        this._coordinate = provider.GetRequiredService<ICoordinateService>();
    }

#region Normalize method
#region Invalid arguments
    [InlineData(Double.NaN)]
    [InlineData(Double.NegativeInfinity)]
    [InlineData(Double.PositiveInfinity)]
    [Theory]
    public void TestNormalize_Invalid_Latitude_NotOutOfRange(double latitude)
    {
        // Arrange
        double longitude = 0;

        // Act
        ArgumentException ex =
            Assert.Throws<ArgumentException>(() =>
                this._coordinate.Normalize(ref latitude, ref longitude));

        // Assert
        Assert.Equal("latitude", ex.ParamName);
    }

    [InlineData(Double.MinValue)]
    [InlineData(-90.00000000000001)]
    [InlineData(90.00000000000001)]
    [InlineData(Double.MaxValue)]
    [Theory]
    public void TestNormalize_Invalid_Latitude_OutOfRange(double latitude)
    {
        // Arrange
        double longitude = 0;

        // Act
        ArgumentOutOfRangeException ex =
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                this._coordinate.Normalize(ref latitude, ref longitude));

        // Assert
        Assert.Equal(latitude, ex.ActualValue);
        Assert.Equal("latitude", ex.ParamName);
    }

    [InlineData(Double.NaN)]
    [InlineData(Double.NegativeInfinity)]
    [InlineData(Double.PositiveInfinity)]
    [Theory]
    public void TestNormalize_Invalid_Longitude_NotOutOfRange(double longitude)
    {
        // Arrange
        double latitude = 0;

        // Act
        ArgumentException ex =
            Assert.Throws<ArgumentException>(() =>
                this._coordinate.Normalize(ref latitude, ref longitude));

        // Assert
        Assert.Equal("longitude", ex.ParamName);
    }

    [InlineData(Double.MinValue)]
    [InlineData(-180.00000000000003)]
    [InlineData(180)]
    [InlineData(180.00000000000003)]
    [InlineData(Double.MaxValue)]
    [Theory]
    public void TestNormalize_Invalid_Longitude_OutOfRange(double longitude)
    {
        // Arrange
        double latitude = 0;

        // Act
        ArgumentOutOfRangeException ex =
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                this._coordinate.Normalize(ref latitude, ref longitude));

        // Assert
        Assert.Equal(longitude, ex.ActualValue);
        Assert.Equal("longitude", ex.ParamName);
    }
#endregion

    [InlineData(-90, -180, -90, -180)]
    [InlineData(-90, -Double.Epsilon, -90, 0)]
    [InlineData(-90, 0, -90, 0)]
    [InlineData(-90, Double.Epsilon, -90, 0)]
    [InlineData(-90, 179.99999999999997, -90, 179.9999)]
    [InlineData(-Double.Epsilon, -180, 0, -180)]
    [InlineData(-Double.Epsilon, -Double.Epsilon, 0, 0)]
    [InlineData(-Double.Epsilon, 0, 0, 0)]
    [InlineData(-Double.Epsilon, Double.Epsilon, 0, 0)]
    [InlineData(-Double.Epsilon, 179.99999999999997, 0, 179.9999)]
    [InlineData(0, -180, 0, -180)]
    [InlineData(0, -Double.Epsilon, 0, 0)]
    [InlineData(0, 0, 0, 0)]
    [InlineData(0, Double.Epsilon, 0, 0)]
    [InlineData(0, 179.99999999999997, 0, 179.9999)]
    [InlineData(Double.Epsilon, -180, 0, -180)]
    [InlineData(Double.Epsilon, -Double.Epsilon, 0, 0)]
    [InlineData(Double.Epsilon, 0, 0, 0)]
    [InlineData(Double.Epsilon, Double.Epsilon, 0, 0)]
    [InlineData(Double.Epsilon, 179.99999999999997, 0, 179.9999)]
    [InlineData(90, -180, 90, -180)]
    [InlineData(90, -Double.Epsilon, 90, 0)]
    [InlineData(90, 0, 90, 0)]
    [InlineData(90, Double.Epsilon, 90, 0)]
    [InlineData(90, 179.99999999999997, 90, 179.9999)]
    [InlineData(-0.12345, -0.12345, -0.1235, -0.1235)]
    [InlineData(-0.12345, -0.12344, -0.1235, -0.1234)]
    [InlineData(-0.12345, 0.12344, -0.1235, 0.1234)]
    [InlineData(-0.12345, 0.12345, -0.1235, 0.1235)]
    [InlineData(-0.12344, -0.12345, -0.1234, -0.1235)]
    [InlineData(-0.12344, -0.12344, -0.1234, -0.1234)]
    [InlineData(-0.12344, 0.12344, -0.1234, 0.1234)]
    [InlineData(-0.12344, 0.12345, -0.1234, 0.1235)]
    [InlineData(0.12344, -0.12345, 0.1234, -0.1235)]
    [InlineData(0.12344, -0.12344, 0.1234, -0.1234)]
    [InlineData(0.12344, 0.12344, 0.1234, 0.1234)]
    [InlineData(0.12344, 0.12345, 0.1234, 0.1235)]
    [InlineData(0.12345, -0.12345, 0.1235, -0.1235)]
    [InlineData(0.12345, -0.12344, 0.1235, -0.1234)]
    [InlineData(0.12345, 0.12344, 0.1235, 0.1234)]
    [InlineData(0.12345, 0.12345, 0.1235, 0.1235)]
    [Theory]
    public void TestNormalize_Valid(
        double latitude,
        double longitude,
        double latitudeExpected,
        double longitudeExpected
    )
    {
        // Arrange
        double latitudeActual = latitude;
        double longitudeActual = longitude;

        // Act
        this._coordinate.Normalize(ref latitudeActual, ref longitudeActual);

        // Assert
        Assert.Equal(latitudeExpected, latitudeActual);
        Assert.Equal(longitudeExpected, longitudeActual);
    }
#endregion
}
