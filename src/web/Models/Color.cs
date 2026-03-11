namespace Shipstone.Pollen.Api.Web.Models;

internal sealed class Color
{
    private readonly System.Drawing.Color _color;

    public byte Alpha => this._color.A;
    public byte Blue => this._color.B;
    public byte Green => this._color.G;
    public byte Red => this._color.R;

    internal Color(System.Drawing.Color color) => this._color = color;
}
