using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

using Shipstone.Pollen.Api.Core;
using Shipstone.Pollen.Api.Core.Accounts;
using Shipstone.Pollen.Api.Core.Pollen;

using Shipstone.Test.Mocks;

namespace Shipstone.Pollen.Api.CoreTest;

public sealed class CoreServiceCollectionExtensionsTest
{
    [Fact]
    public void TestAddPollenCore_Invalid()
    {
        // Act
        ArgumentException ex =
            Assert.Throws<ArgumentNullException>(() =>
                CoreServiceCollectionExtensions.AddPollenCore(null!));

        // Assert
        Assert.Equal("services", ex.ParamName);
    }

    [Fact]
    public void TestAddPollenCore_Valid()
    {
        // Arrange
        ICollection<ServiceDescriptor> collection =
            new List<ServiceDescriptor>();

        MockServiceCollection services = new();
        services._addAction = collection.Add;

        // Act
        IServiceCollection result =
            CoreServiceCollectionExtensions.AddPollenCore(services);

        // Assert
        Assert.Same(services, result);

        IEnumerable<Type> types = new Type[]
        {
            typeof (IAccountAuthenticateHandler),
            typeof (IPollenListHandler)
        };

        foreach (Type type in types)
        {
            ServiceDescriptor descriptor =
                collection.First(s => s.ServiceType.Equals(type));

            Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
        }
    }
}
