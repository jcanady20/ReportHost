using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Threading.Tasks;

using ReportHost.Extensions;
using System.Configuration;
using ReportHost.Data.Models;
using ReportHost.Data.Context;

namespace ReportHost.Data.Reports
{
	public class ReportGenerator : IDisposable
	{
		private IContext m_db;

		public ReportGenerator(IContext context)
		{
			m_db = context;
        }
		
		public IEnumerable<Dictionary<string, object>> Generate(ReportCriteria criteria)
		{
			var baseQuery = GetReportQuery(criteria);
			var columnDetails = GetColumnDetails(baseQuery);
			var tsql = QueryGenerator.RenderReportQuery(baseQuery, criteria, columnDetails);
			return ExecuteResults(tsql);
		}

		public async Task<IEnumerable<IDictionary<string, object>>> GenerateAsync(ReportCriteria criteria)
		{
			var baseQuery = await GetReportQueryAsync(criteria);
			var columnDetails = await GetColumnDetailsAsync(baseQuery);
			var tsql = QueryGenerator.RenderReportQuery(baseQuery, criteria, columnDetails);
			return await ExecuteResultsAsync(tsql);
		}

		public IEnumerable<TableDetail> TableNames()
		{
			return GetTableNames();
		}
		
		public async Task<IEnumerable<TableDetail>> TableNamesAsync()
		{
			return await GetTableNamesAsync();
		}

		public IEnumerable<ColumnDetail> TableColumns(string tableName)
		{
			var tsql = String.Format("SELECT * FROM [{0}]", tableName);
			return GetColumnDetails(tsql);
		}

		public async Task<IEnumerable<ColumnDetail>> TableColumsAsync(string tableName)
		{
			var tsql = String.Format("SELECT * FROM [{0}]", tableName);
			var columnDetails = await GetColumnDetailsAsync(tsql);
			return columnDetails;
		}

		private string GetReportQuery(ReportCriteria criteria)
		{
			var query = string.Empty;
			if (criteria.ReportId.HasValue)
			{
				var report = m_db.Reports.FirstOrDefault(x => x.Id == criteria.ReportId.Value);
				if (report != null)
				{
					query = report.Query;
				}
			}
			else if (String.IsNullOrEmpty(criteria.Query) == false)
			{
				query = criteria.Query;
			}
			return query;
		}
		
		private async Task<string> GetReportQueryAsync(ReportCriteria criteria)
		{
			var query = String.Empty;
			if(criteria.ReportId.HasValue)
			{
				var report = await m_db.Reports.FirstOrDefaultAsync(x => x.Id == criteria.ReportId.Value);
				if(report != null)
				{
					query = report.Query;
				}
				else if (String.IsNullOrEmpty(criteria.Query) == false)
				{
					query = criteria.Query;
				}
			}
			return query;
		}

		private ICollection<ColumnDetail> GetColumnDetails(string tsql)
		{
			var columnDetails = new List<ColumnDetail>();
			using(var conn = new SqlConnection(GetConnectionString()))
			{
				conn.Open();
				using(var cmd = conn.CreateCommand())
				{
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = tsql;

					var reader = cmd.ExecuteReader(CommandBehavior.KeyInfo | CommandBehavior.SchemaOnly);
					columnDetails = reader
						.GetSchemaTable()
						.AsEnumerable()
						.Select(r =>
							new ColumnDetail()
							{
								IsUnique = r.GetBoolean("IsUnique"),
								IsKey = r.GetBoolean("IsKey"),
								ColumnName = r.GetString("ColumnName"),
								ColumnOrdinal = r.GetInt32("ColumnOrdinal"),
								BaseTableName = r.GetString("BaseTableName"),
								DataType = r.Get<Type>("DataType").Name
							}).ToList();
				}
			}
			return columnDetails;
		}

