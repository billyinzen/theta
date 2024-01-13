using Microsoft.AspNetCore.Mvc;

namespace Theta.Api.Features;

/// <summary>
/// Base class for all controllers in the Theta.Api project
/// </summary>
[ApiController]
[Route("api/[controller]")]
public abstract class ThetaController : ControllerBase
{
    protected void SetEntityTagHeader(string etag)
    {
        HttpContext.Response.Headers.ETag = etag;
    }
}