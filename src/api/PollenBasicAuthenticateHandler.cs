using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

using Shipstone.AspNetCore.Authentication.Basic;

using Shipstone.Pollen.Api.Core.Accounts;

namespace Shipstone.Pollen.Api.WebApi;

internal sealed class PollenBasicAuthenticateHandler
    : IBasicAuthenticateHandler
{
    private readonly IAccountAuthenticateHandler _handler;

    public PollenBasicAuthenticateHandler(IAccountAuthenticateHandler handler) =>
        this._handler = handler;

    async Task<IEnumerable<Claim>> IBasicAuthenticateHandler.HandleAsync(
        String userId,
        String password,
        CancellationToken cancellationToken
    )
    {
        try
        {
            await this._handler.HandleAsync(
                userId,
                password,
                cancellationToken
            );
        }

        catch (Exception ex)
        {
            throw new BasicAuthenticateException(
                "The current user could not be authenticated.",
                ex
            );
        }

        return Array.Empty<Claim>();
    }
}
