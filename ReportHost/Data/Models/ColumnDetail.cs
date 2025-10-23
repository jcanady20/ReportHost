namespace ReportHost.Data.Models;

public class ColumnDetail
{
	public string BaseTableName { get; set;  }
	public string ColumnName { get; set; }
	public int ColumnOrdinal { get; set; }
	public bool IsKey { get; set; }
	public bool IsUnique { get; set; }
	public String DataType { get; set; }
}

