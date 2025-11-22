using System.Text;

using ReportHost.Data.Models;
using ReportHost.Extensions;


namespace ReportHost.Data.Reports;

internal class QueryGenerator
{
	internal static string RenderReportQuery(string baseQuery, Criteria criteria, ICollection<ColumnDetail> columnDetails)
	{
		var qryFrom = columnDetails.Select(r => r.BaseTableName).FirstOrDefault();

		var sb = new StringBuilder();
		sb.AppendFormat("DECLARE @page INT = {0}, @pageSize INT = {1};", criteria.Page, criteria.PageSize);
		sb.AppendLine(";WITH cteData AS");
		sb.AppendLine("(");
		sb.AppendLine("SELECT");
		sb.AppendLine(AddRowNumber(columnDetails));
		sb.AppendLine(AddColumns(criteria.Columns));
		sb.AppendFormat("FROM [{0}]", qryFrom);
		sb.AppendLine("WHERE (1=1)");
		//	Add Parameters
		sb.Append(AddParameters(criteria));
		sb.AppendLine(")");
		sb.AppendLine("SELECT * FROM cteData WHERE (1=1)");
		sb.AppendLine("	AND [RowId] BETWEEN ((@page - 1) * @pageSize) + 1 AND (@page * @pageSize)");
		return sb.ToString();
	}

	private static string AddColumns(IEnumerable<string> columns)
	{
		var sb = new StringBuilder();
		sb.Append(",");
		if (columns == null)
		{
			sb.Append("*");
		}
		else
		{
			sb.Append(String.Join(",", columns));
		}
		return sb.ToString();
	}

	private static string AddRowNumber(ICollection<ColumnDetail> columnDetails)
	{
		var sb = new StringBuilder();
		var pks = columnDetails.Where(x => x.IsKey).Select(r => r.ColumnName).ToList();
		sb.Append("ROW_NUMBER() OVER ( ORDER BY ");
		for (var i = 0; i < pks.Count; i++)
		{
			if (i > 0)
			{
				sb.Append(",");
			}
			sb.AppendFormat("[{0}]", pks[i]);
		}
		sb.Append(") AS [RowId]");
		return sb.ToString();
	}

	private static string AddParameters(Criteria criteria)
	{
		var sb = new StringBuilder();

		foreach (var kp in criteria.Parameters)
		{
			sb.AppendFormat(" {0}", AddCondition(kp.Condition));
			if (kp.IsNot)
			{
				sb.Append(" NOT");
			}
			//	Add Column reference
			sb.AppendFormat(" [{0}]", kp.Key);

			//	Add Operator and Value(s)
			//	Handle IN clauses
			if (kp.Operator == Operator.In)
			{
				sb.Append(" IN (");
				if (kp.Values != null && kp.Values.Count > 0)
				{
					sb.Append(AddValues(kp.Values));
				}
				else
				{
					sb.Append(AddValue(kp.Value));
				}
				sb.Append(")");
			}
			else if (kp.Operator == Operator.Between)
			{
				if (kp.Values.Count == 2)
				{
					sb.AppendFormat(" BETWEEN '{0}' AND '{1}'", kp.Values[0], kp.Values[1]);
				}
			}
			else
			{
				sb.AppendFormat(" {0} {1}", AddOperator(kp.Operator), AddValue(kp.Value));
			}
		}
		return sb.ToString();
	}

	private static string AddOperator(Operator reportOperator)
	{
		return reportOperator.GetAttribute<System.ComponentModel.DefaultValueAttribute>().Value.ToString();
	}

	private static string AddCondition(Condition reportCondition)
	{
		return reportCondition.GetAttribute<System.ComponentModel.DefaultValueAttribute>().Value.ToString();
	}

	private static string AddValues(IList<object> values)
	{
		var sb = new StringBuilder();
		for (int i = 0; i < values.Count; i++)
		{
			if (i > 0)
			{
				sb.Append(",");
			}
			sb.Append(AddValue(values[i]));
		}
		return sb.ToString();
	}

	private static string AddValue(object value)
	{
		var result = String.Empty;
		var type = value.GetType();
		if (RequiresSqlQuote(type))
		{
			result = String.Format("'{0}'", value);
		}
		else
		{
			result = value.ToString();
		}
		return result;
	}

	private static bool RequiresSqlQuote(Type dataType)
	{
		bool result = false;
		switch (dataType.ToString())
		{
			case "System.Char":
			case "System.String":
			case "System.DateTime":
			case "System.Guid":
				result = true;
				break;
		}
		return result;
	}

}

