using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ReportHost.Data.Configuration;

public class ColumnMap : IEntityTypeConfiguration<Entities.Column>
{
	public void Configure(EntityTypeBuilder<Entities.Column> builder)
	{
		builder.ToTable("sys.columns");
		builder.HasKey(x => new { x.TableId, x.Name });
		builder.Property(x => x.TableId).HasColumnName("object_id");
		builder.Property(x => x.DataTypeId).HasColumnName("system_type_id");
		builder.Property(x => x.Ordinal).HasColumnName("column_id");
		builder.Property(x => x.IsNullable).HasColumnName("is_nullable");
		builder.Property(x => x.IsIdentity).HasColumnName("is_identity");
	}
}

