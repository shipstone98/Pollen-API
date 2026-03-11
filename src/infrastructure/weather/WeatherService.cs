using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

using Shipstone.Pollen.Api.Core.Pollen;
using Shipstone.Pollen.Api.Core.Services;
using Shipstone.Pollen.Api.Infrastructure.Entities;

namespace Shipstone.Pollen.Api.Infrastructure.Weather;

internal sealed class WeatherService : IWeatherService
{
    private readonly ICoordinateService _coordinate;
    private readonly HttpMessageInvoker _messageInvoker;
    private readonly WeatherOptions _options;

    public WeatherService(
        HttpMessageInvoker messageInvoker,
        ICoordinateService coordinate,
        IOptions<WeatherOptions>? options
    )
    {
        ArgumentNullException.ThrowIfNull(messageInvoker);
        ArgumentNullException.ThrowIfNull(coordinate);
        this._coordinate = coordinate;
        this._messageInvoker = messageInvoker;
        this._options = options?.Value ?? new();
    }

    private async Task<IEnumerable<WeatherForecastEntity>> ListForecastsAsync(
        double latitude,
        double longitude,
        CancellationToken cancellationToken
    )
    {
        HttpRequestMessage requestMessage =
            new(
                HttpMethod.Get,
                $"https://pollen.googleapis.com/v1/forecast:lookup?key={this._options._apiKey}&location.latitude={latitude}&location.longitude={longitude}&days=1&plantsDescription=false"
            );

        HttpResponseMessage responseMessage;
        const String MESSAGE = "The weather forecast could not be listed.";

        try
        {
            responseMessage =
                await this._messageInvoker.SendAsync(
                    requestMessage,
                    cancellationToken
                );
        }

        catch (Exception ex)
        {
            throw new WeatherException(MESSAGE, ex);
        }

        if (responseMessage.StatusCode != HttpStatusCode.OK)
        {
            throw new WeatherException(MESSAGE);
        }

        ForecastResponse? response;

        try
        {
            response =
                await responseMessage.Content.ReadFromJsonAsync<ForecastResponse>(cancellationToken);
        }

        catch (Exception ex)
        {
            throw new WeatherException(MESSAGE, ex);
        }

        if (response is null || response._dailyInfo is null)
        {
            throw new WeatherException(MESSAGE);
        }

        DateTime now = DateTime.UtcNow;
        DateOnly date = DateOnly.FromDateTime(now);

        return response._dailyInfo.SelectMany(di =>
        {
            if (di._pollenTypeInfo is null)
            {
                throw new WeatherException(MESSAGE);
            }

            return di._pollenTypeInfo.Select(pti =>
                WeatherService.Create(pti, now, date, latitude, longitude));
        });
    }

    Task<IEnumerable<WeatherForecastEntity>> IWeatherService.ListForecastsAsync(
        double latitude,
        double longitude,
        CancellationToken cancellationToken
    )
    {
        this._coordinate.Normalize(ref latitude, ref longitude);
        return this.ListForecastsAsync(latitude, longitude, cancellationToken);
    }

    private static WeatherForecastEntity Create(
        ForecastResponse.DailyInfoModel.PollenTypeInfoModel pollenTypeInfo,
        DateTime now,
        DateOnly date,
        double latitude,
        double longitude
    )
    {
        if (!Enum.TryParse(pollenTypeInfo._code, true, out PollenType type))
        {
            type = PollenType.None;
        }

        PollenCategory category;
        Color color;

        if (pollenTypeInfo._indexInfo is not null)
        {
            category =
                WeatherService.GetCategory(pollenTypeInfo._indexInfo._category);

            color =
                pollenTypeInfo._indexInfo._color is null
                    ? new()
                    : Color.FromArgb(
                        (int) (pollenTypeInfo._indexInfo._color._red * 256),
                        (int) (pollenTypeInfo._indexInfo._color._green * 256),
                        (int) (pollenTypeInfo._indexInfo._color._blue * 256)
                    );
        }

        else
        {
            category = PollenCategory.None;
            color = new();
        }

        return new WeatherForecastEntity
        {
            Created = now,
            Date = date,
            Latitude = latitude,
            Longitude = longitude,
            PollenCategory = category,
            PollenColor = color,
            PollenType = type,
            Updated = now
        };
    }

    private static PollenCategory GetCategory(String category) =>
        category.ToLower() switch
        {
            "very low" => PollenCategory.VeryLow,
            "low" => PollenCategory.Low,
            "moderate" => PollenCategory.Moderate,
            "high" => PollenCategory.High,
            "very high" => PollenCategory.VeryHigh,
            _ => PollenCategory.None,
        };
}
