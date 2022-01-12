using DBEntity.Context;
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
    internal class SitemapScan : CommonMethods, IScanner
    {
        private string MainUrl { get; init; }
        public string Host { get; init; }
        private int AmountOfMaxTasks { get; init; }
        private ConcurrentQueue<Task> QueueOfTasks { get; init; }
        private ConcurrentQueue<string> WaitingToScan { get; set; }
        public ConcurrentBag<Task> ListOfTasks { get; init; }
        public SitemapScan(string Url, int AmountOfTasks)
        {
            this.MainUrl = Url;
            this.AmountOfMaxTasks = AmountOfTasks;
            this.Host = new Uri(this.MainUrl).Host;
            this.QueueOfTasks = new();
            this.ListOfTasks = new();
            this.WaitingToScan = new();
        }
        private void GetUrlsFromSourceCodeToQueue(string SourceCode)
        {
            Regex regex = new(@"(?<Protocol>\w+):\/\/(?<Domain>[\w@][\w.:@]+)\/?[\w\.?=%&=\-@/$,]*", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            IEnumerable<string> matches = regex.Matches(SourceCode).Select(p => p.Value).Distinct();
            if (matches != null)
            {
                foreach (string match in matches)
                {
                    if (!DoesExistInQueue(match) && !DoesExistInScan(match))
                    {
                        Enqueue(new Queue() { Url = match, Host = this.Host });
                        System.Diagnostics.Debug.Print($"{match} was added queue table.");
                    }
                }
            }
        }
        private void InitializeQueue()
        {
            HttpResponse response = Task.Run(() => { return DownloadSourceCodeSync(this.MainUrl); }).Result;
            GetUrlsFromSourceCodeToQueue(response.SourceCode);
            var vrQueue = new CrawlerContext().Queue.Where(p => p.Host == this.Host).ToList();
            if (vrQueue != null)
            {
                foreach (var vrNode in vrQueue)
                {
                    WaitingToScan.Enqueue(vrNode.Url);
                    System.Diagnostics.Debug.Print($"{ vrNode.Url} was added queue on ram.");
                }
            }
        }

        public void Scanner()
        {
            InitializeQueue();
            foreach (var vrItem in WaitingToScan)
            {
                Task task = new(() =>
                {
                    System.Diagnostics.Debug.Print($"Scan was started for {vrItem}.");
                    var vrNode = CreateScanNode(vrItem, this.MainUrl, this.Host);
                    if (vrNode != null)
                    {
                        using (CrawlerContext context = new())
                        {
                            using (IDbContextTransaction transaction = context.Database.BeginTransaction())
                            {
                                try
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
                    System.Diagnostics.Debug.Print($"Scan was done for {vrItem}.");
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
}
