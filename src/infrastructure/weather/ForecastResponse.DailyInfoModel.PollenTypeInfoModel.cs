using System;

namespace Shipstone.Pollen.Api.Infrastructure.Weather;

partial class ForecastResponse
{
    partial class DailyInfoModel
    {
        internal sealed partial class PollenTypeInfoModel
        {
            internal String _code;
            internal ForecastResponse.DailyInfoModel.PollenTypeInfoModel.IndexInfoModel? _indexInfo;

            public String Code
            {
                set => this._code = value;
            }

            public ForecastResponse.DailyInfoModel.PollenTypeInfoModel.IndexInfoModel? IndexInfo
            {
                set => this._indexInfo = value;
            }
        }
    }
}
