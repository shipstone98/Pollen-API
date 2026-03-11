using System;
using Microsoft.Extensions.DependencyInjection;

namespace Shipstone.Pollen.Api.Infrastructure.Weather;

/// <summary>
/// Provides a set of <c>static</c> (<c>Shared</c> in Visual Basic) methods for registering services with objects that implement <see cref="IServiceCollection" />.
/// </summary>
public static class WeatherInfrastructureServiceCollectionExtensions
{
    /// <summary>
    /// Registers Pollen weather infrastructure services with the specified <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to register services with.</param>
    /// <param name="configureWeather">A delegate to configure <see cref="WeatherOptions" />, or <c>null</c>.</param>
    /// <returns>A reference to <c><paramref name="services" /></c> that can be further used to register services.</returns>
    /// <exception cref="ArgumentNullException"><c><paramref name="services" /></c> is <c>null</c>.</exception>
    public static IServiceCollection AddPollenInfrastructureWeather(
        this IServiceCollection services,
        Action<WeatherOptions>? configureWeather = null
    )
    {
        ArgumentNullException.ThrowIfNull(services);

        return services
            .AddSingleton<IWeatherService, WeatherService>()
            .Configure<WeatherOptions>(options =>
            {
                if (configureWeather is not null)
                {
                    configureWeather(options);
                }
            });
    }
}
