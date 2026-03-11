using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Shipstone.Pollen.Api.Infrastructure.WeatherTest.Mocks;

internal sealed class MockHttpMessageInvoker : HttpMessageInvoker
{
    internal Func<HttpRequestMessage, HttpResponseMessage> _sendFunc;

    internal MockHttpMessageInvoker(HttpMessageHandler handler) : base(handler) =>
        this._sendFunc = _ => throw new NotImplementedException();

    protected sealed override void Dispose(bool disposing) =>
        throw new NotImplementedException();

    public sealed override bool Equals(Object? obj) =>
        throw new NotImplementedException();

    public sealed override int GetHashCode() =>
        throw new NotImplementedException();

    public sealed override HttpResponseMessage Send(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    ) =>
        throw new NotImplementedException();

    public sealed override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken
    )
    {
        HttpResponseMessage result = this._sendFunc(request);
        return Task.FromResult(result);
    }

    public sealed override String? ToString() =>
        throw new NotImplementedException();
}
