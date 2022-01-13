namespace DBEntity.Models
{
    public class Scan
    {
#pragma warning disable CS8618
        public int ID { get; set; } //2021112201
        public string UrlHash { get; set; } //2021112201
        public string ParentUrl { get; set; } //2021112201
        public string Host { get; set; } //2021112201
        public string Url { get; set; } //2021112201
        public string Title { get; set; } //2021112201
        public byte[] CompressedSourceCode { get; set; } //2021112201
        public byte[] CompressedInnerText { get; set; } //2021112201
        public DateTime DiscoveryDate { get; set; } //2021112201
        public long FetchTimeMS { get; set; } //2021112201
    }
}
