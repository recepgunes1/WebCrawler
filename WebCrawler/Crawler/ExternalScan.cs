using DBEntity.Context;
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
    internal class ExternalScan : CommonMethods, IScanner
    {
        private string MainUrl { get; init; }
        public string Host { get; init; }
        private int AmountOfMaxTasks { get; init; }
        private ConcurrentQueue<Task> QueueOfTasks { get; init; }
        private ConcurrentQueue<string> WaitingToScan { get; set; }
        public ConcurrentBag<Task> ListOfTasks { get; init; }
        public ExternalScan(string Url, int AmountOfTasks)
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
                    var vrFiltredUrls = vrSelectedNodes
                        .Where(p => p != null)
                        .Select(p => p.Attributes["href"]?.Value)
                        .Where(p => !string.IsNullOrEmpty(p))
                        .Select(p => p.StartsWith(Path.AltDirectorySeparatorChar)
                                && !p.StartsWith($"{Path.AltDirectorySeparatorChar}{Path.AltDirectorySeparatorChar}") ? $"{ParentUrl}{p}" : p)
                        .Select(p => p.EndsWith(Path.AltDirectorySeparatorChar) ? p.Remove(p.Length - 1) : p)
                        .Where(p => p.IsValidUrl())
                        .Distinct();
                    foreach (var vrUrl in vrFiltredUrls)
                    {
                        if (vrUrl != null)
                        {
                            if (!DoesExistInQueue(vrUrl) && !DoesExistInScan(vrUrl))
                            {
                                Enqueue(new Queue() { Url = vrUrl, Host = this.Host });
                                System.Diagnostics.Debug.Print($"{vrUrl} was added queue table.");
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
            var vrQueue = new CrawlerContext().Queue.Where(p => p.Host == this.Host).ToList();

            if (vrQueue != null)
            {
                foreach (var vrNode in vrQueue)
                {
                    WaitingToScan.Enqueue(vrNode.Url);
                    System.Diagnostics.Debug.Print($"{vrNode.Url} was added queue on ram.");
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
        public override string ToString()
        {
            return "External Scan";
        }
    }
}