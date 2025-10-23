using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;

namespace ReportHost.Data.Configuration;
public class SchemaMap : EntityTypeConfiguration<Entities.Schema>
{
	public SchemaMap()
	{
		Map(x => x.ToTable("sys.schemas"));
		HasKey(x => x.Id);
		Property(x => x.Id).HasColumnName("schema_id");
	}
}

