using System.Collections.Generic;

namespace Shipstone.Pollen.Api.Infrastructure.Weather;

internal sealed partial class ForecastResponse
{
    internal IEnumerable<ForecastResponse.DailyInfoModel>? _dailyInfo;

    public IEnumerable<ForecastResponse.DailyInfoModel>? DailyInfo
    {
        get => this._dailyInfo;
        set => this._dailyInfo = value;
    }
}
