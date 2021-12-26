using DBEntity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DBEntity.Mapping
{
    public class ApplicationMapping : IEntityTypeConfiguration<Application>
    {
        public void Configure(EntityTypeBuilder<Application> builder)
        {
            //Not Null
            builder.Property(p => p.ID).IsRequired();
            builder.Property(p => p.Backup).IsRequired();
            builder.Property(p => p.CreatecreationDate).IsRequired();
            builder.Property(p => p.Explanation).IsRequired();

            //Default Value
            builder.Property(p => p.CreatecreationDate).ValueGeneratedOnAdd();
            builder.Property(p => p.CreatecreationDate).HasDefaultValueSql("GETDATE()");

            //Max Length
            builder.Property(p => p.Explanation).HasMaxLength(1024);

            //Primary Key
            builder.HasKey(p => p.ID);

            //Table Settings
            builder.ToTable("tblApplication");
        }
    }
}
