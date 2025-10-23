using System.Data.Entity;
using ReportHost.Data.Entities;

namespace ReportHost.Data.Context;

public class ReportContext : DbContext
{
	public ReportContext() : base() { }
	public ReportContext(string nameOrConnectionString) : base(nameOrConnectionString) { }

	public DbSet<Report> Reports { get; set; }
	public DbSet<Table> Tables { get; set; }
	public DbSet<Schema> Schemas { get; set; }
	public DbSet<DataType> DataTypes { get; set; }
	public DbSet<Column> Columns { get; set; }

	protected override void OnModelCreating(DbModelBuilder modelBuilder)
	{
		modelBuilder.Configurations.AddFromAssembly(typeof(ReportContext).Assembly);
		base.OnModelCreating(modelBuilder);
	}
}
