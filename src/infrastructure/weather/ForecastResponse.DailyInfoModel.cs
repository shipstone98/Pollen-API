using System.Collections.Generic;

namespace Shipstone.Pollen.Api.Infrastructure.Weather;

partial class ForecastResponse
{
    internal sealed partial class DailyInfoModel
    {
        internal IEnumerable<ForecastResponse.DailyInfoModel.PollenTypeInfoModel>? _pollenTypeInfo;

        public IEnumerable<ForecastResponse.DailyInfoModel.PollenTypeInfoModel>? PollenTypeInfo
        {
            get => this._pollenTypeInfo;
            set => this._pollenTypeInfo = value;
        }
    }
}
