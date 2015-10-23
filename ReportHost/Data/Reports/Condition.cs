using System;
using System.ComponentModel;

namespace ReportHost.Data.Reports
{
	public enum Condition
	{
		[DefaultValue("AND")]
		AND = 1,
		[DefaultValue("OR")]
		OR	= 2,
	}
}
