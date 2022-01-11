using DBEntity.Models;
using HtmlAgilityPack;
using System;
using System.IO;

namespace WebCrawler.Other
{
    public static class UrlOperations
    {
        public static bool IsValidUrl(this HtmlNode? Target, string Host)
        {
            var vrTemp = Target?.Attributes["href"]?.Value;
            if (!string.IsNullOrEmpty(vrTemp))
            {
                if (vrTemp.StartsWith(Path.AltDirectorySeparatorChar) && !vrTemp.StartsWith($"{Path.AltDirectorySeparatorChar}{Path.AltDirectorySeparatorChar}"))
                {
                    var vrUrl = $"{Host}{vrTemp}";
                    return Uri.TryCreate(vrUrl, UriKind.Absolute, out Uri? uriResult1) && (uriResult1.Scheme == Uri.UriSchemeHttp || uriResult1.Scheme == Uri.UriSchemeHttps);
                }
                return Uri.TryCreate(vrTemp, UriKind.Absolute, out Uri? uriResult2) && (uriResult2.Scheme == Uri.UriSchemeHttp || uriResult2.Scheme == Uri.UriSchemeHttps);
            }
            return false;
        }

        public static bool IsValidUrl(this string Url)
        {
            return Uri.TryCreate(Url, UriKind.Absolute, out Uri? uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        public static bool DoTheyHaveSameHost(string Url, string Host)
        {
            return new Uri(Url).Host.Contains(Host);
        }

        public static bool DoTheyHaveSameHost(this Scan Node, string Host)
        {
            return Node.Host.Contains(Host);
        }
        public static bool DoTheyHaveSameHost(this Queue Node, string Host)
        {
            return Node.Host.Contains(Host);
        }

    }
}
