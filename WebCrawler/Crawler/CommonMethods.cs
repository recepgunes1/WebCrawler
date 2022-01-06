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
                System.Diagnostics.Debug.Print(Target);
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

        protected Scan CreateScanNode(string Url, string Parent, string Host, int DepthLevel)
        {
            Scan scan = new();
            HttpResponse response = Task.Run(() => { return DownloadSourceCodeSync(Url); }).Result;
            HtmlDocument? document = CreateHtmlDocument(response.SourceCode);
            if (document != null)
            {
                scan.UrlHash = Url.EncryptSHA256();
                scan.Url = Url;
                scan.ParentUrl = Parent;
                scan.Host = Host;
                scan.Title = document.DocumentNode.SelectSingleNode("/html/head/title")?.InnerText == null ? string.Empty : document.DocumentNode.SelectSingleNode("/html/head/title").InnerText;
                scan.CompressedInnerText = document.DocumentNode.SelectSingleNode("/html")?.InnerText == null ? string.Empty : Encoding.Default.GetString(document.DocumentNode.SelectSingleNode("/html").InnerText.Zip());
                scan.CompressedSourceCode = document.DocumentNode.SelectSingleNode("/html")?.InnerHtml == null ? string.Empty : Encoding.Default.GetString(document.DocumentNode.SelectSingleNode("/html").InnerHtml.Zip());
                scan.DepthLevel = DepthLevel;
                scan.FetchTimeMS = response.FetchTimeMS;
            }
            return scan;
        }
        protected bool DoesExistInQueue(string Url)
        {
            using(CrawlerContext context = new())
            {
                return context.Queue.ToHashSet().Select(p => p.Url).Contains(Url);
            }
        }
        protected bool DoesExistInScan(string Url)
        {
            using (CrawlerContext context = new())
            {
                return context.Scan.ToHashSet().Select(p => p.UrlHash).Contains(Url.EncryptSHA256());
            }
        }

        protected void Dequeue(Queue item)
        {
            using (CrawlerContext context = new())
            {
                context.Queue.Remove(item);
                context.SaveChanges();
            }
        }
        protected void Dequeue(string Url)
        {
            using (CrawlerContext context = new())
            {
                var vrTemp = context.Queue.Find(Url);
                if(vrTemp != null)
                {
                    context.Queue.Remove(vrTemp);
                    context.SaveChanges();
                }
            }
        }

        protected void Enqueue(Queue item)
        {
            using (CrawlerContext context = new())
            {
                context.Queue.Add(item);
                context.SaveChanges();
            }
        }
    }
}
