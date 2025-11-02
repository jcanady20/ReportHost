using System.Data;
using System.Text.Json;

namespace ReportHost.Extensions;

public static class DataReaderExtensions
{
	public static string Serialize(this IDataReader reader)
	{
		var result = String.Empty;
		var records = SerializeRecords(reader);
		result = JsonSerializer.Serialize(records);
		return result;
	}

	public static Dictionary<string, object> FlattenRecord(this IDataReader reader)
	{
		return SerializeRow(reader);
	}

	private static Dictionary<string, object> SerializeRow(IDataReader reader)
	{
		var result = new Dictionary<string, object>();
		foreach (var col in Columns(reader))
		{
			if (result.ContainsKey(col))
			{
				//	TODO :: Add Code to add # to duplicate column names
				//	TODO :: Handle empty column names
				continue;
			}
			result.Add(col, reader[col]);
		}

		return result;
	}

	private static IEnumerable<Dictionary<string, object>> SerializeRecords(IDataReader reader)
	{
		while(reader.Read())
		{
			yield return SerializeRow(reader);
		}
	}

	private static IEnumerable<string> Columns(IDataReader reader)
	{
		for(var i = 0; i < reader.FieldCount; i++)
		{
			yield return reader.GetName(i);
		}
	}
}

