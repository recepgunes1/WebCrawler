using DBEntity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DBEntity.Mapping
{
    public class QueueMapping : IEntityTypeConfiguration<Queue>
    {
        public void Configure(EntityTypeBuilder<Queue> builder)
        {
            //Primary Key
            builder.HasKey(p => p.ID);
            builder.HasIndex(p => p.Url).IsUnique();

            //Not Null
            builder.Property(p => p.ID).IsRequired();
            builder.Property(p => p.ParentUrl).IsRequired();
            builder.Property(p => p.Url).IsRequired();
            builder.Property(p => p.Host).IsRequired();

            //Table Settings
            builder.ToTable("tblQueue");
        }
    }
}
