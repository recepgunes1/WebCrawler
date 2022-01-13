namespace WebCrawler.Classes
{
    internal class HttpResponse
    {
        //2021112201
        public string SourceCode { get; set; }
        public long FetchTimeMS { get; set; }
        //2021112213
        public override string ToString()
        {
            return $"Source Code: {WebCrawler.Other.CompressionOperation.Zip(SourceCode)}";
        }
    }
}
