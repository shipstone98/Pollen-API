namespace Shipstone.Pollen.Api.Infrastructure.Weather;

internal sealed class ColorResponse
{
    internal double _blue;
    internal double _green;
    internal double _red;

    public double Blue
    {
        set => this._blue = value;
    }

    public double Green
    {
        set => this._green = value;
    }

    public double Red
    {
        set => this._red = value;
    }
}
