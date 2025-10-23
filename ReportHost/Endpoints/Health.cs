using Microsoft.AspNetCore.Authorization;

namespace ReportHost.Endpoints;

[ExcludeFromCodeCoverage]
public static class HealthEndpoint
{
  public static WebApplication AddHealthEndpoint(this WebApplication app, string endpointName)
  {
    var versionSet = NeutralApiVersion.BuildNeutralVersionSet(app);
    app.MapGet(endpointName,
        [AllowAnonymous]
        () => {
        return Results.Ok("ok");
      }
    ).WithApiVersionSet(versionSet);
    return app;
  }
}