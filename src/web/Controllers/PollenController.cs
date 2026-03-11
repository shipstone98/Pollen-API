using System;
using System.Threading;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using Shipstone.Utilities.Linq;

using Shipstone.Pollen.Api.Core.Pollen;
using Shipstone.Pollen.Api.Web.Models.Pollen;

namespace Shipstone.Pollen.Api.Web.Controllers;

internal sealed class PollenController : ControllerBase
{
    [ActionName("List")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [Route("/api/[controller]/[action]")]
    public IActionResult List(
        [FromServices] IPollenListHandler handler,
        [FromQuery] double latitude,
        [FromQuery] double longitude,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(handler);
        ArgumentOutOfRangeException.ThrowIfLessThan(latitude, -90);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(latitude, 90);
        ArgumentOutOfRangeException.ThrowIfLessThan(longitude, -180);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(longitude, 180);

        Object? response =
            handler
                .HandleAsync(latitude, longitude, cancellationToken)
                .SelectAsync((p, _) => new RetrieveResponse(p));

        return this.Ok(response);
    }
}
