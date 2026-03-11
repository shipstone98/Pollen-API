using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using Shipstone.Pollen.Api.Infrastructure.Data;

using Shipstone.Test.Mocks;

namespace Shipstone.Pollen.Api.Infrastructure.DataTest;

public sealed class DataInfrastructureServiceCollectionExtensionsTest
{
    [Fact]
    public void TestAddPollenInfrastructureData_Invalid()
    {
        // Act
        ArgumentException ex =
            Assert.Throws<ArgumentNullException>(() =>
                DataInfrastructureServiceCollectionExtensions.AddPollenInfrastructureData(null!));

        // Assert
        Assert.Equal("services", ex.ParamName);
    }

    [Fact]
    public void TestAddPollenInfrastructureData_Valid()
    {
        // Arrange
        ICollection<ServiceDescriptor> collection =
            new List<ServiceDescriptor>();

        MockServiceCollection services = new();
        services._addAction = collection.Add;

        // Act
        IServiceCollection result =
            DataInfrastructureServiceCollectionExtensions.AddPollenInfrastructureData(services);

        // Assert
        Assert.Same(services, result);

        IEnumerable<Type> types = new Type[]
        {
            typeof (IRepository),
            typeof (IUserRepository),
            typeof (IWeatherForecastRepository)
        };

        foreach (Type type in types)
        {
            ServiceDescriptor descriptor =
                collection.First(s => s.ServiceType.Equals(type));

            Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
        }
    }
}
