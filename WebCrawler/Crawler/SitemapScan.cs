using DBEntity.Context; //2021112204
using DBEntity.Models;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebCrawler.Classes;

namespace WebCrawler.Crawler
{
    //2021112210 - 2021112218
    internal class SitemapScan : CommonMethods, IScanner
    {
        //2021112201 - 2021112209
        private string MainUrl { get; init; }
        public string Host { get; init; }
        private int AmountOfMaxTasks { get; init; }
        //2021112228
        private ConcurrentQueue<Task> QueueOfTasks { get; init; }
        private ConcurrentQueue<string> WaitingToScan { get; set; }
        public ConcurrentBag<Task> ListOfTasks { get; init; }
        public SitemapScan(string Url, int AmountOfTasks) //2021112202
        {
            this.MainUrl = Url; //2021112212
            this.AmountOfMaxTasks = AmountOfTasks;
            this.Host = new Uri(this.MainUrl).Host;
            this.QueueOfTasks = new();
            this.ListOfTasks = new();
            this.WaitingToScan = new();
        }
        private void GetUrlsFromSourceCodeToQueue(string SourceCode)
        {
            Regex regex = new(@"(?<Protocol>\w+):\/\/(?<Domain>[\w@][\w.:@]+)\/?[\w\.?=%&=\-@/$,]*", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            IEnumerable<string> matches = regex.Matches(SourceCode).Select(p => p.Value).Distinct(); //2021112236 - 2021112226
            if (matches != null)
            {
                foreach (string match in matches)
                {
                    if (!DoesExistInQueue(match) && !DoesExistInScan(match))
                    {
                        Enqueue(new Queue() { Url = match, Host = this.Host });
                    }
                }
            }
        }
        private void InitializeQueue()
        {
            HttpResponse response = Task.Run(() => { return DownloadSourceCodeSync(this.MainUrl); }).Result;
            GetUrlsFromSourceCodeToQueue(response.SourceCode);
            var vrQueue = new CrawlerContext().Queue.Where(p => p.Host == this.Host).ToList(); //2021112226 - 2021112250
            if (vrQueue != null)
            {
                foreach (var vrNode in vrQueue)
                {
                    WaitingToScan.Enqueue(vrNode.Url);
                }
            }
        }
        public void Scanner()
        {
            InitializeQueue();
            if (WaitingToScan.Count != 0)
            {
                foreach (var vrItem in WaitingToScan)
                {
                    Task task = new(() =>
                    {
                        var vrNode = CreateScanNode(vrItem, this.MainUrl, this.Host);
                        if (vrNode != null)
                        {
                            using (CrawlerContext context = new()) //2021112230
                            {
                                using (IDbContextTransaction transaction = context.Database.BeginTransaction())
                                {
                                    try //2021112205
                                    {
                                        if (!DoesExistInScan(vrNode.Url))
                                        {
                                            context.Scan.Add(vrNode);
                                            context.SaveChanges();
                                            transaction.Commit();
                                        }
                                        Dequeue(vrNode.Url);
                                    }
                                    catch
                                    {
                                        transaction.Rollback();
                                    }
                                }
                            }
                        }
                        TaskFinished(QueueOfTasks);
                    });
                    QueueOfTasks.Enqueue(task);
                    ListOfTasks.Add(task);
                    for (int i = 0; i < GetMissingTaskAmount(this.ListOfTasks, this.AmountOfMaxTasks); i++)
                    {
                        TaskFinished(QueueOfTasks);
                    }
                }
            }
        }
        //2021112213
        public override string ToString()
        {
            return "Sitemap or RSS Scanning";
        }
    }
}
