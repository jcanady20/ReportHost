using System.Data.Entity.ModelConfiguration;

namespace ReportHost.Data.Configuration;

public class ColumnMap : EntityTypeConfiguration<Entities.Column>
{
	public ColumnMap()
	{
		Map(x => x.ToTable("sys.columns"));
		HasKey(x => new { x.TableId, x.Name });
		Property(x => x.TableId).HasColumnName("object_id");
		Property(x => x.DataTypeId).HasColumnName("system_type_id");
		Property(x => x.Ordinal).HasColumnName("column_id");
		Property(x => x.IsNullable).HasColumnName("is_nullable");
		Property(x => x.IsIdentity).HasColumnName("is_identity");
	}
}

