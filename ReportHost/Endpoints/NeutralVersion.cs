using Asp.Versioning.Builder;

namespace ReportHost.Endpoints;

[ExcludeFromCodeCoverage]
public static class NeutralApiVersion
{
  public static ApiVersionSet BuildNeutralVersionSet(this WebApplication app) => app.NewApiVersionSet().IsApiVersionNeutral().Build();
}