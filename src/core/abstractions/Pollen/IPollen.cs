using System.Drawing;

namespace Shipstone.Pollen.Api.Core.Pollen;

/// <summary>
/// Represents a pollen forecast.
/// </summary>
public interface IPollen
{
    /// <summary>
    /// Gets the category of the <see cref="IPollen" />.
    /// </summary>
    /// <value>The <see cref="PollenCategory" /> of the <see cref="IPollen" />.</value>
    PollenCategory Category { get; }

    /// <summary>
    /// Gets the color of the <see cref="IPollen" />.
    /// </summary>
    /// <value>The <see cref="Color" /> of the <see cref="IPollen" />.</value>
    Color Color { get; }

    /// <summary>
    /// Gets the ID of the <see cref="IPollen" />.
    /// </summary>
    /// <value>The ID of the <see cref="IPollen" />.</value>
    long Id { get; }

    /// <summary>
    /// Gets the type of the <see cref="IPollen" />.
    /// </summary>
    /// <value>The <see cref="PollenType" /> of the <see cref="IPollen" />.</value>
    PollenType Type { get; }
}
