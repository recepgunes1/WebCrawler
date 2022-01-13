using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace WebCrawler.Crawler
{
    internal interface IScanner
    {
        //2021112201
        ConcurrentBag<Task> ListOfTasks { get; init; } //2021112228
        string Host { get; init; }
        void Scanner();
    }
}
