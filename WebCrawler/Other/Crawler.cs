using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using WebCrawler.Classes;

namespace WebCrawler.Other
{
    internal class Crawler
    {
        private string _Url;
        private string Url { get { return _Url; } set { _Url = value; } }
        private string Host { get; init; }
        public Crawler(string Target)
        {
            Url = Target;
            Host = new Uri(Url).Host;
        }
        private async Task<HttpResponse> DownloadSourceCodeSync()
        {
            Stopwatch stopwatch = new();
            HttpResponse httpResponse = new();
            stopwatch.Start();
            try
            {
                using (HttpClient client = new())
                {
                    using (HttpResponseMessage response = await client.GetAsync(Url))
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

    }
}
