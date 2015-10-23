using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Validation;

using ReportHost.Data.Entities;

namespace ReportHost.Data.Context
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
	public class Context : DbContext, IContext
	{

		public Context() : base() { }
		public Context(string nameOrConnectionString) : base(nameOrConnectionString) { }

		#region DbSets
		public DbSet<Report> Reports { get; set; }
		public DbSet<Table> Tables { get; set; }
		public DbSet<Schema> Schemas { get; set; }
		public DbSet<DataType> DataTypes { get; set; }
		public DbSet<Column> Columns { get; set; }
		#endregion

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			modelBuilder.Configurations.AddFromAssembly(typeof(Context).Assembly);
			base.OnModelCreating(modelBuilder);
		}
		public void SetModified(object entity)
		{
			Entry(entity).State = EntityState.Modified;
		}
		public DbEntityValidationResult GetValidationResult(object entity)
		{
			return Entry(entity).GetValidationResult();
		}
		public int ExecuteSqlCommand(string sql, params object[] parameters)
		{
			return this.Database.ExecuteSqlCommand(sql, parameters);
		}
		public IEnumerable<T> SqlQuery<T>(string sql, params object[] parameters)
		{
			return this.Database.SqlQuery<T>(sql, parameters);
		}
		internal static IContext CreateContext()
		{
			return new Context("SqlServer");
		}
	}
}
