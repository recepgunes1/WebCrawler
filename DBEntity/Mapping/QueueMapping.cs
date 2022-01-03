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
            builder.HasKey(p => p.Url);

            //Not Null
            builder.Property(p => p.Url).IsRequired();
            builder.Property(p => p.Host).IsRequired();

            //Table Settings
            builder.ToTable("tblQueue");
        }
    }
}
