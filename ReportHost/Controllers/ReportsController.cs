using Microsoft.AspNetCore.Mvc;
using ReportHost.Data.Context;
using ReportHost.Data.Models;
using ReportHost.Data.Queries;
using ReportHost.Data.Reports;
using System.Data.Entity;

namespace ReportHost.Service.api
{
	[Route("api/reports")]
	public class ReportsController : Controller
	{
		private ReportContext _db;
		private ILogger _logger;
		private Generator _generator;

		public ReportsController(ILogger logger, ReportContext context, Generator generator)
		{
			_logger = logger;
			_db = context;
			_generator = generator;
		}

		[HttpGet]
		[Route("")]
		public async Task<IActionResult> GetReports()
		{

			var reports = await _db.Reports.AsNoTracking().Select(r => new { Id = r.Id, Name = r.Name }).ToListAsync();
			
			return Ok(reports);
		}

		[HttpGet]
		[Route("columns/{tableName}")]
		public async Task<IActionResult> GetTableColumns(string tableName)
		{
			var columns = await _db.Tables.GetTableColumnsAsync(tableName);

			return Ok(columns);
		}

		[HttpGet]
		[Route("tables")]
		public async Task<IActionResult> GetTables()
		{
			var tables = await _db.Tables.GetTables().Select(r => new TableDetail() { SchemaName = r.Schema.Name, TableName = r.Name }).ToListAsync();
			return Ok(tables);
		}

		[HttpPost]
		[Route("results")]
		public async Task<IActionResult> GetReportResults([FromBody]Criteria criteria)
		{
			var result = await _generator.GenerateAsync(criteria);
			return Ok(result);
		}
	}
}
