using System;
using System.Drawing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Shipstone.Pollen.Api.Infrastructure.Entities;

namespace Shipstone.Pollen.Api.Infrastructure.Data.Configuration;

internal readonly struct WeatherForecastConfiguration
    : IEntityTypeConfiguration<WeatherForecastEntity>
{
    void IEntityTypeConfiguration<WeatherForecastEntity>.Configure(EntityTypeBuilder<WeatherForecastEntity> builder)
    {
        builder
            .Property(wf => wf.PollenCategory)
            .HasConversion<String>();

        builder
            .Property(wf => wf.PollenColor)
            .HasColumnName("PollenColorArgb")
            .HasConversion(i => i.ToArgb(), o => Color.FromArgb(o));

        builder
            .Property(wf => wf.PollenType)
            .HasConversion<String>();

        builder.HasIndex(wf => new { wf.Date, wf.Latitude, wf.Longitude });

        builder
            .HasIndex(wf =>
                new { wf.Date, wf.Latitude, wf.Longitude, wf.PollenType })
            .IsUnique();
    }
}
