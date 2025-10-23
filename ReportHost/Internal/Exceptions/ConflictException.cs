
namespace ReportHost.Internal;

[ExcludeFromCodeCoverage]
public class ConflictException : Exception
{
    public ConflictException() : base()
    { }
    public ConflictException(string message) : base(message)
    { }
    public ConflictException(string message, Exception exception) : base(message, exception)
    { }
}
