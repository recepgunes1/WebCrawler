using DBEntity.Context;
using DBEntity.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.Classes;

namespace WebCrawler.Other
{
    internal class BackupOperation
    {
        public void SaveIt(Backup backup)
        {
            var vrJson =  JsonConvert.SerializeObject(backup, Formatting.Indented);
            using(CrawlerContext context = new())
            {
                Application application = new() { Backup = vrJson};
                context.Application.Add(application);
                context.SaveChanges();
            }
        }

        public Backup? GetBackup()
        {
            using (CrawlerContext context = new())
            {
                Application? application =  context.Application.ToList().OrderByDescending(p => p.CreationDate).FirstOrDefault();
                if(application != null)
                {
                    Backup? backup = JsonConvert.DeserializeObject<Backup>(application.Backup);
                    if(backup != null)
                    {
                        return backup;
                    }
                }
            }
            return null;
        }
    }
}
