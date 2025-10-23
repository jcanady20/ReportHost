using Microsoft.AspNetCore.Authorization;

namespace ReportHost.Endpoints;

[ExcludeFromCodeCoverage]
public static class VersionEndpoint
{
  private static (Version Version, string Name) GetAssemblyVersion()
  {
    var assm = System.Reflection.Assembly.GetEntryAssembly();
    var details = assm.GetName();
    var version = details.Version;
    return (version, details.Name);
  }

  public static WebApplication AddVersionEndpoint(this WebApplication app, string endpointName)
  {
    var versionSet = NeutralApiVersion.BuildNeutralVersionSet(app);
    app.MapGet(endpointName,
        [AllowAnonymous]
        () => {
          var (version, name) = GetAssemblyVersion();
          return Results.Ok(new {
            name = name,
            major = version.Major,
            minor = version.Minor,
            build = version.Build,
            version = version.ToString(),
          });
      }
    ).WithApiVersionSet(versionSet);

    return app;
  }
}