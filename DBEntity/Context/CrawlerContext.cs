using DBEntity.Mapping;
using DBEntity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;

namespace DBEntity.Context
{
    public class CrawlerContext : DbContext
    {
#pragma warning disable CS8618
#pragma warning disable CS8600
#pragma warning disable CA1416
#pragma warning disable CS8602
        public DbSet<Application> Application { get; set; }
        public DbSet<Queue> Queue { get; set; }
        public DbSet<Scan> Scan { get; set; }
        public CrawlerContext() : base()
        {
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("WebCrawler");
            string DatabaseProiver = (string)registryKey.GetValue("Database Provider");
            string ConnectionString = (string)registryKey.GetValue("Connection String");
            if (!string.IsNullOrEmpty(DatabaseProiver) && !string.IsNullOrEmpty(ConnectionString))
            {
                switch (DatabaseProiver)
                {
                    case "MsSQL":
                        optionsBuilder.UseSqlServer(ConnectionString);
                        break;
                    case "SQLite":
                        optionsBuilder.UseSqlite(ConnectionString);
                        break;
                    case "In-Memory":
                        optionsBuilder.UseInMemoryDatabase(ConnectionString);
                        break;
                }
                base.OnConfiguring(optionsBuilder);
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ApplicationMapping());
            modelBuilder.ApplyConfiguration(new QueueMapping());
            modelBuilder.ApplyConfiguration(new ScanMapping());
            modelBuilder.UseCollation("Turkish_CI_AS");
            base.OnModelCreating(modelBuilder);
        }
    }
}
