using System;

namespace Shipstone.Pollen.Api.Infrastructure.Weather;

partial class ForecastResponse
{
    partial class DailyInfoModel
    {
        partial class PollenTypeInfoModel
        {
            internal sealed class IndexInfoModel
            {
                internal String _category;
                internal ColorResponse? _color;

                public String Category
                {
                    set => this._category = value;
                }

                public ColorResponse? Color
                {
                    set => this._color = value;
                }
            }
        }
    }
}
