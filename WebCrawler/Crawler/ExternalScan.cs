using DBEntity.Context; //2021112204
using DBEntity.Models;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebCrawler.Classes;
using WebCrawler.Other;

namespace WebCrawler.Crawler
{
    //2021112210 - 2021112218
    internal class ExternalScan : CommonMethods, IScanner
    {
        //2021112201 - 2021112209
        private string MainUrl { get; init; }
        public string Host { get; init; }
        private int AmountOfMaxTasks { get; init; }
        //2021112228
        private ConcurrentQueue<Task> QueueOfTasks { get; init; }
        private ConcurrentQueue<string> WaitingToScan { get; set; }
        public ConcurrentBag<Task> ListOfTasks { get; init; }
        public ExternalScan(string Url, int AmountOfTasks) //2021112202
        {
            MainUrl = Url;
            AmountOfMaxTasks = AmountOfTasks;
            Host = new Uri(Url).Host;
            QueueOfTasks = new();
            ListOfTasks = new();
            WaitingToScan = new();
        }

        private void GetUrlsFromSourceCodeToQueue(string SourceCode, string ParentUrl)
        {
            var vrDocument = CreateHtmlDocument(SourceCode);
            if (vrDocument != null)
            {
                var vrSelectedNodes = vrDocument.DocumentNode.SelectNodes("//a");
                if (vrSelectedNodes != null)
                {
                    IEnumerable<string>? FiltredUrls = vrSelectedNodes
                        .Where(p => p != null)
                        .Select(p => p.Attributes["href"]?.Value)
                        .Where(p => !string.IsNullOrEmpty(p))
                        .Select(p => p.StartsWith(Path.AltDirectorySeparatorChar) && !p.StartsWith($"{Path.AltDirectorySeparatorChar}{Path.AltDirectorySeparatorChar}") ? $"{ParentUrl}{p}" : p)
                        .Select(p => p.EndsWith(Path.AltDirectorySeparatorChar) ? p.Remove(p.Length - 1) : p)
                        .Where(p => p.IsValidUrl())
                        .Distinct(); //2021112236 - 2021112226
                    if (FiltredUrls != null)
                    {
                        foreach (var vrUrl in FiltredUrls)
                        {
                            if (vrUrl != null)
                            {
                                if (!DoesExistInQueue(vrUrl) && !DoesExistInScan(vrUrl))
                                {
                                    Enqueue(new Queue() { Url = vrUrl, Host = this.Host });
                                }
                            }
                        }
                    }
                }
            }
        }

        private void InitializeQueue()
        {
            HttpResponse response = Task.Run(() => { return DownloadSourceCodeSync(this.MainUrl); }).Result;
            GetUrlsFromSourceCodeToQueue(response.SourceCode, this.MainUrl);
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
            if (this.WaitingToScan.Count != 0)
            {
                foreach (var vrItem in this.WaitingToScan)
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
            return "External Scan";
        }
    }
}