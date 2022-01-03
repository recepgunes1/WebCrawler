using DBEntity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DBEntity.Mapping
{
    public class ScanMapping : IEntityTypeConfiguration<Scan>
    {
        public void Configure(EntityTypeBuilder<Scan> builder)
        {
            //Primary Key
            builder.HasKey(p => p.UrlHash);

            //Not Null
            builder.Property(p => p.UrlHash).IsRequired();
            builder.Property(p => p.Url).IsRequired();
            builder.Property(p => p.Host).IsRequired();
            builder.Property(p => p.DiscoveryDate).IsRequired();
            builder.Property(p => p.FetchTimeMS).IsRequired();

            //Can be Null
            builder.Property(p => p.ParentUrl).IsRequired(false);
            builder.Property(p => p.Title).IsRequired(false);
            builder.Property(p => p.CompressedSourceCode).IsRequired(false);
            builder.Property(p => p.CompressedInnerText).IsRequired(false);

            //Default Value
            builder.Property(p => p.DiscoveryDate).HasDefaultValueSql("GETDATE()");

            //Max Length
            builder.Property(p => p.UrlHash).HasMaxLength(64);

            //Table Settings
            builder.ToTable("tblScan");
        }
    }
}
