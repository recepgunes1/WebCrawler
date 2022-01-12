using System;

namespace WebCrawler.Other
{
    public static class UrlOperations
    {
        public static bool IsValidUrl(this string Url)
        {
            return Uri.TryCreate(Url, UriKind.Absolute, out Uri? uriResult) && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        public static bool DoTheyHaveSameHost(string Url, string Host)
        {
            return new Uri(Url).Host == Host;
        }

    }
}
