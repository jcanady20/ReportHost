namespace ReportHost.Data.Entities;

[ExcludeFromCodeCoverage]
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

