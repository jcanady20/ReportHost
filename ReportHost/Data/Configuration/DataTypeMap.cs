using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ReportHost.Data.Configuration;

public class DataTypeMap : IEntityTypeConfiguration<Entities.DataType>
{
	public void Configure(EntityTypeBuilder<Entities.DataType> builder)
	{
		builder.ToTable("sys.types");
		builder.HasKey(x => x.Id);
		builder.Property(x => x.Id).HasColumnName("system_type_id");
	}
}

