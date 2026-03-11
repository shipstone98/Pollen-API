using System;
using Xunit;

using Shipstone.Pollen.Api.Core.Pollen;
using Shipstone.Pollen.Api.Infrastructure.Entities;

namespace Shipstone.Pollen.Api.Infrastructure.AbstractionsTest.Entities;

public sealed class WeatherForecastEntityTest
{
    [InlineData(PollenCategory.None - 1)]
    [InlineData(PollenCategory.VeryHigh + 1)]
    [Theory]
    public void TestPollenCategory_Init_Invalid(PollenCategory pollenCategory)
    {
        // Act
        ArgumentException ex =
            Assert.Throws<ArgumentException>(() =>
                new WeatherForecastEntity
                {
                    PollenCategory = pollenCategory
                });

        // Assert
        Assert.Equal("value", ex.ParamName);
    }

    [Fact]
    public void TestPollenCategory_Init_Valid()
    {
        // Arrange
        foreach (PollenCategory pollenCategory in Enum.GetValues<PollenCategory>())
        {
            // Act
            WeatherForecastEntity weatherForecast = new WeatherForecastEntity
            {
                PollenCategory = pollenCategory
            };

            // Assert
            Assert.Equal(pollenCategory, weatherForecast.PollenCategory);
        }
    }

    [InlineData(PollenType.None - 1)]
    [InlineData(PollenType.Weed + 1)]
    [Theory]
    public void TestPollenType_Init_Invalid(PollenType pollenType)
    {
        // Act
        ArgumentException ex =
            Assert.Throws<ArgumentException>(() =>
                new WeatherForecastEntity
                {
                    PollenType = pollenType
                });

        // Assert
        Assert.Equal("value", ex.ParamName);
    }

    [Fact]
    public void TestPollenType_Init_Valid()
    {
        // Arrange
        foreach (PollenType pollenType in Enum.GetValues<PollenType>())
        {
            // Act
            WeatherForecastEntity weatherForecast = new WeatherForecastEntity
            {
                PollenType = pollenType
            };

            // Assert
            Assert.Equal(pollenType, weatherForecast.PollenType);
        }
    }
}
