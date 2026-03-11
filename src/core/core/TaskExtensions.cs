using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shipstone.Pollen.Api.Core;

internal static class TaskExtensions
{
    internal static async IAsyncEnumerable<TSource> ToAsyncEnumerable<TSource>(this Task<TSource[]> task)
    {
        foreach (TSource item in await task)
        {
            yield return item;
        }
    }
}
