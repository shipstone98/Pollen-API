using System;
using Microsoft.Extensions.Options;

namespace Shipstone.Pollen.Api.Infrastructure.Weather;

/// <summary>
/// Specifies options for weather requirements.
/// </summary>
public class WeatherOptions : IOptions<WeatherOptions>
{
    internal String? _apiKey;

    /// <summary>
    /// Gets or sets the API key for Google Cloud Pollen API.
    /// </summary>
    /// <value>The API key for Google Cloud Pollen API, or <c>null</c>.</value>
    public String? ApiKey
    {
        get => this._apiKey;
        set => this._apiKey = value;
    }

    WeatherOptions IOptions<WeatherOptions>.Value => this;

    /// <summary>
    /// Initializes a new instance of the <see cref="WeatherOptions" /> class.
    /// </summary>
    public WeatherOptions() { }
}
