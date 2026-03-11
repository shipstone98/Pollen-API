using Microsoft.EntityFrameworkCore;

using Shipstone.Extensions.Security;

namespace Shipstone.Pollen.Api.Infrastructure.Data.MySql;

internal sealed class MySqlDbContext : PollenDbContext<MySqlDbContext>
{
    internal MySqlDbContext(
        DbContextOptions<MySqlDbContext> options,
        IEncryptionService encryption
    )
        : base(options, encryption)
    { }
}
