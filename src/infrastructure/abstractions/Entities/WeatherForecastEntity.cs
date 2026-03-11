using System;
using System.Drawing;

using Shipstone.Pollen.Api.Core.Pollen;

namespace Shipstone.Pollen.Api.Infrastructure.Entities;

/// <summary>
/// Represents a weather forecast.
/// </summary>
public class WeatherForecastEntity : Entity<long>
{
    private PollenCategory _pollenCategory;
    private PollenType _pollenType;

    /// <summary>
    /// Gets or initializes the date of the weather forecast.
    /// </summary>
    /// <value>The date of the weather forecast.</value>
    public DateOnly Date { get; init; }

    /// <summary>
    /// Gets or initializes the latitude coordinate of the weather forecast.
    /// </summary>
    /// <value>The latitude coordinate of the weather forecast.</value>
    public double Latitude { get; init; }

    /// <summary>
    /// Gets or initializes the longitude coordinate of the weather forecast.
    /// </summary>
    /// <value>The longitude coordinate of the weather forecast.</value>
    public double Longitude { get; init; }

    /// <summary>
    /// Gets or initializes the category of the pollen of the weather forecast.
    /// </summary>
    /// <value>The <see cref="Core.Pollen.PollenCategory" /> of the weather forecast.</value>
    /// <exception cref="ArgumentException">The property is initialized and the value is not one of the <see cref="Core.Pollen.PollenCategory" /> values.</exception>
    public PollenCategory PollenCategory
    {
        get => this._pollenCategory;

        init
        {
            if (!Enum.IsDefined(value))
            {
                throw new ArgumentException(
                    $"{nameof (value)} is not one of the PollenCategory values.",
                    nameof (value)
                );
            }

            this._pollenCategory = value;
        }
    }

    /// <summary>
    /// Gets or initializes the color of the weather forecast.
    /// </summary>
    /// <value>The color of the weather forecast.</value>
    public Color PollenColor { get; init; }

    /// <summary>
    /// Gets or initializes the type of the pollen of the weather forecast.
    /// </summary>
    /// <value>The <see cref="Core.Pollen.PollenType" /> of the weather forecast.</value>
    /// <exception cref="ArgumentException">The property is initialized and the value is not one of the <see cref="Core.Pollen.PollenType" /> values.</exception>
    public PollenType PollenType
    {
        get => this._pollenType;

        init
        {
            if (!Enum.IsDefined(value))
            {
                throw new ArgumentException(
                    $"{nameof (value)} is not one of the PollenType values.",
                    nameof (value)
                );
            }

            this._pollenType = value;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="WeatherForecastEntity" /> class.
    /// </summary>
    public WeatherForecastEntity() { }
}
