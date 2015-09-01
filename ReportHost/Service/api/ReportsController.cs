using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Results;
using System.Threading.Tasks;
using System.Data.Entity;

using ReportHost.Logging;
using ReportHost.Data.Models;
using ReportHost.Data.Reports;

namespace ReportHost.Service.api
{
	[RoutePrefix("api/reports")]
	public class ReportsController : BaseApiController
	{
		private ILogger m_logger;
		internal ReportsController()
		{
			m_logger = new NLogger();
		}

		[HttpGet]
		[Route("reports")]
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
				IEnumerable<ColumnDetail> columns = null;
				using (var rg = new Data.Reports.ReportGenerator())
				{
					columns = await rg.TableColumsAsync(tableName);
				}

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
				IEnumerable<TableDetail> tables = null;
				using(var rg = new Data.Reports.ReportGenerator())
				{
					tables = await rg.TableNamesAsync();
				}
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
		public async Task<IHttpActionResult> GetReportResults([FromBody]ReportCriteria criteria)
		{
			try
			{
				IEnumerable<IDictionary<string, object>> result = null;
				using(var rg = new Data.Reports.ReportGenerator())
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
