using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ReportHost.Data.Configuration;

public class TableMap : IEntityTypeConfiguration<Entities.Table>
{
	public void Configure(EntityTypeBuilder<Entities.Table> builder)
	{
		builder.ToTable("sys.objects");
		builder.HasKey(x => x.Id);
		builder.Property(x => x.Id).HasColumnName("object_id");
		builder.Property(x => x.IsMSShipped).HasColumnName("is_ms_shipped");
		builder.Property(x => x.ObjectType).HasColumnName("type");
		builder.Property(x => x.SchemaId).HasColumnName("schema_id");
	}
}

