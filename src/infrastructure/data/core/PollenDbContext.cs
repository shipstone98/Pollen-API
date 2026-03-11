using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Shipstone.EntityFrameworkCore;
using Shipstone.Extensions.Security;

using Shipstone.Pollen.Api.Infrastructure.Data.Configuration;
using Shipstone.Pollen.Api.Infrastructure.Entities;

namespace Shipstone.Pollen.Api.Infrastructure.Data;

/// <summary>
/// Represents a database. This class cannot be instantiated.
/// </summary>
/// <typeparam name="TContext">The type of the database context.</typeparam>
public abstract class PollenDbContext<TContext> : DbContext, IDataSource
    where TContext : notnull, PollenDbContext<TContext>
{
    private readonly IEncryptionService _encryption;
    private readonly DbSet<UserEntity> _users;
    private readonly DbSet<WeatherForecastEntity> _weatherForecasts;

    /// <summary>
    /// Gets the user database set.
    /// </summary>
    /// <value>The user database set.</value>
    public DbSet<UserEntity> Users => this._users;

    /// <summary>
    /// Gets the weather forecast database set.
    /// </summary>
    /// <value>The weather forecast database set.</value>
    public DbSet<WeatherForecastEntity> WeatherForecasts =>
        this._weatherForecasts;

    IDataSet<UserEntity> IDataSource.Users =>
        new DataSet<UserEntity>(this._users);

    IDataSet<WeatherForecastEntity> IDataSource.WeatherForecasts =>
        new DataSet<WeatherForecastEntity>(this._weatherForecasts);

    protected PollenDbContext(
        DbContextOptions<TContext> options,
        IEncryptionService encryption
    )
        : base(options)
    {
        ArgumentNullException.ThrowIfNull(encryption);
        this._encryption = encryption;
        this._users = this.Set<UserEntity>();
        this._weatherForecasts = this.Set<WeatherForecastEntity>();
    }

#warning Not tested
    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        IEntityTypeConfiguration<UserEntity> userConfiguration =
            new UserConfiguration(this._encryption);

        IEntityTypeConfiguration<WeatherForecastEntity> weatherForecastConfiguration =
            new WeatherForecastConfiguration();

        modelBuilder
            .ApplyConfiguration(userConfiguration)
            .ApplyConfiguration(weatherForecastConfiguration);
    }

#warning Not tested
    Task IDataSource.SaveAsync(CancellationToken cancellationToken) =>
        this.SaveChangesAsync(cancellationToken);
}
