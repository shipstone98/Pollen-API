using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Shipstone.EntityFrameworkCore;
using Shipstone.Extensions.Security;

using Shipstone.Pollen.Api.Infrastructure.Entities;

namespace Shipstone.Pollen.Api.Infrastructure.Data.Repositories;

internal sealed class UserRepository : IUserRepository
{
    private readonly IDataSource _dataSource;
    private readonly INormalizationService _normalization;

    public UserRepository(
        IDataSource dataSource,
        INormalizationService normalization
    )
    {
        ArgumentNullException.ThrowIfNull(dataSource);
        ArgumentNullException.ThrowIfNull(normalization);
        this._dataSource = dataSource;
        this._normalization = normalization;
    }

    Task<UserEntity?> IUserRepository.RetrieveAsync(
        String emailAddress,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(emailAddress);

        String emailAddressNormalized =
            this._normalization.Normalize(emailAddress);

        return this._dataSource.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(
                u =>
                    String.Equals(
                        emailAddressNormalized,
                        u.EmailAddressNormalized
                    ),
                cancellationToken
            );
    }

    Task IUserRepository.UpdateAsync(
        UserEntity user,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(user);

        return this._dataSource.Users.SetStateAsync(
            user,
            DataEntityState.Updated,
            cancellationToken
        );
    }
}
