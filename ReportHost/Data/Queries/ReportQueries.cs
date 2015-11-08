using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Threading.Tasks;

using ReportHost.Data.Context;
using ReportHost.Data.Entities;

namespace ReportHost.Data.Queries
{
	public static class ReportQueries
	{
		public static IQueryable<Report> GetReports(this IContext db)
		{
			var qry = db.Reports.AsQueryable();

			return qry;
		}

		public static Report GetReportById(this IContext db, int reportId)
		{
			var qry = db.GetReports();
			var report = qry.FirstOrDefault(x => x.Id == reportId);

			return report;
		}

		public static async Task<Report> GetReportByIdAsync(this IContext db, int reportId)
		{
			var qry = db.GetReports();
			var report = await qry.FirstOrDefaultAsync(x => x.Id == reportId);

			return report;
		}
	}
}
