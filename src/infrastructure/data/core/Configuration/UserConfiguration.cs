using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Shipstone.Extensions.Security;

using Shipstone.Pollen.Api.Infrastructure.Entities;

namespace Shipstone.Pollen.Api.Infrastructure.Data.Configuration;

internal sealed class UserConfiguration : IEntityTypeConfiguration<UserEntity>
{
    private readonly IEncryptionService _encryption;

    internal UserConfiguration(IEncryptionService encryption) =>
        this._encryption = encryption;

    void IEntityTypeConfiguration<UserEntity>.Configure(EntityTypeBuilder<UserEntity> builder)
    {
        IEnumerable<Expression<Func<UserEntity, String>>> protectedProperties =
            new Expression<Func<UserEntity, String>>[]
            {
                u => u.EmailAddress,
                u => u.EmailAddressNormalized
            };

        foreach (Expression<Func<UserEntity, String>> property in protectedProperties)
        {
            builder
                .Property(property)
                .HasConversion(
                    i => this._encryption.Encrypt(i),
                    o => this._encryption.Decrypt(o)
                );
        }
    }
}
