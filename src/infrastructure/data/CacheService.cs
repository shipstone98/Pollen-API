using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

using Shipstone.Utilities.Text.Json;

using Shipstone.Pollen.Api.Core.Services;
using Shipstone.Pollen.Api.Infrastructure.Entities;

namespace Shipstone.Pollen.Api.Infrastructure.Data;

internal sealed class CacheService : ICacheService
{
    private readonly IDistributedCache _cache;
    private readonly ICoordinateService _coordinate;
    private readonly JsonSerializerOptions _options;

    public CacheService(IDistributedCache cache, ICoordinateService coordinate)
    {
        ArgumentNullException.ThrowIfNull(cache);
        ArgumentNullException.ThrowIfNull(coordinate);

        JsonSerializerOptions options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        JsonConverter colorConverter = new ColorJsonConverter();
        JsonConverter enumConverter = new JsonStringEnumConverter();
        options.Converters.Add(colorConverter);
        options.Converters.Add(enumConverter);
        this._cache = cache;
        this._coordinate = coordinate;
        this._options = options;
    }

    private async Task AddAsync(
        WeatherForecastEntity weatherForecast,
        CancellationToken cancellationToken
    )
    {
        String key =
            CacheService.CreateKey(
                weatherForecast.Date,
                weatherForecast.Latitude,
                weatherForecast.Longitude
            );

        List<WeatherForecastEntity> weatherForecasts =
            await this.GetListAsync(key, cancellationToken) ?? new();

        weatherForecasts.Add(weatherForecast);
        String s = JsonSerializer.Serialize(weatherForecasts, this._options);

        await this._cache.SetStringAsync(
            key,
            s,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpiration =
                    weatherForecast.Date
                        .AddDays(1)
                        .ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc)
            },
            cancellationToken
        );
    }

    private async Task<List<WeatherForecastEntity>?> GetListAsync(
        String key,
        CancellationToken cancellationToken
    )
    {
        String? s = await this._cache.GetStringAsync(key, cancellationToken);

        try
        {
            return JsonSerializer.Deserialize<List<WeatherForecastEntity>>(
                s!,
                this._options
            );
        }

        catch
        {
            return null;
        }
    }

    private async IAsyncEnumerable<WeatherForecastEntity> ListAsync(
        DateOnly date,
        double latitude,
        double longitude,
        [EnumeratorCancellation] CancellationToken cancellationToken
    )
    {
        String key = CacheService.CreateKey(date, latitude, longitude);

        IEnumerable<WeatherForecastEntity>? weatherForecasts =
            await this.GetListAsync(key, cancellationToken);

        if (weatherForecasts is not null)
        {
            foreach (WeatherForecastEntity weatherForecast in weatherForecasts)
            {
                yield return weatherForecast;
            }
        }
    }

    Task ICacheService.AddAsync(
        WeatherForecastEntity weatherForecast,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(weatherForecast);
        return this.AddAsync(weatherForecast, cancellationToken);
    }

    IAsyncEnumerable<WeatherForecastEntity> ICacheService.ListAsync(
        DateOnly date,
        double latitude,
        double longitude,
        CancellationToken cancellationToken
    )
    {
        this._coordinate.Normalize(ref latitude, ref longitude);
        return this.ListAsync(date, latitude, longitude, cancellationToken);
    }

    private static String CreateKey(
        DateOnly date,
        double latitude,
        double longitude
    ) =>
        $"{date}:({latitude},{longitude})";
}
