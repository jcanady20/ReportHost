using System.Collections.Generic;

namespace ReportHost.Data.Reports
{
	public class ReportParameter
	{
		public ReportParameter()
		{
			//Values = new List<object>();
			Condition = ReportCondition.AND;
		}
		public ReportCondition Condition { get; set; }
		public ReportOperator Operator { get; set; }
		public bool IsNot { get; set; }
		public string Key { get; set; }
		public object Value { get; set; }
		public IList<object> Values { get; set; }
	}
}
