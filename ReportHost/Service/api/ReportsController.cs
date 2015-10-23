using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Results;
using System.Threading.Tasks;
using System.Data.Entity;

using ReportHost.Logging;
using ReportHost.Data.Context;
using ReportHost.Data.Models;
using ReportHost.Data.Reports;
using ReportHost.Data.Queries;

namespace ReportHost.Service.api
{
	[RoutePrefix("api/reports")]
	public class ReportsController : ApiController
	{
		private IContext m_db;
		private ILogger m_logger;

		public ReportsController(ILogger logger, IContext context)
		{
			m_logger = logger;
			m_db = context;
		}

		[HttpGet]
		[Route("")]
		public async Task<IHttpActionResult> GetReports()
		{
			try
			{
				var reports = await m_db.Reports.AsNoTracking().Select(r => new { Id = r.Id, Name = r.Name }).ToListAsync();
				
				return Ok(reports);
			}
			catch (Exception e)
			{
				m_logger.Error(e);
				return new ExceptionResult(e, this);
			}
		}

		[HttpGet]
		[Route("columns/{tableName}")]
		public async Task<IHttpActionResult> GetTableColumns(string tableName)
		{
			try
			{
				var columns = await m_db.Tables.GetTableColumnsAsync(tableName);

				return Ok(columns);
			}
			catch(Exception e)
			{
				m_logger.Error(e);
				return new ExceptionResult(e, this);
			}
		}

		[HttpGet]
		[Route("tables")]
		public async Task<IHttpActionResult> GetTables()
		{
			try
			{
				var tables = await m_db.Tables.GetTables().Select(r => new TableDetail() { SchemaName = r.Schema.Name, TableName = r.Name }).ToListAsync();
				return Ok(tables);
			}
			catch(Exception e)
			{
				m_logger.Error(e);
				return new ExceptionResult(e, this);
			}
		}

		[HttpPost]
		[Route("results")]
		public async Task<IHttpActionResult> GetReportResults([FromBody]Criteria criteria)
		{
			try
			{
				IEnumerable<IDictionary<string, object>> result = null;
				using(var rg = new Data.Reports.Generator(m_db))
				{
					result = await rg.GenerateAsync(criteria);
				}
				return Ok(result);
			}
			catch(Exception e)
			{
				m_logger.Error(e);
				return new ExceptionResult(e, this);
			}
		}
	}
}
