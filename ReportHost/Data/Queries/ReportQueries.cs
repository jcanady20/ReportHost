using System.Data.Entity;

using ReportHost.Data.Context;
using ReportHost.Data.Entities;

namespace ReportHost.Data.Queries;

public static class ReportQueries
{
	public static IQueryable<Report> GetReports(this ReportContext db)
	{
		var qry = db.Reports.AsQueryable();

		return qry;
	}

	public static Report GetReportById(this ReportContext db, int reportId)
	{
		var qry = db.GetReports();
		var report = qry.FirstOrDefault(x => x.Id == reportId);

		return report;
	}

	public static async Task<Report> GetReportByIdAsync(this ReportContext db, int reportId)
	{
		var qry = db.GetReports();
		var report = await qry.FirstOrDefaultAsync(x => x.Id == reportId);

		return report;
	}
}

