using DBEntity.Context; //2021112204
using DBEntity.Models;
using Newtonsoft.Json;
using System.Linq;
using WebCrawler.Classes;

namespace WebCrawler.Other
{
    internal class BackupOperation
    {
        public void SaveIt(Backup backup)
        {
            var vrJson = JsonConvert.SerializeObject(backup, Formatting.Indented); //2021112232
            using (CrawlerContext context = new()) //2021112230
            {
                Application application = new() { Backup = vrJson };
                context.Application.Add(application);
                context.SaveChanges();
            }
        }

        public Backup? GetBackup()
        {
            using (CrawlerContext context = new()) //2021112230
            {
                Application? application = context.Application.ToList().OrderByDescending(p => p.CreationDate).FirstOrDefault(); //2021112226
                if (application != null)
                {
                    Backup? backup = JsonConvert.DeserializeObject<Backup>(application.Backup); //2021112232
                    if (backup != null)
                    {
                        return backup;
                    }
                }
            }
            return null;
        }
    }
}
