using System;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading;

using Microsoft.Owin.Hosting;
using System.ServiceProcess;

namespace ReportHost
{
	partial class Program
	{
		protected static Service.ReportService m_service;
		protected static Type m_type = typeof(Program);
		protected static List<MethodInfo> m_methods = null;

		[STAThread]
		static void Main(string[] args)
		{
			if (Environment.UserInteractive == false)
			{
				ServiceBase[] ServicesToRun = new ServiceBase[] { new Service.ReportService() };
				ServiceBase.Run(ServicesToRun);
			}
			else
			{
				CreateMethodCache();
				AddTraceListeners();

				Console.Clear();
				Console.ForegroundColor = ConsoleColor.White;
				Console.BufferHeight = 300;
				Console.BufferWidth = 100;
				Console.Title = "Report Service Host";

				if (args.Length > 0)
				{
					RunCommand(args);
					return;
				}

				bool pContinue = true;
				while (pContinue)
				{
					Console.Write(":>");

					string[] Coms = Console.ReadLine().Split(new char[] { ' ' });
					if (Coms.Length == 0)
						continue;

					switch (Coms[0].ToLower())
					{
						case "exit":
						case "quit":
							pContinue = false;
							break;
						default:
							RunCommand(Coms);
							break;
					}
					Console.WriteLine("");
				}
			}
		}

		static void RunCommand(string[] args)
		{
			string CallingMethod = args[0];
			object[] param = new object[] { };
			if (args.Length > 1)
			{
				//Remove the First Param which should be the calling Method
				param = new object[args.Length - 1];
				for (int i = 0; i < param.Length; i++)
				{
					int j = i;
					param[i] = args[++j];
				}
			}

			foreach (MethodInfo mi in m_methods)
			{
				if (
					//	Validate the File Name
						String.Compare(mi.Name.ToLower(), CallingMethod.ToLower(), true) != 0
					//	Validate the Parameter Count
						|| mi.GetParameters().Length != param.Length
					)
				{
					continue;
				}

				try
				{
					object prog = System.Activator.CreateInstance(m_type);
					m_type.InvokeMember(mi.Name, BindingFlags.Default | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, prog, param);
					return;
				}
				catch (Exception e)
				{
					if (e.InnerException != null)
					{
						Console.WriteLine();
						WriteExceptions(e.InnerException);
					}
					return;
				}
			}
			// If we fell out of the loop and didn't find a Matching Method, then Call Help.
			Console.WriteLine("Unknown Command");
			Program.Help();
		}

		[Description("Shows Help for All Commands")]
		static void Help()
		{
			Console.WriteLine("Valid Commands");
			foreach (MethodInfo m in m_methods)
			{
				DescriptionAttribute[] attribs = (DescriptionAttribute[])m.GetCustomAttributes(typeof(DescriptionAttribute), false);
				if (attribs != null && attribs.Length > 0)
				{
					Console.ForegroundColor = ConsoleColor.Green;
					Console.Write(m.Name);
					ParameterInfo[] parm = m.GetParameters();
					Console.ForegroundColor = ConsoleColor.Cyan;
					Console.Write("(");
					for (int i = 0; i < parm.Length; i++)
					{
						if (i > 0)
							Console.Write(", ");

						Console.Write("({0}){1}", parm[i].ParameterType.Name, parm[i].Name);
					}
					Console.Write(")");
					Console.ForegroundColor = ConsoleColor.White;
					Console.WriteLine("\n\t{0}", attribs[0].Description);
				}
			}
		}

		[Description("Clears the Current display Buffer")]
		static void Clear()
		{
			Console.Clear();
		}

		[Description("Quits out of the application")]
		static void Quit()
		{
			return;
		}

		[Description("List Local Drives")]
		static void LocalDrives()
		{
			string[] drives = Environment.GetLogicalDrives();
			IEnumerable<string> strs = drives.Select(s => s.Replace(":\\", ""));
			foreach (String s in strs)
			{
				System.IO.DriveInfo drvi = new System.IO.DriveInfo(s);
				if (drvi.DriveType == DriveType.CDRom)
					continue;
				Console.WriteLine("{0}:\\", s);
			}
		}

