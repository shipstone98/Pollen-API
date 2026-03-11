using System;
using Xunit;

using Shipstone.Pollen.Api.Infrastructure.Weather;

namespace Shipstone.Pollen.Api.Infrastructure.WeatherTest;

public sealed class WeatherOptionsTest
{
    [InlineData(null)]
    [InlineData("My API key")]
    [Theory]
    public void TestApiKey_Set(String? apiKey)
    {
        // Arrange
        WeatherOptions options = new();

        // Act
        options.ApiKey = apiKey;

        // Assert
        Assert.Equal(apiKey, options.ApiKey);
    }
}
