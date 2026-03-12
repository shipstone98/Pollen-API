using System.Drawing;

using Shipstone.Pollen.Api.Infrastructure.Entities;

namespace Shipstone.Pollen.Api.Core.Pollen;

internal sealed class Pollen : IPollen
{
    private readonly WeatherForecastEntity _weatherForecast;

    PollenCategory IPollen.Category => this._weatherForecast.PollenCategory;
    Color IPollen.Color => this._weatherForecast.PollenColor;
    PollenType IPollen.Type => this._weatherForecast.PollenType;

    internal Pollen(WeatherForecastEntity weatherForecast) =>
        this._weatherForecast = weatherForecast;
}
