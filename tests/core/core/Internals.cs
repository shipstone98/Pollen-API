using System;
using Xunit;

using Shipstone.Pollen.Api.Core.Pollen;

namespace Shipstone.Pollen.Api.CoreTest;

internal static class Internals
{
    internal static void AssertEqual(
        this IPollen pollen,
        long id,
        PollenType type,
        PollenCategory category,
        byte colorRed,
        byte colorGreen,
        byte colorBlue
    )
    {
        Assert.Equal(category, pollen.Category);
        Assert.Equal(Byte.MaxValue, pollen.Color.A);
        Assert.Equal(colorBlue, pollen.Color.B);
        Assert.Equal(colorGreen, pollen.Color.G);
        Assert.Equal(colorRed, pollen.Color.R);
        Assert.Equal(id, pollen.Id);
        Assert.Equal(type, pollen.Type);
    }

    internal static void SetId(this Object entity, Object id)
    {
        Object?[]? arguments = new Object?[1] { id };

        entity
            .GetType()
            .GetProperty("Id")!
            .GetSetMethod()!
            .Invoke(entity, arguments);
    }
}
