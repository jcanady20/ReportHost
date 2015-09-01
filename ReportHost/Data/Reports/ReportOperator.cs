using System.ComponentModel;

namespace ReportHost.Data.Reports
{
	public enum ReportOperator
	{
		[DefaultValue("=")]
		Equals = 1,
		[DefaultValue("<>")]
		NotEquals = 2,
		[DefaultValue(">")]
		GreaterThan = 3,
		[DefaultValue(">=")]
		GreaterThanEquals = 4,
		[DefaultValue("<")]
		LessThan = 5,
		[DefaultValue("<=")]
		LessThanEquals = 6,
		[DefaultValue("LIKE")]
		Like = 7,
		[DefaultValue("IN")]
		In = 8,
		[DefaultValue("BETWEEN")]
		Between = 9,
	}
}
