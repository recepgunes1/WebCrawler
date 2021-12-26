﻿namespace DBEntity.Models
{
    public class Scan
    {
#pragma warning disable CS8618
        public string UrlHash { get; set; }
        public string ParentUrl { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string CompressedSourceCode { get; set; }
        public string CompressedInnerText { get; set; }
        public long FetchTimeMS { get; set; }
    }
}