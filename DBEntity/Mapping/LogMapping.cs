using DBEntity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DBEntity.Mapping
{
    //2021112210
    public class LogMapping : IEntityTypeConfiguration<Log>
    {
        public void Configure(EntityTypeBuilder<Log> builder)
        {
            //Not Null
            builder.Property(p => p.ID).IsRequired();

            //Nullable
            builder.Property(p => p.Exception).IsRequired(false);
            builder.Property(p => p.ExceptionMessage).IsRequired(false);
            builder.Property(p => p.ExceptionSource).IsRequired(false);
            builder.Property(p => p.ExceptionStackTree).IsRequired(false);
            builder.Property(p => p.ExceptionHResult).IsRequired(false);
            builder.Property(p => p.ExceptionData).IsRequired(false);
            builder.Property(p => p.ExceptionTargetSite).IsRequired(false);
            builder.Property(p => p.ExceptionHelpLink).IsRequired(false);
            builder.Property(p => p.InnerException).IsRequired(false);
            builder.Property(p => p.InnerExceptionMessage).IsRequired(false);
            builder.Property(p => p.InnerExceptionSource).IsRequired(false);
            builder.Property(p => p.InnerExceptionStackTree).IsRequired(false);
            builder.Property(p => p.InnerExceptionHResult).IsRequired(false);
            builder.Property(p => p.InnerExceptionData).IsRequired(false);
            builder.Property(p => p.InnerExceptionTargetSite).IsRequired(false);
            builder.Property(p => p.InnerExceptionHelpLink).IsRequired(false);

            //Default Value
            builder.Property(p => p.OccurredDate).ValueGeneratedOnAdd();
            builder.Property(p => p.OccurredDate).HasDefaultValueSql("GETDATE()");

            //Primary Key
            builder.HasKey(p => p.ID);

            //Table Settings
            builder.ToTable("Logging");
        }
    }
}
