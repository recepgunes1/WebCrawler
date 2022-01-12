using DBEntity.Context;
using DBEntity.Models;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebCrawler.Classes;
using WebCrawler.Other;

namespace WebCrawler.Crawler
{
    internal class CommonMethods
    {
        protected async Task<HttpResponse> DownloadSourceCodeSync(string Target)
        {
            Stopwatch stopwatch = new();
            HttpResponse httpResponse = new();
            stopwatch.Start();
            try
            {
                using (HttpClient client = new())
                {
                    using (HttpResponseMessage response = await client.GetAsync(Target))
                    {
                        using (Stream stream = await response.Content.ReadAsStreamAsync())
                        {
                            using (StreamReader reader = new StreamReader(stream))
                            {
                                httpResponse.SourceCode = reader.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch
            {
                httpResponse.SourceCode = string.Empty;
            }
            stopwatch.Stop();
            httpResponse.FetchTimeMS = stopwatch.ElapsedMilliseconds;
            return httpResponse;
        }

        protected HtmlDocument? CreateHtmlDocument(string SourceCode)
        {
            HtmlDocument document = new HtmlDocument();
            if (!string.IsNullOrEmpty(SourceCode))
            {
                document.LoadHtml(SourceCode);
                return document;
            }
            return null;
        }

        protected Scan? CreateScanNode(string Url, string Parent, string Host)
        {
            Scan? scan = new();
            HttpResponse response = Task.Run(() => { return DownloadSourceCodeSync(Url); }).Result;
            HtmlDocument? document = CreateHtmlDocument(response.SourceCode);
            if (document != null)
            {
                scan.UrlHash = Url.EncryptSHA256();
                scan.Url = Url;
                scan.ParentUrl = Parent;
                scan.Host = Host;
                scan.Title = document.DocumentNode.SelectSingleNode("/html/head/title")?.InnerText == null ? string.Empty : document.DocumentNode.SelectSingleNode("/html/head/title").InnerText;
                scan.CompressedInnerText = document.DocumentNode.SelectSingleNode("/html")?.InnerText == null ? string.Empty.Zip() : document.DocumentNode.SelectSingleNode("/html").InnerText.Zip();
                scan.CompressedSourceCode = document.DocumentNode.SelectSingleNode("/html")?.InnerHtml == null ? string.Empty.Zip() : document.DocumentNode.SelectSingleNode("/html").InnerHtml.Zip();
                scan.FetchTimeMS = response.FetchTimeMS;
                return scan;
            }
            return null;
        }

        protected bool DoesExistInQueue(string Url)
        {
            using (CrawlerContext context = new())
            {
                return context.Queue.Where(p => p.Url == Url).FirstOrDefault() != null;
            }
        }

        protected bool DoesExistInScan(string Url)
        {
            using (CrawlerContext context = new())
            {
                return context.Scan.Where(p => p.UrlHash == Url.EncryptSHA256()).FirstOrDefault() != null;
            }
        }

        protected void Dequeue(string Url)
        {
            using (CrawlerContext context = new())
            {
                using (IDbContextTransaction transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var queue = context.Queue.Where(p => p.Url == Url).FirstOrDefault();
                        if (queue != null)
                        {
                            if (DoesExistInQueue(queue.Url))
                            {
                                context.Remove(queue);
                                context.SaveChanges();
                                transaction.Commit();
                            }
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                    }
                }
            }
        }

        protected void Enqueue(Queue item)
        {
            using (CrawlerContext context = new())
            {
                using (IDbContextTransaction transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        if (!DoesExistInQueue(item.Url))
                        {
                            context.Queue.Add(item);
                            context.SaveChanges();
                            transaction.Commit();
                        }
                    }
                    catch
                    {
                        transaction.Rollback();
                    }
                }
            }
        }

        protected int GetMissingTaskAmount(ConcurrentBag<Task> tasks, int AmoutOfTasks)
        {
            var vrCount = tasks.Count(p => p.Status == TaskStatus.Running);
            if (AmoutOfTasks > vrCount)
                return AmoutOfTasks - vrCount;
            return 0;
        }

        protected void TaskFinished(ConcurrentQueue<Task> Tasks)
        {
            if (Tasks.TryDequeue(out var vrTempTask) && vrTempTask != null)
            {
                vrTempTask.Start();
            }
        }

    }
}
