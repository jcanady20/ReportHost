using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace ReportHost.Data.Context
{
	public class ContextFactory
	{
		public static IContext CreateContext()
		{
			return Context.CreateContext();
		}
	}
}
