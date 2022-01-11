using System;
using System.IO;
using System.Linq;

namespace WebCrawler.Crawler
{
    class Logger
    {
        public void Writer(Exception ex)
        {
            string output = $"Source: {ex?.Source}{Environment.NewLine}" +
$"Stack Tree: {ex?.StackTrace}{Environment.NewLine}" +
$"Message: {ex?.Message}{Environment.NewLine}" +
$"Inner Exception Source: {ex?.InnerException?.Source}{Environment.NewLine}" +
$"Inner Exception Stack Tree: {ex?.InnerException?.StackTrace}{Environment.NewLine}" +
$"Inner Exception Message: {ex?.InnerException?.Message}{Environment.NewLine}" +
$"{string.Join("", Enumerable.Repeat("=", 100))}";
            File.WriteAllText($"log\\{DateTime.Now.ToString("fffffff")}.txt", output);
        }
    }
}
