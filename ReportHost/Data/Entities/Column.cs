using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportHost.Data.Entities
{
	public class Column
	{
		public string Name { get; set; }
		public int TableId { get; set; }
		public byte DataTypeId { get; set; }
		public int Ordinal { get; set; }
		public bool IsNullable { get; set; }
		public bool IsIdentity { get; set; }

		public virtual DataType DataType { get; set; }
	}
}
