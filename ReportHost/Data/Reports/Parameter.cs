namespace ReportHost.Data.Reports;

[ExcludeFromCodeCoverage]
public class Parameter
{
	public Parameter()
	{
		//Values = new List<object>();
		Condition = Condition.AND;
	}
	public Condition Condition { get; set; }
	public Operator Operator { get; set; }
	public bool IsNot { get; set; }
	public string Key { get; set; }
	public object Value { get; set; }
	public IList<object> Values { get; set; }
}

