using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Shipstone.Pollen.Api.Infrastructure.WeatherTest.Mocks;

internal sealed class MockHttpContent : HttpContent
{
    internal Func<Stream> _createContentReadStreamFunc;

    internal MockHttpContent() =>
        this._createContentReadStreamFunc = () =>
            throw new NotImplementedException();

    protected sealed override Stream CreateContentReadStream(CancellationToken cancellationToken) =>
        throw new NotImplementedException();

    protected sealed override Task<Stream> CreateContentReadStreamAsync() =>
        throw new NotImplementedException();

    protected sealed override Task<Stream> CreateContentReadStreamAsync(CancellationToken cancellationToken)
    {
        Stream result = this._createContentReadStreamFunc();
        return Task.FromResult(result);
    }

    protected sealed override void Dispose(bool disposing) =>
        throw new NotImplementedException();

    public sealed override bool Equals(Object? obj) =>
        throw new NotImplementedException();

    public sealed override int GetHashCode() =>
        throw new NotImplementedException();

    protected sealed override void SerializeToStream(
        Stream stream,
        TransportContext? context,
        CancellationToken cancellationToken
    ) =>
        throw new NotImplementedException();

    protected sealed override Task SerializeToStreamAsync(
        Stream stream,
        TransportContext? context
    ) =>
        throw new NotImplementedException();

    protected sealed override Task SerializeToStreamAsync(
        Stream stream,
        TransportContext? context,
        CancellationToken cancellationToken
    ) =>
        throw new NotImplementedException();

    public sealed override String? ToString() =>
        throw new NotImplementedException();

    protected sealed override bool TryComputeLength(out long length) =>
        throw new NotImplementedException();
}
