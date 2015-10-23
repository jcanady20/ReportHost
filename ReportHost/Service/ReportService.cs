using System;
using System.Diagnostics;
using System.ServiceProcess;
using Microsoft.Owin.Hosting;
using ReportHost.Logging;

namespace ReportHost.Service
{
	public class ReportService : ServiceBase
	{
		private EventLog m_eventlog;
		private static string m_eventlogName = "ReportService";
		private static string m_eventlogSource = "ReportService";
		private IDisposable m_owin;
		private ILogger m_logger = new NLogger();


		public ReportService()
		{
			BaseAddress = ServiceUrl.GetServiceUrl();
			base.CanStop = true;
			base.CanShutdown = true;
			base.CanPauseAndContinue = false;
			base.AutoLog = true;

			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
		}

		public void Start()
		{
			m_logger.Trace("Starting Report Host Service");
			m_owin = WebApp.Start<Startup>(url: BaseAddress);
			m_logger.Info("Report Host Listening @ " + BaseAddress);
		}

		public static String EventLogName
		{
			get
			{
				return m_eventlogName;
			}
		}

		public static String EventLogSource
		{
			get
			{
				return m_eventlogSource;
			}
		}

		public string BaseAddress { get; private set; }

		protected override void OnStart(string[] args)
		{
			Start();
			base.OnStart(args);
		}

		protected override void OnStop()
		{
			m_logger.Trace("Stopping License Service");
			if (m_owin != null)
			{
				m_owin.Dispose();
			}
			base.OnStop();
		}

		protected override void OnShutdown()
		{
			m_logger.Trace("Shutting Down Test Service");
			base.OnShutdown();
		}

		public override EventLog EventLog
		{
			get
			{
				if (m_eventlog == null)
					m_eventlog = new EventLog(m_eventlogName);

				return m_eventlog;
			}
		}

		private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			//	Get the Exception Object
			var excp = e.ExceptionObject as Exception;
			if (excp == null)
				return;

			m_logger.Error(excp);



			var message = excp.BuildExceptionMessage();
			//	Create an EventLogEntry
			this.EventLog.WriteEntry(message, EventLogEntryType.Error, 1002);

		}
	}
}
