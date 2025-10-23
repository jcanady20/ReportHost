namespace ReportHost.Data.Entities;

public class Table
{
	public Table()
	{ }

	public int Id { get; set; }
	public string Name { get; set; }
	public bool IsMSShipped { get; set; }
	public string ObjectType { get; set; }
	public int SchemaId { get; set; }

	public virtual Schema Schema { get; set; }
	public virtual ICollection<Column> Columns { get; set; }
}

