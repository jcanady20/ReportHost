using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection.Extensions;

using System.Diagnostics;

namespace ReportHost.Internal;

[ExcludeFromCodeCoverage]
public class ExceptionHandler
{
  private readonly IDictionary<Type, ExceptionHandle> _exceptionMap;
  public ExceptionHandler(IDictionary<Type, ExceptionHandle> exceptionMap)
  {
    _exceptionMap = exceptionMap;
  }
  public ProblemDetails HandleException(HttpContext httpContext, Exception e, IHostEnvironment environment)
  {
    //  These properties are the default values
    //  They will be overwritten if the exception type is found in the exception map
    var statusCode = StatusCodes.Status500InternalServerError;
    var errorCode = "InternalServerError";
    var errorMessage = "Internal Server Error";
    var title = "An unknown error has occurred";
    var type = "";
    var exceptionType = e.GetType();
    if (_exceptionMap.ContainsKey(exceptionType))
    {
      var handler = _exceptionMap[exceptionType];
      statusCode = handler.StatusCode;
      errorCode = handler.ErrorCode;
      title = handler.Title is null ? exceptionType.Name : handler.Title;
      errorMessage = handler.MessageHandler is null ? e.Message : handler.MessageHandler(e);
      type = handler.Type;
    }
    return new ProblemDetails()
    {
      Title = title,
      Status = statusCode,
      Type = type,
      Detail = errorMessage,
      Extensions = { { "errorCode", errorCode } }
    };
  }
}

[ExcludeFromCodeCoverage]
public class ExceptionHandle
{
  /// <summary>
  /// Represents an exception handler for a specific exception type
  /// </summary>
  /// <param name="statusCode">HttpStatus code that represents this error type</param>
  /// <param name="errorCode">A short error code that that represents this issue. It SHOULD NOT change from occurrence to occurrence of the issue. </param>
  /// <param name="messageHandler">A <see cref="Func{Exception}"/> that generates the message from the given exception </param>
  /// <param name="title">A short, human-readable summary of the problem type. It SHOULD NOT change from occurrence to occurrence of the problem, except for purposes of localization(e.g., using proactive content negotiation; see[RFC7231], Section 3.4).</param>
  /// <param name="type">A URI reference [RFC3986] that identifies the problem type. This specification encourages that, when dereferenced, it provide human-readable documentation for the problem type (e.g., using HTML [W3C.REC-html5-20141028]). When this member is not present, its value is assumed to be "about:blank".</param>
  public ExceptionHandle(int statusCode, string errorCode, Func<Exception, string> messageHandler, string title = null, string type = null)
  {
    StatusCode = statusCode;
    ErrorCode = errorCode;
    MessageHandler = messageHandler;
    Title = title;
    Type = type;
  }
  public int StatusCode { get; }
  public string ErrorCode { get; }
  public Func<Exception, string> MessageHandler { get; }
  public string Title { get; }
  public string Type { get; }
}


[ExcludeFromCodeCoverage]
public class ExceptionHandlerBuilder
{
  private readonly Dictionary<Type, ExceptionHandle> _exceptionMap;
  private readonly IServiceCollection _services;
  public ExceptionHandlerBuilder(IServiceCollection services)
  {
    _exceptionMap = new Dictionary<Type, ExceptionHandle>();
    _services = services;
  }
  /// <summary>
  /// Adds an exception handler for the given exception type
  /// </summary>
  /// <param name="exceptionType">The <see cref="Type"/> of the underlying exception</param>
  /// <param name="statusCode">HttpStatus code that represents this error type</param>
  /// <param name="errorCode">A short error code that that represents this issue. It SHOULD NOT change from occurrence to occurrence of the issue. </param>
  /// <param name="messageHandler">A <see cref="Func{Exception}"/> that generates the message from the given exception </param>
  /// <param name="title">A short, human-readable summary of the problem type. It SHOULD NOT change from occurrence to occurrence of the problem, except for purposes of localization(e.g., using proactive content negotiation; see[RFC7231], Section 3.4).</param>
  /// <param name="type">A URI reference [RFC3986] that identifies the problem type. This specification encourages that, when dereferenced, it provide human-readable documentation for the problem type (e.g., using HTML [W3C.REC-html5-20141028]). When this member is not present, its value is assumed to be "about:blank".</param>
  /// <returns></returns>
  public ExceptionHandlerBuilder AddHandler(Type exceptionType, int statusCode = StatusCodes.Status500InternalServerError, string errorCode= "InternalServerError", Func<Exception, String> messageHandler = null, string title = null, string type = null)
  {
    if (!_exceptionMap.ContainsKey(exceptionType))
    {
      var exp = new ExceptionHandle(statusCode, errorCode, messageHandler, title, type);
      _exceptionMap.Add(exceptionType, exp);
    }
    return this;
  }
  /// <summary>
  /// Adds an exception handler for the given exception type
  /// </summary>
  /// <param name="statusCode">HttpStatus code that represents this error type</param>
  /// <param name="errorCode">A short error code that that represents this issue. It SHOULD NOT change from occurrence to occurrence of the issue. </param>
  /// <param name="messageHandler">A <see cref="Func{Exception}"/> that generates the message from the given exception </param>
  /// <param name="title">A short, human-readable summary of the problem type. It SHOULD NOT change from occurrence to occurrence of the problem, except for purposes of localization(e.g., using proactive content negotiation; see[RFC7231], Section 3.4).</param>
  /// <param name="type">A URI reference [RFC3986] that identifies the problem type. This specification encourages that, when dereferenced, it provide human-readable documentation for the problem type (e.g., using HTML [W3C.REC-html5-20141028]). When this member is not present, its value is assumed to be "about:blank".</param>
  /// <typeparam name="T"></typeparam>
  /// <returns></returns>
  public ExceptionHandlerBuilder AddHandler<T>(int statusCode = StatusCodes.Status500InternalServerError, string errorCode= "InternalServerError", Func<Exception, String> messageHandler = null, string title = null, string type = null) where T : Exception
  {
    return AddHandler(typeof(T), statusCode, errorCode, messageHandler, title, type);
  }
  public void Build()
  {
    _services.AddSingleton(new ExceptionHandler(_exceptionMap));
    _services.AddProblemDetails(options => {
      options.CustomizeProblemDetails = context => {
        context.ProblemDetails.Instance =
        $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
        var traceId = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;
        context.ProblemDetails.Extensions.TryAdd("traceId", traceId);
        context.ProblemDetails.Extensions.TryAdd("headers", context.HttpContext.Request.Headers);
        context.ProblemDetails.Extensions.TryAdd("parameters", context.HttpContext.Request.QueryString);
      };
    });
    _services.AddExceptionHandler<ReportHostExceptionHandler>();
  }
}

/// <summary>
/// Adds the exception handler to the service collection
/// </summary>
[ExcludeFromCodeCoverage]
public static class ExceptionHandlerExtensions
{
  public static ExceptionHandlerBuilder AddReportHostExceptionHandling(this IServiceCollection services)
  {
    return new ExceptionHandlerBuilder(services);
  }

  public static void UseReportHostExceptionHandler(this WebApplication app)
  {
    app.UseExceptionHandler(opts => { });
  }
}
