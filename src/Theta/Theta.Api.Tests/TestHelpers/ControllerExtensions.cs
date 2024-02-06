using Theta.Api.Features;

namespace Theta.Api.Tests.TestHelpers;

public static class ControllerExtensions
{
    public static string? GetResponseEtag(this ThetaController controller)
        => controller.ControllerContext.HttpContext.Response.Headers.ETag;
}