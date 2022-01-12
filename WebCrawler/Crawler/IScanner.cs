using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace WebCrawler.Crawler
{
    internal interface IScanner
    {
        ConcurrentBag<Task> ListOfTasks { get; init; }
        string Host { get; init;}
        void Scanner();
    }
}
