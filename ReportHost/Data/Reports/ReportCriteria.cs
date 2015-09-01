using System;
using System.Collections.Generic;

namespace ReportHost.Data.Reports
{
	public class ReportCriteria
	{
		public ReportCriteria()
		{
			Page = 1;
			PageSize = 10;
				 
		}
		public Nullable<int> ReportId { get; set; }
		public string Query { get; set; }
		public IList<string> Columns { get; set; }
		public ICollection<ReportParameter> Parameters { get; set; }
		public int Page { get; set; }
		public int PageSize { get; set; }
	}
}