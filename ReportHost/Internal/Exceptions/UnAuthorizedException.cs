namespace ReportHost.Internal;

[ExcludeFromCodeCoverage]
public class UnauthorizedException : Exception
{
  public UnauthorizedException() : base()
  { }
  public UnauthorizedException(string message) : base(message)
  { }
  public UnauthorizedException(string message, Exception exception) : base(message, exception)
  { }
}
