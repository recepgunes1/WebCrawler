using DBEntity.Context;
using DBEntity.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebCrawler.Classes;
using WebCrawler.Other;

namespace WebCrawler.Crawler
{
    internal class InternalScan : CommonMethods, ICrawler
    {
        public string MainUrl { get; init; }
        public string Host { get; init; }
        public bool RecursionFlag { get; set; }
        public int DepthLevel { get; set; }

        public InternalScan(string Url)
        {
            MainUrl = Url;
            Host = new Uri(Url).Host;
            RecursionFlag = false;
            DepthLevel = 1;
        }

        public HashSet<string>? GetUrlsFromSourceCode(string Url)
        {
            var vrDocument = CreateHtmlDocument(Url);
            if(vrDocument != null)
            {
                var vrSelectedNodes = vrDocument.DocumentNode.SelectNodes("//a");
                if (vrSelectedNodes != null)
                {
                    return vrSelectedNodes.Where(p => p.IsValidUrl(Url)).Select(p => p.Attributes["href"].Value).Where(p => UrlOperations.DoTheyHaveSameHost(p, this.Host))
                        .Select(p => p.StartsWith(Path.AltDirectorySeparatorChar) ? p = $"{Url}{p}" : p)
                        .ToHashSet<string>();
                }
            }
            return null;
        }

        public void InitializeQueue()
        {
            HttpResponse response = Task.Run(() => { return DownloadSourceCodeSync(this.MainUrl); }).Result;
            var vrUrls = GetUrlsFromSourceCode(response.SourceCode);
            if(vrUrls != null)
            {
                foreach(var vrUrl in vrUrls)
                {
                    if(UrlOperations.DoTheyHaveSameHost(vrUrl, this.Host))
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
            if (RecursionFlag)
            {
                using (CrawlerContext context = new())
                {
                    var vrUrls = context.Scan.ToList().Where(p => p.Host == this.Host).Select(p => p.Url).Distinct();
                    var vrParentUrls = context.Scan.ToList().Where(p => p.Host == this.Host).Select(p => p.ParentUrl).Distinct();
                    var vrSubParentUrls = vrUrls.Where(p => !vrParentUrls.Contains(p));
                    InitializeQueue(vrSubParentUrls);
                }
            }
            else
            {
                InitializeQueue();
            }
            var Queue = new CrawlerContext().Queue.Where(p => p.Host == this.Host).Select(p => p.Url);
            if (Queue.Count() == 0 || Queue == null)
                return;
            foreach (var ParenUrl in Queue)
            {
                Dequeue(ParenUrl);
                var vrResponse = Task.Run(() => { return DownloadSourceCodeSync(ParenUrl); }).Result;
                var vrSubUrls = GetUrlsFromSourceCode(vrResponse.SourceCode);
                if(vrSubUrls != null)
                {
                    foreach (var vrSubUrl in vrSubUrls)
                    {
                        if (!DoesExistInScan(vrSubUrl))
                        {
                            using(CrawlerContext context = new())
                            {
                                context.Scan.Add(CreateScanNode(vrSubUrl, ParenUrl, this.Host, this.DepthLevel));
                                context.SaveChanges();
                            }
                        }
                    }
                }
            }
            this.DepthLevel++;
            this.RecursionFlag = true;
            Scanner();
        }
    }
}
