using DBEntity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DBEntity.Mapping
{
    public class QueueMapping : IEntityTypeConfiguration<Queue>
    {
        public void Configure(EntityTypeBuilder<Queue> builder)
        {
            //Not Null
            builder.Property(p => p.Url).IsRequired();
            builder.Property(p => p.Host).IsRequired();

            //Index
            builder.HasIndex(p => p.Url).IsUnique();

            //Table Settings
            builder.ToTable("tblQueue");
            builder.HasNoKey();
        }
    }
}
