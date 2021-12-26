using DBEntity.Mapping;
using DBEntity.Models;
using Microsoft.EntityFrameworkCore;

namespace DBEntity.Context
{
    public class CrawlerContext : DbContext
    {
        public DbSet<Application> Application { get; set; }
        public DbSet<Queue> Queue { get; set; }
        public DbSet<Scan> Scan { get; set; }
        public CrawlerContext() : base()
        {
            Database.EnsureCreatedAsync();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(LocalDB)\\MSSQLLocalDB;Database=dbWebCrawler;Integrated Security=true");
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ApplicationMapping());
            modelBuilder.ApplyConfiguration(new QueueMapping());
            modelBuilder.ApplyConfiguration(new ScanMapping());
            base.OnModelCreating(modelBuilder);
        }
    }
}
