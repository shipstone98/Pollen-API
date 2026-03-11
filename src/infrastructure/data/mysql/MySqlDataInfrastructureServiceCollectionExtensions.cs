using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Shipstone.Extensions.Security;

namespace Shipstone.Pollen.Api.Infrastructure.Data.MySql;

/// <summary>
/// Provides a set of <c>static</c> (<c>Shared</c> in Visual Basic) methods for registering services with objects that implement <see cref="IServiceCollection" />.
/// </summary>
public static class MySqlDataInfrastructureServiceCollectionExtensions
{
#warning Not tested
    /// <summary>
    /// Registers Pollen MySQL data infrastructure services with the specified <see cref="IServiceCollection" />.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection" /> to register services with.</param>
    /// <param name="connectionString">The connection string for the MySQL instance, or <c>null</c>.</param>
    /// <returns>A reference to <c><paramref name="services" /></c> that can be further used to register services.</returns>
    /// <exception cref="ArgumentNullException"><c><paramref name="services" /></c> is <c>null</c>.</exception>
    public static IServiceCollection AddPollenInfrastructureDataMySql(
        this IServiceCollection services,
        String? connectionString = null
    )
    {
        ArgumentNullException.ThrowIfNull(services);

        ServerVersion serverVersion =
            ServerVersion.AutoDetect(connectionString);

        return services
            .AddScoped<IDataSource>(provider =>
                provider.GetRequiredService<MySqlDbContext>())
            .AddScoped(provider =>
            {
                DbContextOptionsBuilder<MySqlDbContext> optionsBuilder = new();
                optionsBuilder.UseMySql(connectionString, serverVersion);

                IEncryptionService encryption =
                    provider.GetRequiredService<IEncryptionService>();

                return new MySqlDbContext(optionsBuilder.Options, encryption);
            });
    }
}
