namespace WebCrawler.Classes
{
    internal class StatusOfTask
    {
        public string Status { get; set; }
        public int Amount { get; set; }
        public override string ToString()
        {
            return $"{Status}:{Amount}";
        }
    }
}
