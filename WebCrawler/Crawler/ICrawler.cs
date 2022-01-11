using System.Collections.Generic;

namespace WebCrawler.Crawler
{
    internal interface ICrawler
    {
        string MainUrl { get; init; }
        string Host { get; init; }
        bool RecursionFlag { get; set; }
        int DepthLevel { get; set; }
        int AmountOfMaxTasks { get; init; }
        void GetUrlsFromSourceCodeToQueue(string SourceCode, string ParentUrl);
        void InitializeQueue();
        void InitializeQueue(IEnumerable<string> Param);
        void Scanner();
    }
}
