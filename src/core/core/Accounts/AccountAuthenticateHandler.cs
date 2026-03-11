using System;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.Extensions.Identity;
using Shipstone.Utilities;

using Shipstone.Pollen.Api.Infrastructure.Data;
using Shipstone.Pollen.Api.Infrastructure.Entities;

namespace Shipstone.Pollen.Api.Core.Accounts;

internal sealed class AccountAuthenticateHandler : IAccountAuthenticateHandler
{
    private readonly IPasswordService _password;
    private readonly IRepository _repository;

    public AccountAuthenticateHandler(
        IRepository repository,
        IPasswordService password
    )
    {
        ArgumentNullException.ThrowIfNull(repository);
        ArgumentNullException.ThrowIfNull(password);
        this._password = password;
        this._repository = repository;
    }

    private async Task HandleAsync(
        String emailAddress,
        String password,
        CancellationToken cancellationToken
    )
    {
        UserEntity? user =
            await this._repository.Users.RetrieveAsync(
                emailAddress,
                cancellationToken
            );

        if (user is null)
        {
            throw new NotFoundException("A user whose email address matches the provided email address could not be found.");
        }

        bool isSecure;

        try
        {
            isSecure = this._password.Verify(user.PasswordHash, password);
        }

        catch (IncorrectPasswordException ex)
        {
            throw new IncorrectPasswordException(
                "The hashed representation of the provided password does not match the password hash of the user whose email address matches the provided email address.",
                ex
            );
        }

        if (!isSecure)
        {
            user.PasswordHash = this._password.Hash(password);
            await this._repository.Users.UpdateAsync(user, cancellationToken);
            await this._repository.SaveAsync(cancellationToken);
        }
    }

    Task IAccountAuthenticateHandler.HandleAsync(
        String emailAddress,
        String password,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(emailAddress);
        ArgumentNullException.ThrowIfNull(password);
        return this.HandleAsync(emailAddress, password, cancellationToken);
    }
}
