namespace WebCrawler.Classes
{
    internal class StatusOfTask
    {
        //2021112201
        public string Status { get; set; }
        public int Amount { get; set; }
        //2021112213
        public override string ToString()
        {
            return $"{Status}:{Amount}";
        }
    }
}
