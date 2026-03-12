using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace Shipstone.Pollen.Api.Infrastructure.DataTest.Mocks;

internal sealed class MockDistributedCache : IDistributedCache
{
    internal Func<String, byte[]?> _getFunc;
    internal Action<String, byte[], DistributedCacheEntryOptions> _setAction;

    public MockDistributedCache()
    {
        this._getFunc = _ => throw new NotImplementedException();
        this._setAction = (_, _, _) => throw new NotImplementedException();
    }

    byte[]? IDistributedCache.Get(String key) =>
        throw new NotImplementedException();

    Task<byte[]?> IDistributedCache.GetAsync(
        String key,
        CancellationToken token
    )
    {
        byte[]? result = this._getFunc(key);
        return Task.FromResult(result);
    }

    void IDistributedCache.Refresh(String key) =>
        throw new NotImplementedException();

    Task IDistributedCache.RefreshAsync(String key, CancellationToken token) =>
        throw new NotImplementedException();

    void IDistributedCache.Remove(String key) =>
        throw new NotImplementedException();

    Task IDistributedCache.RemoveAsync(String key, CancellationToken token) =>
        throw new NotImplementedException();

    void IDistributedCache.Set(
        String key,
        byte[] value,
        DistributedCacheEntryOptions options
    ) =>
        throw new NotImplementedException();

    Task IDistributedCache.SetAsync(
        String key,
        byte[] value,
        DistributedCacheEntryOptions options,
        CancellationToken token
    )
    {
        this._setAction(key, value, options);
        return Task.CompletedTask;
    }
}
