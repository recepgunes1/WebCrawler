using DBEntity.Mapping;
using DBEntity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;

namespace DBEntity.Context
{
    //2021112210
    public class CrawlerContext : DbContext
    {
#pragma warning disable CS8618
#pragma warning disable CS8600
#pragma warning disable CA1416
#pragma warning disable CS8602

        public DbSet<Application> Application { get; set; } //2021112201
        public DbSet<Queue> Queue { get; set; } //2021112201
        public DbSet<Scan> Scan { get; set; } //2021112201

        //2021112202 - 2021112212
        public CrawlerContext() : base()
        {
            Database.SetCommandTimeout(int.MaxValue);
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey("WebCrawler");
            string DatabaseProiver = (string)registryKey.GetValue("Database Provider");
            string ConnectionString = (string)registryKey.GetValue("Connection String");
            if (!string.IsNullOrEmpty(DatabaseProiver) && !string.IsNullOrEmpty(ConnectionString)) //2021112239
            {
                switch (DatabaseProiver)
                {
                    case "MsSQL":
                        optionsBuilder.UseSqlServer(ConnectionString, p => p.MaxBatchSize(1));
                        break;
                    case "SQLite":
                        optionsBuilder.UseSqlite(ConnectionString, p => p.MaxBatchSize(1));
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
            modelBuilder.UseCollation("Turkish_CI_AI");
            base.OnModelCreating(modelBuilder);
        }
    }
}
