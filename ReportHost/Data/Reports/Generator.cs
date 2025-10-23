using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;

using ReportHost.Extensions;
using System.Configuration;
using ReportHost.Data.Entities;
using ReportHost.Data.Models;
using ReportHost.Data.Context;
using ReportHost.Data.Queries;

namespace ReportHost.Data.Reports
{
	public class Generator : IDisposable
	{
		private readonly ReportContext _db;
		private readonly IConfiguration _configuration;

		public Generator(ReportContext context, IConfiguration configuration)
		{
			_db = context;
			_configuration = configuration;
        }
		
		public IEnumerable<Dictionary<string, object>> Generate(Criteria criteria)
		{
			var baseQuery = GetReportQuery(criteria);
			var columnDetails = GetColumnDetails(baseQuery);
			var tsql = QueryGenerator.RenderReportQuery(baseQuery, criteria, columnDetails);
			return ExecuteResults(tsql);
		}

		public async Task<IEnumerable<IDictionary<string, object>>> GenerateAsync(Criteria criteria)
		{
			var baseQuery = await GetReportQueryAsync(criteria);
			var columnDetails = await GetColumnDetailsAsync(baseQuery);
			var tsql = QueryGenerator.RenderReportQuery(baseQuery, criteria, columnDetails);
			return await ExecuteResultsAsync(tsql);
		}

		public IEnumerable<Column> TableColumns(string tableName, string schemaName = "dbo")
		{
			var columns = _db.GetTableColumns(tableName, schemaName);
			return columns;
		}

		public async Task<IEnumerable<Column>> TableColumsAsync(string tableName, string schemaName = "dbo")
		{
			var columns = await _db.Tables.GetTableColumnsAsync(tableName, schemaName);
			return columns;
		}

		private string GetReportQuery(Criteria criteria)
		{
			var query = string.Empty;
			if (criteria.ReportId.HasValue)
			{
				var report = _db.GetReportById(criteria.ReportId.Value);
				if (report != null)
				{
					query = report.Query;
				}
			}
            else if (String.IsNullOrEmpty(criteria.TableName) == false)
            {
                query = String.Format("SELECT * FROM {0}", criteria.TableName);
            }
            if (String.IsNullOrEmpty(query))
            {
                throw new Exception("Unable to materize base query");
            }
            return query;
		}
		
		private async Task<string> GetReportQueryAsync(Criteria criteria)
		{
			var query = String.Empty;
            if (criteria.ReportId.HasValue)
            {
                var report = await _db.GetReportByIdAsync(criteria.ReportId.Value);
                if (report != null)
                {
                    query = report.Query;
                }
            }
            else if (String.IsNullOrEmpty(criteria.TableName) == false)
            {
                query = String.Format("SELECT * FROM {0}", criteria.TableName);
            }
            if(String.IsNullOrEmpty(query))
            {
                throw new Exception("Unable to materize base query");
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
			if (_db.Database is not null)
				return _db.Database.Connection.ConnectionString;
			
			return _configuration.GetConnectionString("SqlServer");
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
				_db.Dispose();
			}
		}
	}
}
