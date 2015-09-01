using System;

namespace ReportHost.Logging
{
	interface ILogger
	{
		void Trace(string message);
		void Trace(string message, params object[] args);
		void Info(string message);
		void Info(string message, params object[] args);
		void Debug(string message);
		void Debug(string message, params object[] args);
		void Warn(string message);
		void Warn(string message, params object[] args);
		void Error(Exception x);
		void Fatal(Exception x);
	}
}
