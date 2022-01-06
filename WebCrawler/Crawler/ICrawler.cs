using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebCrawler.Crawler
{
    internal interface ICrawler
    {
        string MainUrl { get; init; }
        string Host { get; init; }
        bool RecursionFlag { get; set; }
        int DepthLevel { get; set; }
        HashSet<string>? GetUrlsFromSourceCode(string Url);
        void InitializeQueue();
        void InitializeQueue(IEnumerable<string> Param);
        void Scanner();
    }
}
