﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ReportHost.Data.Entities;

namespace ReportHost.Data.Context
{
	public interface IContext : IDisposable
	{

		#region DbSets
		DbSet<Report> Reports { get; set; }
		DbSet<Table> Tables { get; set; }
		DbSet<Schema> Schemas { get; set; }
		DbSet<DataType> DataTypes { get; set; }
		DbSet<Column> Columns { get; set; }
		#endregion

		Database Database { get; }
		DbContextConfiguration Configuration { get; }
		void SetModified(object entity);
		DbEntityValidationResult GetValidationResult(object entity);
		int ExecuteSqlCommand(string sql, params object[] parameters);
		IEnumerable<T> SqlQuery<T>(string sql, params object[] parameters);
		int SaveChanges();
		Task<int> SaveChangesAsync();
		Task<int> SaveChangesAsync(CancellationToken cancellationToken);
	}
}
