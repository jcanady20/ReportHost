using Microsoft.AspNetCore.Mvc;
using ReportHost.Data.Context;
using ReportHost.Data.Models;
using ReportHost.Data.Queries;
using ReportHost.Data.Reports;
using Microsoft.EntityFrameworkCore;
namespace ReportHost.Controllers;

[Route("api/reports")]
public class ReportsController : Controller
{
	private ReportContext _db;
	private Generator _generator;

	public ReportsController(ReportContext context, Generator generator)
	{
		_db = context;
		_generator = generator;
	}

	[HttpGet]
	public async Task<IActionResult> GetReports()
	{
		var reports = await _db.Reports.AsNoTracking().Select(r => new { Id = r.Id, Name = r.Name }).ToListAsync();
		return Ok(reports);
	}

	[HttpGet("columns/{tableName}")]
	public async Task<IActionResult> GetTableColumns(string tableName)
	{
		var columns = await _db.Tables.GetTableColumnsAsync(tableName);

		return Ok(columns);
	}

	[HttpGet("tables")]
	public async Task<IActionResult> GetTables()
	{
		var tables = await _db.Tables.GetTables().Select(r => new TableDetail() { SchemaName = r.Schema.Name, TableName = r.Name }).ToListAsync();
		return Ok(tables);
	}

	[HttpPost("results")]
	public async Task<IActionResult> GetReportResults([FromBody]Criteria criteria)
	{
		var result = await _generator.GenerateAsync(criteria);
		return Ok(result);
	}
}

