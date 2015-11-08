using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;

namespace ReportHost.Data.Configuration
{
	public class TableMap : EntityTypeConfiguration<Entities.Table>
	{
		public TableMap()
		{
			Map(x => x.ToTable("sys.objects"));
			HasKey(x => x.Id);
			Property(x => x.Id).HasColumnName("object_id");
			Property(x => x.IsMSShipped).HasColumnName("is_ms_shipped");
			Property(x => x.ObjectType).HasColumnName("type");
			Property(x => x.SchemaId).HasColumnName("schema_id");
		}
	}
}
