using Microsoft.AspNetCore.Mvc;

namespace Theta.Api.Features;

/// <summary>
/// Base class for all controllers in the Theta.Api project
/// </summary>
[ApiController]
[Route("api/[controller]")]
public abstract class ThetaController : ControllerBase
{
    /// <summary>
    /// Set the "etag" header value in the HTTP response
    /// </summary>
    /// <param name="etag"></param>
    protected void SetEntityTagHeader(string etag)
        => HttpContext.Response.Headers.ETag = etag;
}