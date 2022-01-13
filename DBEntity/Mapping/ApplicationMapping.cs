using DBEntity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DBEntity.Mapping
{
    //2021112210
    public class ApplicationMapping : IEntityTypeConfiguration<Application>
    {
        public void Configure(EntityTypeBuilder<Application> builder)
        {
            //Not Null
            builder.Property(p => p.ID).IsRequired();
            builder.Property(p => p.Backup).IsRequired();
            builder.Property(p => p.CreationDate).IsRequired();

            //Default Value
            builder.Property(p => p.CreationDate).ValueGeneratedOnAdd();
            builder.Property(p => p.CreationDate).HasDefaultValueSql("GETDATE()");

            //Primary Key
            builder.HasKey(p => p.ID);

            //Table Settings
            builder.ToTable("ApplicationBackup");
        }
    }
}