		[Description("Open Application Log Folder")]
		static void OpenLogFolder()
		{
			Process.Start(Path.Combine(GetCurrentPath(), "ApplicationLogs"));
		}

		[System.ComponentModel.Description("Install Service")]
		static void InstallService()
		{
			try
			{
				// "/LogFile=" - to suppress install log creation
				System.Configuration.Install.ManagedInstallerClass.InstallHelper(new string[] { "/LogFile=", System.Reflection.Assembly.GetExecutingAssembly().Location });
			}
			catch { }
		}

		[System.ComponentModel.Description("Uninstall Service")]
		static void UninstallService()
		{
			try
			{
				// "/LogFile=" - to suppress uninstall log creation
				System.Configuration.Install.ManagedInstallerClass.InstallHelper(new string[] { "/u", "/LogFile=", System.Reflection.Assembly.GetExecutingAssembly().Location });
			}
			catch { }
		}

		[System.ComponentModel.Description("Create Service EventLog")]
		static void CreateEventLog()
		{
			if (!System.Diagnostics.EventLog.SourceExists(Service.ReportService.EventLogSource))
			{
				System.Diagnostics.EventLog.CreateEventSource(Service.ReportService.EventLogSource, Service.ReportService.EventLogName);
			}
		}

		[System.ComponentModel.Description("Start Service Interactively")]
		static void StartService()
		{
			m_service = new Service.ReportService();
			m_service.Start();
		}

		[System.ComponentModel.Description("Stops the Service")]
		static void StopService()
		{
			if (m_service != null && m_service.CanStop)
				m_service.Stop();
		}

		static void ls()
		{
			var path = @"C:\Windows";
			var di = new DirectoryInfo(path);
			foreach (var file in di.EnumerateFiles())
			{
				Console.WriteLine(file.Name);
			}
		}

		static TimeSpan CalculateEta(DateTime startTime, int totalItems, int completeItems)
		{
			TimeSpan _eta = TimeSpan.MinValue;
			//	Avoid Divide by Zero Errors
			if (completeItems > 0)
			{
				int _itemduration = (int)DateTime.Now.Subtract(startTime).TotalMilliseconds / completeItems;
				_eta = TimeSpan.FromMilliseconds((double)((totalItems - completeItems) * _itemduration));
			}
			return _eta;
		}

		static void WriteExceptions(Exception e)
		{
			Console.BackgroundColor = ConsoleColor.Black;
			Console.ForegroundColor = ConsoleColor.Red;

			Trace.Write("Source:");
			Trace.Write(e.Source);
			Trace.WriteLine("\nMessage:");
			Trace.Write(e.Message);
			Trace.WriteLine("\nStack Trace:");
			Trace.Write(e.StackTrace);
			Trace.WriteLine("\nUser Defined Data:");
			foreach (System.Collections.DictionaryEntry de in e.Data)
			{
				Trace.WriteLine(string.Format("[{0}] :: {1}", de.Key, de.Value));
			}
			if (e.InnerException != null)
			{
				WriteExceptions(e.InnerException);
			}
			Console.ForegroundColor = ConsoleColor.White;
		}

		static string GetCurrentPath()
		{
			var asm = Assembly.GetExecutingAssembly();
			var fi = new FileInfo(asm.Location);
			return fi.DirectoryName;
		}

		static void HexDump(byte[] bytes)
		{
			for (int line = 0; line < bytes.Length; line += 16)
			{
				byte[] lineBytes = bytes.Skip(line).Take(16).ToArray();
				System.Text.StringBuilder sb = new System.Text.StringBuilder();
				sb.AppendFormat("{0:x8} ", line);
				sb.Append(string.Join(" ", lineBytes.Select(b => b.ToString("x2")).ToArray()).PadRight(16 * 3));
				sb.Append(" ");
				sb.Append(new string(lineBytes.Select(b => b < 32 ? '.' : (char)b).ToArray()));
				Console.WriteLine(sb);
			}
		}

		static void CreateMethodCache()
		{
			m_methods = m_type.GetMethods(BindingFlags.Static | BindingFlags.NonPublic).ToList();
		}

		static void AddTraceListeners()
		{
			TextWriterTraceListener CWriter = new TextWriterTraceListener(Console.Out);
			Trace.Listeners.Add(CWriter);
		}
	}
}