using System.Text.Json;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Options;

namespace ReportHost.Internal;

[ExcludeFromCodeCoverage]
internal class ReportHostExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;
    private readonly IHostEnvironment _environment;
    private readonly ExceptionHandler _handler;
    private readonly JsonSerializerOptions _jsonOptions;

    public ReportHostExceptionHandler(IProblemDetailsService problemDetailsService, IOptions<JsonSerializerOptions> jsonOptions, IHostEnvironment environment, ExceptionHandler handler)
    {
        _problemDetailsService = problemDetailsService;
        _jsonOptions = jsonOptions.Value;
        _environment = environment;
        _handler = handler;
    }
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var problemDetail = _handler.HandleException(httpContext, exception, _environment);
        httpContext.Response.StatusCode = problemDetail.Status.Value;
        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            ProblemDetails = problemDetail,
            HttpContext = httpContext
        });
    }
}
