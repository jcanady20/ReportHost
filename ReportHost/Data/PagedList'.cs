namespace ReportHost.Data;

[ExcludeFromCodeCoverage]
public class PagedList<T>
{
	public int TotalCount { get; set; }
	public int TotalPages { get; set; }
	public List<T> Entities { get; set; }
}

