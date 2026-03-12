using System;
using System.Collections.Generic;
using Microsoft.Extensions.Options;

namespace Shipstone.Pollen.Api.WebApi;

internal sealed class AuthenticationOptions : IOptions<AuthenticationOptions>
{
    internal Dictionary<String, String> _accounts;

    public Dictionary<String, String> Accounts
    {
        get => this._accounts;
        set => this._accounts = value;
    }

    AuthenticationOptions IOptions<AuthenticationOptions>.Value => this;

    public AuthenticationOptions() => this._accounts = new();
}
