using System;
using System.Collections.Generic;

namespace ReportHost.Data
{
	public class PagedList<T>
	{
		public int TotalCount { get; set; }
		public int TotalPages { get; set; }
		public List<T> Entities { get; set; }
	}
}
