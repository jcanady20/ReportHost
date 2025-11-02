using Microsoft.EntityFrameworkCore;
using ReportHost.Data.Entities;

namespace ReportHost.Data.Context;

public class ReportContext : DbContext
{
	public ReportContext(DbContextOptions options) : base(options)
	{ }

	public DbSet<Report> Reports { get; set; }
	public DbSet<Table> Tables { get; set; }
	public DbSet<Schema> Schemas { get; set; }
	public DbSet<DataType> DataTypes { get; set; }
	public DbSet<Column> Columns { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
        base.OnConfiguring(optionsBuilder);
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.ApplyConfigurationsFromAssembly(typeof(ReportContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
