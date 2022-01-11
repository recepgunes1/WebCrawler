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
    internal class ExternalScan : CommonMethods//, ICrawler
    {
        public string MainUrl { get; init; }
        public string Host { get; init; }
        public bool RecursionFlag { get; set; }
        public int DepthLevel { get; set; }
        public int AmountOfMaxTasks { get; init; }
        public ConcurrentQueue<Task> Tasks { get; init; }
        public ConcurrentBag<Task> ListOfTasks { get; init; }

        public ExternalScan(string Url, int AmountOfTasks)
        {
            MainUrl = Url;
            AmountOfMaxTasks = AmountOfTasks;
            Host = new Uri(Url).Host;
            Tasks = new();
            ListOfTasks = new();
            RecursionFlag = false;
            DepthLevel = 1;

        }

        public void GetUrlsFromSourceCodeToQueue(string SourceCode, string ParentUrl)
        {
            if (!UrlOperations.DoTheyHaveSameHost(ParentUrl, this.Host))
                return;
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
                        .Select(p => p.StartsWith(Path.AltDirectorySeparatorChar) && !p.EndsWith(Path.AltDirectorySeparatorChar) ? $"{ParentUrl}{p}" : p)
                        .Select(p => p.EndsWith(Path.AltDirectorySeparatorChar) ? p.Remove(p.Length - 1) : p)
                        .Where(p => p.IsValidUrl())
                        .Distinct();
                    foreach (var vrUrl in vrFiltredUrls)
                    {
                        if (vrUrl != null)
                        {
                            Enqueue(ParentUrl, vrUrl, this.Host);
                        }
                    }
                }
            }
        }

        public void InitializeQueue()
        {
            HttpResponse response = Task.Run(() => { return DownloadSourceCodeSync(this.MainUrl); }).Result;
            System.Diagnostics.Debug.Print(this.MainUrl);
            GetUrlsFromSourceCodeToQueue(response.SourceCode, this.MainUrl);
        }

        public void InitializeQueue(IEnumerable<string> Param)
        {
            if (Param != null)
            {
                foreach (var vrUrl in Param)
                {
                    if (UrlOperations.DoTheyHaveSameHost(vrUrl, this.Host))
                    {
                        if (!DoesExistInQueue(vrUrl))
                        {
                            var vrQueue = new Queue() { Url = vrUrl, Host = this.Host };
                            Enqueue(vrQueue);
                        }
                    }
                }
            }
        }

        public void Scanner()
        {
            int irCounter = 0;
            if (RecursionFlag)
            {
                using (CrawlerContext context = new())
                {
                    var vrUrls = context.Scan.ToList().Where(p => p.DoTheyHaveSameHost(this.Host)).Select(p => p.Url).Distinct();
                    var vrParentUrls = context.Scan.ToList().Where(p => p.DoTheyHaveSameHost(this.Host)).Select(p => p.ParentUrl).Distinct();
                    var vrSubParentUrls = vrUrls.Where(p => !vrParentUrls.Contains(p));
                    InitializeQueue(vrSubParentUrls);
                }
            }
            else
            {
                InitializeQueue();
            }
            var Queue = new CrawlerContext().Queue.ToList().Where(p => p.DoTheyHaveSameHost(this.Host)).Select(p => p.Url);
            if (Queue.Count() == 0 || Queue == null)
                return;
            while (new CrawlerContext().Queue.ToList().Count(p => p.DoTheyHaveSameHost(this.Host)) > 0)
            {
                Task task = new(() =>
                {
                    Queue queueItem;
                    while (true)
                    {
                        var vrTemp = Dequeue(this.Host);
                        if (vrTemp != null)
                        {
                            queueItem = vrTemp;
                            break;
                        }
                    }
                    if (!DoesExistInScan(queueItem.Url))
                    {
                        var vrNode = CreateScanNode(queueItem.Url, queueItem.ParentUrl, this.Host, this.DepthLevel);
                        if (vrNode != null)
                        {
                            using (CrawlerContext context = new())
                            {
                                using (IDbContextTransaction transaction = context.Database.BeginTransaction())
                                {
                                    try
                                    {
                                        context.Scan.Add(vrNode);
                                        context.SaveChanges();
                                        transaction.Commit();
                                    }
                                    catch
                                    {
                                        transaction.Rollback();
                                    }
                                }
                            }
                        }
                    }
                    Task.Factory.StartNew(() =>
                    {
                        var vrResponse = Task.Run(() => { return DownloadSourceCodeSync(queueItem.Url); }).Result;
                        GetUrlsFromSourceCodeToQueue(vrResponse.SourceCode, queueItem.Url);
                    });
                    //TaskFinished();
                });
                Tasks.Enqueue(task);
                ListOfTasks.Add(task);
                var vrArray = ListOfTasks.Skip(irCounter * AmountOfMaxTasks).Take(this.GetMissingTaskAmount()).ToArray();
                for (int i = 0; i < vrArray.Length; i++)
                {
                    vrArray[i].Start();
                }
                //var vrCounter = GetMissingTaskAmount();
                //for (int i = 0; i < vrCounter; i++)
                //{
                //    if (Tasks.TryDequeue(out var vrTempTask) && vrTempTask != null)
                //    {
                //        vrTempTask.Start();
                //    }
                //}
            }
            this.DepthLevel++;
            this.RecursionFlag = true;
            Scanner();
        }

        private void TaskFinished()
        {
            if (GetMissingTaskAmount() == 0)
                return;
            if (Tasks.TryDequeue(out var vrTempTask) && vrTempTask != null)
            {
                vrTempTask.Start();
            }
        }
        private int GetMissingTaskAmount()
        {
            var vrCount = this.ListOfTasks.Count(p => p.Status == (TaskStatus)3);
            if (vrCount < this.AmountOfMaxTasks)
                return this.AmountOfMaxTasks - vrCount;
            return 0;
        }
    }
}