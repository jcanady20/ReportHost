using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Threading.Tasks;

namespace ReportHost.Service
{
	public static class ServiceUrl
	{
		public static string GetServiceUrl()
		{
			return ConfigurationManager.AppSettings["baseAddress"];
		}
	}
}