		private async Task<ICollection<ColumnDetail>> GetColumnDetailsAsync(string tsql)
		{
			var columnDetails = new List<ColumnDetail>();
			using(var conn = new SqlConnection(GetConnectionString()))
			{
				await conn.OpenAsync();
				using(var cmd = conn.CreateCommand())
				{
					cmd.CommandType = CommandType.Text;
					cmd.CommandText = tsql;
					var reader = await cmd.ExecuteReaderAsync(CommandBehavior.KeyInfo | CommandBehavior.SchemaOnly);
					columnDetails = reader
						.GetSchemaTable()
						.AsEnumerable()
						.Select(r => new ColumnDetail() {
							IsUnique = r.GetBoolean("IsUnique"),
							IsKey = r.GetBoolean("IsKey"),
							ColumnName = r.GetString("ColumnName"),
							ColumnOrdinal = r.GetInt32("ColumnOrdinal"),
							BaseTableName = r.GetString("BaseTableName"),
							DataType = r.Get<Type>("DataType").Name
						}).ToList();
				}
			}
			return columnDetails;
		}

		private ICollection<TableDetail> GetTableNames()
		{
			var tableDetails = new List<TableDetail>();
			var tsql = "SELECT [s].[name] AS [Schema], [o].[Name] AS [Table] FROM [sys].[objects] AS [o] INNER JOIN [sys].[schemas] AS [s] ON [s].[schema_id] = [o].[schema_id] WHERE [Type] = 'U' ORDER BY [o].[Name]";
			using(var conn = new SqlConnection(GetConnectionString()))
			{
				conn.Open();
				using(var cmd = conn.CreateCommand())
				{
					cmd.CommandText = tsql;
					cmd.CommandType = CommandType.Text;

					using(var reader = cmd.ExecuteReader())
					{
						while(reader.Read())
						{
							tableDetails.Add(new TableDetail() { TableName = reader.GetString(0), SchemaName = reader.GetString(1) });
						}
					}
				}
			}

			return tableDetails;
		}

		private async Task<ICollection<TableDetail>> GetTableNamesAsync()
		{
			var tableDetails = new List<TableDetail>();
			var tsql = "SELECT [s].[name] AS [Schema], [o].[Name] AS [Table] FROM [sys].[objects] AS [o] INNER JOIN [sys].[schemas] AS [s] ON [s].[schema_id] = [o].[schema_id] WHERE [Type] = 'U' ORDER BY [o].[Name]";
			using (var conn = new SqlConnection(GetConnectionString()))
			{
				conn.Open();
				using (var cmd = conn.CreateCommand())
				{
					cmd.CommandText = tsql;
					cmd.CommandType = CommandType.Text;

					using (var reader = await cmd.ExecuteReaderAsync())
					{
						while (reader.Read())
						{
							tableDetails.Add(new TableDetail() { TableName = reader.GetString(0), SchemaName = reader.GetString(1) });
						}
					}

				}
			}


			return tableDetails;
		}

		private IEnumerable<Dictionary<string, object>> ExecuteResults(string tsql)
		{
			using(var conn = new SqlConnection(GetConnectionString()))
			{
				conn.Open();

				using(var cmd = conn.CreateCommand())
				{
					cmd.CommandType = CommandType.Text;
					cmd.CommandTimeout = 60;
					cmd.CommandText = tsql;
					using(var reader = cmd.ExecuteReader(CommandBehavior.SequentialAccess))
					{
						if (reader.HasRows)
						{
							while (reader.Read())
							{
								yield return reader.FlattenRecord();
							}
						}
					}
				}
			}
		}

		private async Task<IEnumerable<IDictionary<string, object>>> ExecuteResultsAsync(string tsql)
		{
			var records = new List<Dictionary<string, object>>();
			using (var conn = new SqlConnection(GetConnectionString()))
			{
				await conn.OpenAsync();

				using (var cmd = conn.CreateCommand())
				{
					cmd.CommandType = CommandType.Text;
					cmd.CommandTimeout = 60;
					cmd.CommandText = tsql;
					using (var reader = await cmd.ExecuteReaderAsync())
					{
						if (reader.HasRows)
						{
							while (reader.Read())
							{
								records.Add(reader.FlattenRecord());
							}
						}
					}
				}
			}
			return records;
		}

		private string GetConnectionString()
		{
			var connSting = string.Empty;
			if(m_db.Database != null)
			{
				connSting = m_db.Database.Connection.ConnectionString;
			}
			else
			{
				connSting = ConfigurationManager.ConnectionStrings["SqlServer"].ConnectionString;
			}

			return connSting;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if(disposing)
			{
				m_db.Dispose();
			}
		}
	}
}
