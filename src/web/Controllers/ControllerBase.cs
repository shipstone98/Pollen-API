using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Shipstone.Pollen.Api.Web.Controllers;

[ApiController]
[Authorize]
[Route("/api/[controller]")]
internal abstract class ControllerBase
    : Microsoft.AspNetCore.Mvc.ControllerBase
{ }
