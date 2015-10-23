using System;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;

namespace ReportHost.Data.Configuration
{
	public class DataTypeMap : EntityTypeConfiguration<Entities.DataType>
	{
		public DataTypeMap()
		{
			Map(x => x.ToTable("sys.types"));
			HasKey(x => x.Id);
			Property(x => x.Id).HasColumnName("system_type_id");
		}
	}
}
