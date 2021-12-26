using DBEntity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DBEntity.Mapping
{
    public class ScanMapping : IEntityTypeConfiguration<Scan>
    {
        public void Configure(EntityTypeBuilder<Scan> builder)
        {
            //Not Null
            builder.Property(p => p.UrlHash).IsRequired();
            builder.Property(p => p.Url).IsRequired();
            builder.Property(p => p.FetchTimeMS).IsRequired();

            //Can be Null
            builder.Property(p => p.ParentUrl).IsRequired(false);
            builder.Property(p => p.Title).IsRequired(false);
            builder.Property(p => p.CompressedSourceCode).IsRequired(false);
            builder.Property(p => p.CompressedInnerText).IsRequired(false);

            //Max Length
            builder.Property(p => p.UrlHash).HasMaxLength(64);

            //Index
            builder.HasIndex(p => p.UrlHash).IsUnique();

            //Table Settings
            builder.ToTable("tblScan");
            builder.HasNoKey();
        }
    }
}
