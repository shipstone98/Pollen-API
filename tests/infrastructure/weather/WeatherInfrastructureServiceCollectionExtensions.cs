using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

using Shipstone.Pollen.Api.Infrastructure.Weather;

using Shipstone.Test.Mocks;

namespace Shipstone.Pollen.Api.Infrastructure.WeatherTest;

public sealed class WeatherInfrastructureServiceCollectionExtensionsTest
{
    [Fact]
    public void TestAddPollenInfrastructureWeather_Invalid()
    {
        // Act
        ArgumentException ex =
            Assert.Throws<ArgumentNullException>(() =>
                WeatherInfrastructureServiceCollectionExtensions.AddPollenInfrastructureWeather(null!));

        // Assert
        Assert.Equal("services", ex.ParamName);
    }

    [Fact]
    public void TestAddPollenInfrastructureWeather_Valid()
    {
        // Arrange
        IList<ServiceDescriptor> list = new List<ServiceDescriptor>();
        MockServiceCollection services = new();
        services._addAction = list.Add;
        services._countFunc = () => list.Count;
        services._itemFunc = i => list[i];

        // Act
        IServiceCollection result =
            WeatherInfrastructureServiceCollectionExtensions.AddPollenInfrastructureWeather(services);

        // Assert
        Assert.Same(services, result);

        IEnumerable<Type> types = new Type[]
        {
            typeof (IWeatherService),
            typeof (IConfigureOptions<WeatherOptions>)
        };

        foreach (Type type in types)
        {
            ServiceDescriptor descriptor =
                list.First(s => s.ServiceType.Equals(type));

            Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
        }
    }
}
