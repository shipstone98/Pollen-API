using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

using Shipstone.AspNetCore.Authentication.Basic;
using Shipstone.Extensions.Identity;

namespace Shipstone.Pollen.Api.WebApi;

internal sealed class PollenBasicAuthenticateHandler
    : IBasicAuthenticateHandler
{
    private readonly AuthenticationOptions _options;
    private readonly IPasswordService _password;

    public PollenBasicAuthenticateHandler(
        IPasswordService password,
        IOptions<AuthenticationOptions>? options
    )
    {
        this._options = options?.Value ?? new();
        this._password = password;
    }

    Task<IEnumerable<Claim>> IBasicAuthenticateHandler.HandleAsync(
        String userId,
        String password,
        CancellationToken cancellationToken
    )
    {
        if (!this._options._accounts.TryGetValue(
            userId,
            out String? passwordHash
        ))
        {
            throw new BasicAuthenticateException();
        }

        try
        {
            this._password.Verify(passwordHash, password);
        }

        catch (IncorrectPasswordException)
        {
            throw new BasicAuthenticateException();
        }

        IEnumerable<Claim> claims = Array.Empty<Claim>();
        return Task.FromResult(claims);
    }
}
