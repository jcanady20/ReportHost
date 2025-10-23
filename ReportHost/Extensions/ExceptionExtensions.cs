namespace System;

public static class ExceptionHelper
{
	public static string BuildExceptionMessage(this Exception e)
	{
		var msg = new Text.StringBuilder();
		msg.AppendLine(e.Message);
		if (e.InnerException != null) { msg.Append(e.InnerException.BuildExceptionMessage()); }
		return msg.ToString();
	}
}
