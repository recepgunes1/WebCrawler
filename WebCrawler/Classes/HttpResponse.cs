namespace WebCrawler.Classes
{
    internal class HttpResponse
    {
        public string SourceCode { get; set; }
        public long FetchTimeMS { get; set; }
        public override string ToString()
        {
            return $"Source Code: {WebCrawler.Other.CompressionOperation.Zip(SourceCode)}";
        }
    }
}
