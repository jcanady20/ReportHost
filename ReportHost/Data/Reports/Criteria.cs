using System;
using System.Collections.Generic;

namespace ReportHost.Data.Reports
{
	public class Criteria
	{
		public Criteria()
		{
			Page = 1;
			PageSize = 10;
				 
		}
		public Nullable<int> ReportId { get; set; }
		public string TableName { get; set; }
		public IEnumerable<string> Columns { get; set; }
		public ICollection<Parameter> Parameters { get; set; }
		public int Page { get; set; }
		public int PageSize { get; set; }
	}
}