using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

using Shipstone.Pollen.Api.Core.Pollen;
using Shipstone.Pollen.Api.Core.Services;
using Shipstone.Pollen.Api.Infrastructure.Entities;
using Shipstone.Pollen.Api.Infrastructure.Weather;

using Shipstone.Pollen.Api.Infrastructure.WeatherTest.Mocks;
using Shipstone.Test.Mocks;

namespace Shipstone.Pollen.Api.Infrastructure.WeatherTest;

public sealed class WeatherServiceTest
{
    private readonly MockCoordinateService _coordinate;
    private readonly MockHttpMessageInvoker _messageInvoker;
    private readonly IWeatherService _weather;

    public WeatherServiceTest()
    {
        IList<ServiceDescriptor> list = new List<ServiceDescriptor>();
        MockServiceCollection services = new();
        services._addAction = list.Add;
        services._countFunc = () => list.Count;
        services._itemFunc = i => list[i];
        services._getEnumeratorFunc = list.GetEnumerator;
        services.AddPollenInfrastructureWeather();
        IServiceProvider provider = new MockServiceProvider(services);
        MockCoordinateService coordinate = new();
        services.AddSingleton<ICoordinateService>(coordinate);
        MockHttpMessageHandler messageHandler = new();
        MockHttpMessageInvoker messageInvoker = new(messageHandler);
        services.AddSingleton<HttpMessageInvoker>(messageInvoker);
        MockOptions<WeatherOptions> options = new();
        options._valueFunc = () => new();
        services.AddSingleton<IOptions<WeatherOptions>>(options);
        this._coordinate = coordinate;
        this._messageInvoker = messageInvoker;
        this._weather = provider.GetRequiredService<IWeatherService>();
    }

#region ListForecastsAsync method
#region Invalid arguments
    [Fact]
    public async Task TestListForecastsAsync_Failure_ExceptionThrown()
    {
        // Arrange
        Exception innerException = new();
        this._coordinate._normalizeFunc = (_, _) => (0, 0);
        this._messageInvoker._sendFunc = _ => throw innerException;

        // Act
        Exception ex =
            await Assert.ThrowsAsync<WeatherException>(() =>
                this._weather.ListForecastsAsync(
                    0,
                    0,
                    CancellationToken.None
                ));

        // Assert
        Assert.Same(innerException, ex.InnerException);
    }

#region Exception not thrown
    [Fact]
    public async Task TestListForecastsAsync_Failure_ExceptionNotThrown_StatusCode200_ContentReadable()
    {
#region Arrange
        // Arrange
        this._coordinate._normalizeFunc = (_, _) => (0, 0);

        this._messageInvoker._sendFunc = _ =>
        {
            HttpResponseMessage responseMessage = new(HttpStatusCode.OK);
            MockHttpContent content = new();
            responseMessage.Content = content;

            content._createContentReadStreamFunc = () =>
            {
                MockStream stream = new();
                stream._closeAction = () => { };
                IEnumerable<byte> dataBytes = Encoding.UTF8.GetBytes("{}");
                Queue<byte> queue = new(dataBytes);

                stream._readFunc = m =>
                {
                    int n = 0;

                    for (int i = 0; i < m.Length && i < queue.Count; i ++)
                    {
                        m.Span[i] = queue.Dequeue();
                        ++ n;
                    }

                    return n;
                };

                return stream;
            };

            return responseMessage;
        };
#endregion

        // Act and assert
        await Assert.ThrowsAsync<WeatherException>(() =>
            this._weather.ListForecastsAsync(
                0,
                0,
                CancellationToken.None
            ));
    }

    [Fact]
    public async Task TestListForecastsAsync_Failure_ExceptionNotThrown_StatusCode200_ContentNotReadable()
    {
        // Arrange
        this._coordinate._normalizeFunc = (_, _) => (0, 0);

        this._messageInvoker._sendFunc = _ =>
        {
            HttpResponseMessage responseMessage = new(HttpStatusCode.OK);
            MockHttpContent content = new();
            responseMessage.Content = content;

            content._createContentReadStreamFunc = () =>
            {
                MockStream stream = new();
                stream._closeAction = () => { };
                stream._readFunc = m => 0;
                return stream;
            };

            return responseMessage;
        };

        // Act and assert
        await Assert.ThrowsAsync<WeatherException>(() =>
            this._weather.ListForecastsAsync(
                0,
                0,
                CancellationToken.None
            ));
    }

