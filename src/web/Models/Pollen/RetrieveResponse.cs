using Shipstone.Pollen.Api.Core.Pollen;

namespace Shipstone.Pollen.Api.Web.Models.Pollen;

internal sealed class RetrieveResponse
{
    private readonly Color _color;
    private readonly IPollen _pollen;

    public PollenCategory Category => this._pollen.Category;
    public Color Color => this._color;
    public long Id => this._pollen.Id;
    public PollenType Type => this._pollen.Type;

    internal RetrieveResponse(IPollen pollen)
    {
        this._color = new(pollen.Color);
        this._pollen = pollen;
    }
}
