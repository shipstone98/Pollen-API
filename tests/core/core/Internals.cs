using System;
using Xunit;

using Shipstone.Pollen.Api.Core.Pollen;

namespace Shipstone.Pollen.Api.CoreTest;

internal static class Internals
{
    internal static void AssertEqual(
        this IPollen pollen,
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
        Assert.Equal(type, pollen.Type);
    }
}