    [Fact]
    public async Task TestListForecastsAsync_Failure_ExceptionNotThrown_StatusCodeNot200()
    {
        // Arrange
        this._coordinate._normalizeFunc = (_, _) => (0, 0);

        this._messageInvoker._sendFunc = _ =>
            new(HttpStatusCode.InternalServerError);

        // Act and assert
        await Assert.ThrowsAsync<WeatherException>(() =>
            this._weather.ListForecastsAsync(
                0,
                0,
                CancellationToken.None
            ));
    }
#endregion
#endregion

    [Fact]
    public async Task TestListForecastsAsync_Success()
    {
#region Arrange
        // Arrange
        const double LATITUDE = 0.12345;
        const double LONGITUDE = 0.6789;
        const byte GRASS_POLLEN_COLOR_RED = 100;
        const byte GRASS_POLLEN_COLOR_BLUE = 150;
        const byte GRASS_POLLEN_COLOR_GREEN = 200;
        const byte TREE_POLLEN_COLOR_RED = 110;
        const byte TREE_POLLEN_COLOR_BLUE = 160;
        const byte TREE_POLLEN_COLOR_GREEN = 210;
        const byte WEED_POLLEN_COLOR_RED = 120;
        const byte WEED_POLLEN_COLOR_BLUE = 170;
        const byte WEED_POLLEN_COLOR_GREEN = 220;
        this._coordinate._normalizeFunc = (_, _) => (LATITUDE, LONGITUDE);

        this._messageInvoker._sendFunc = _ =>
        {
            HttpResponseMessage responseMessage = new(HttpStatusCode.OK);
            MockHttpContent content = new();
            responseMessage.Content = content;

            content._createContentReadStreamFunc = () =>
            {
                MockStream stream = new();
                stream._closeAction = () => { };

                IEnumerable<byte> dataBytes =
                    Encoding.UTF8.GetBytes($"{{\"dailyInfo\":[{{\"pollenTypeInfo\":[{{\"code\":\"GRASS\",\"indexInfo\":{{\"category\":\"Moderate\",\"color\":{{\"red\":{GRASS_POLLEN_COLOR_RED / 256.0},\"green\":{GRASS_POLLEN_COLOR_GREEN / 256.0},\"blue\":{GRASS_POLLEN_COLOR_BLUE / 256.0}}}}}}},{{\"code\":\"TREE\",\"indexInfo\":{{\"category\":\"Very Low\",\"color\":{{\"red\":{TREE_POLLEN_COLOR_RED / 256.0},\"green\":{TREE_POLLEN_COLOR_GREEN / 256.0},\"blue\":{TREE_POLLEN_COLOR_BLUE / 256.0}}}}}}},{{\"code\":\"WEED\",\"indexInfo\":{{\"category\":\"Very High\",\"color\":{{\"red\":{WEED_POLLEN_COLOR_RED / 256.0},\"green\":{WEED_POLLEN_COLOR_GREEN / 256.0},\"blue\":{WEED_POLLEN_COLOR_BLUE / 256.0}}}}}}}]}}]}}");

                Queue<byte> queue = new(dataBytes);

                stream._readFunc = m =>
                {
                    int n = 0;

                    for (int i = 0; i < m.Length && i < queue.Count; i ++)
                    {
                        m.Span[i] = queue.Dequeue();
                        ++ n;
                    }

                    return n;
                };

                return stream;
            };

            return responseMessage;
        };
#endregion

        // Act
        IEnumerable<WeatherForecastEntity> result =
            await this._weather.ListForecastsAsync(
                0,
                0,
                CancellationToken.None
            );

#region Assert
        // Assert
        WeatherForecastEntity grass =
            result.First(wf => wf.PollenType == PollenType.Grass);

        WeatherForecastEntity tree =
            result.First(wf => wf.PollenType == PollenType.Tree);

        WeatherForecastEntity weed =
            result.First(wf => wf.PollenType == PollenType.Weed);

        DateTime created = grass.Created;

        grass.AssertEqual(
            created,
            LATITUDE,
            LONGITUDE,
            PollenCategory.Moderate,
            PollenType.Grass,
            GRASS_POLLEN_COLOR_RED,
            GRASS_POLLEN_COLOR_GREEN,
            GRASS_POLLEN_COLOR_BLUE
        );

        tree.AssertEqual(
            created,
            LATITUDE,
            LONGITUDE,
            PollenCategory.VeryLow,
            PollenType.Tree,
            TREE_POLLEN_COLOR_RED,
            TREE_POLLEN_COLOR_GREEN,
            TREE_POLLEN_COLOR_BLUE
        );

        weed.AssertEqual(
            created,
            LATITUDE,
            LONGITUDE,
            PollenCategory.VeryHigh,
            PollenType.Weed,
            WEED_POLLEN_COLOR_RED,
            WEED_POLLEN_COLOR_GREEN,
            WEED_POLLEN_COLOR_BLUE
        );
#endregion
    }
#endregion
}
