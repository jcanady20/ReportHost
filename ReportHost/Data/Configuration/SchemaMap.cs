using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ReportHost.Data.Configuration;
public class SchemaMap : IEntityTypeConfiguration<Entities.Schema>
{
	public void Configure(EntityTypeBuilder<Entities.Schema> builder)
	{
		builder.ToTable("sys.schemas");
		builder.HasKey(x => x.Id);
		builder.Property(x => x.Id).HasColumnName("schema_id");
	}
}
