using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportHost.Data.Entities
{
	public class Report
	{
		public Report()
		{
			Created = DateTime.Now;
		}

		public int Id { get; set; }
		public string Name { get; set; }
		public string Query { get; set; }
		public string CreatedBy { get; set; }
		public DateTime Created { get; set; }
	}
}
