using DBEntity.Context;
using DBEntity.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.Classes;

namespace WebCrawler.Other
{
    internal class Crawler
    {
        private string _Url;
        private string Url { get { return _Url; } set { _Url = value; } }
        public string Host { get; init; }
        private bool blFlag { get; set; }
        public Crawler(string Target)
        {
            Url = Target;
            Host = new Uri(Url).Host;
            blFlag = false;
        }
        private async Task<HttpResponse> DownloadSourceCodeSync(string Target)
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
        private HtmlDocument? CreateHtmlDocument(string SourceCode)
        {
            HtmlDocument document = new HtmlDocument();
            if (!string.IsNullOrEmpty(SourceCode))
            {
                document.LoadHtml(SourceCode);
                return document;
            }
            return null;
        }
        public HashSet<string>? GetUrlsFromSourceCode(string Target)
        {
            var vrDocument = CreateHtmlDocument(Target);
            if (vrDocument != null)
            {
                var vrSelectedNodes = vrDocument.DocumentNode.SelectNodes("//a");
                if (vrSelectedNodes != null)
                {
                    return vrSelectedNodes.Where(p => p.IsValidUrl(this.Url)).Select(p => p.Attributes["href"].Value).
                        Select(p => p.StartsWith(Path.AltDirectorySeparatorChar) ? p = $"{this.Url}{p}" : p).ToHashSet<string>();
                }
            }
            return null;
        }
        private Scan CreateScanNode(string Target, string Parent)
        {
            Scan scan = new Scan();
            HttpResponse response = Task.Run(() => { return DownloadSourceCodeSync(Target); }).Result;
            System.Diagnostics.Debug.WriteLine($"Worked for Url: {Target} Parent Url: {Parent}");
            HtmlDocument? document = CreateHtmlDocument(response.SourceCode);
            if (document != null)
            {
                scan.UrlHash = Target.EncryptSHA256();
                scan.Url = Target;
                scan.ParentUrl = Parent;
                scan.Host = this.Host;
                scan.Title = document.DocumentNode.SelectSingleNode("/html/head/title")?.InnerText == null ? string.Empty : document.DocumentNode.SelectSingleNode("/html/head/title").InnerText;
                scan.CompressedInnerText = document.DocumentNode.SelectSingleNode("/html")?.InnerText == null ? string.Empty : Encoding.Default.GetString(document.DocumentNode.SelectSingleNode("/html").InnerText.Zip());
                scan.CompressedSourceCode = document.DocumentNode.SelectSingleNode("/html")?.InnerHtml == null ? string.Empty : Encoding.Default.GetString(document.DocumentNode.SelectSingleNode("/html").InnerHtml.Zip());
                scan.FetchTimeMS = response.FetchTimeMS;
            }
            return scan;
        }
        public void InitializeInternalQueue()
        {
            HttpResponse response = Task.Run(() => { return DownloadSourceCodeSync(this.Url); }).Result;
            var vrUrls = GetUrlsFromSourceCode(response.SourceCode);
            if (vrUrls != null)
            {
                foreach (var vrUrl in vrUrls)
                {
                    System.Diagnostics.Debug.Print($"vrUrl: {vrUrl}\t\tthis.Host: {this.Host}\t\tFunction Result: {UrlOperations.DoTheyHaveSameHost(vrUrl, this.Host)}");
                    if (UrlOperations.DoTheyHaveSameHost(vrUrl, this.Host))
                    {
                        using (CrawlerContext context = new())
                        {
                            using (var vrTransaction = context.Database.BeginTransaction())
                            {
                                try
                                {
                                    Queue queue = new() { Url = vrUrl, Host = this.Host };
                                    context.Queue.Add(queue);
                                    context.SaveChanges();
                                    vrTransaction.Commit();
                                }
                                catch
                                {
                                    vrTransaction.Rollback();
                                }
                            }
                        }
                    }
                }
            }
        }
        public void InitializeInternalQueue(IEnumerable<string> Urls)
        {
            if (Urls != null)
            {
                foreach (var vrUrl in Urls)
                {
                    if (UrlOperations.DoTheyHaveSameHost(vrUrl, this.Host))
                    {
                        using (CrawlerContext context = new())
                        {
                            using (var vrTransaction = context.Database.BeginTransaction())
                            {
                                try
                                {
                                    Queue queue = new() { Url = vrUrl, Host = this.Host };
                                    context.Queue.Add(queue);
                                    context.SaveChanges();
                                    vrTransaction.Commit();
                                }
                                catch
                                {
                                    vrTransaction.Rollback();
                                }
                            }
                        }
                    }
                }
            }
        }
        public void Enqueue(string _Url, string _Host)
        {
            using (CrawlerContext context = new())
            {
                Queue queue = new() { Url = _Url, Host = _Host };
                context.Queue.Add(queue);
                context.SaveChanges();
            }
        }
        private void Dequeue(Queue item)
        {
            using (CrawlerContext context = new())
            {
                context.Queue.Remove(item);
                context.SaveChanges();
            }
        }
        public void InternalScan()
        {
            if (blFlag)
            {
                using (CrawlerContext context = new())
                {
                    var vrUrls = context.Scan.ToList().Where(p => UrlOperations.DoTheyHaveSameHost(p.Url, this.Host)).Select(p => p.Url).ToHashSet();
                    var vrParentUrls = context.Scan.ToList().Where(p => UrlOperations.DoTheyHaveSameHost(p.Url, this.Host)).Select(p => p.ParentUrl).ToHashSet();
                    var vrSubParentUrls = vrUrls.Where(p => !vrParentUrls.Contains(p)).ToHashSet();
                    InitializeInternalQueue(vrSubParentUrls);
                }
            }
            else
            {
                InitializeInternalQueue();
            }
            var Queue = new CrawlerContext().Queue.ToList().Where(p => UrlOperations.DoTheyHaveSameHost(p.Url, this.Host)).ToList();
            if (Queue.Count == 0)
                return;
            foreach (var item in Queue)
            {
                Dequeue(item);
                var vrResponse = Task.Run(() => { return DownloadSourceCodeSync(item.Url); }).Result;
                var vrSubUrls = GetUrlsFromSourceCode(vrResponse.SourceCode);
                if (vrSubUrls != null)
                {
                    foreach (var vrSubUrl in vrSubUrls)
                    {
                        if (!UrlOperations.DoTheyHaveSameHost(vrSubUrl, this.Host))
                            continue;
                        using (CrawlerContext context = new())
                        {
                            using (var vrTransaction = context.Database.BeginTransaction())
                            {
                                if (context.Scan.ToHashSet().Where(p => UrlOperations.DoTheyHaveSameHost(p.Url, this.Host)).Select(p => p.Url).Contains(vrSubUrl))
                                    continue;
                                try
                                {
                                    context.Scan.Add(CreateScanNode(vrSubUrl, item.Url));
                                    context.SaveChanges();
                                    vrTransaction.Commit();
                                }
                                catch
                                {
                                    vrTransaction.Rollback();
                                }
                            }
                        }
                    }
                }
            }
            blFlag = true;
            InternalScan();
        }
    }
}
