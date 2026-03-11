using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Controllers;

using Shipstone.Pollen.Api.Web.Controllers;

namespace Shipstone.Pollen.Api.Web;

internal sealed class PollenControllerFeatureProvider
    : ControllerFeatureProvider
{
    private readonly IReadOnlySet<Type> _controllers;

    internal PollenControllerFeatureProvider() =>
        this._controllers = new HashSet<Type> { typeof (PollenController) };

    protected sealed override bool IsController(TypeInfo typeInfo) =>
        this._controllers.Contains(typeInfo);
}
