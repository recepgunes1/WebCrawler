namespace WebCrawler.Crawler
{
    internal class InternalScan : CommonMethods//, ICrawler
    {
        //public string MainUrl { get; init; }
        //public string Host { get; init; }
        //public bool RecursionFlag { get; set; }
        //public int DepthLevel { get; set; }
        //public int AmountOfMaxTasks { get; init; }

        //public InternalScan(string Url)
        //{
        //    MainUrl = Url;
        //    Host = new Uri(Url).Host;
        //    RecursionFlag = false;
        //    DepthLevel = 1;
        //}

        //public void GetUrlsFromSourceCodeToQueue(string SourceCode, string ParentUrl)
        //{
        //    var vrDocument = CreateHtmlDocument(SourceCode);
        //    if(vrDocument != null)
        //    {
        //        var vrSelectedNodes = vrDocument.DocumentNode.SelectNodes("//a");
        //        if (vrSelectedNodes != null)
        //        {
        //        }
        //    }
        //}

        //public void InitializeQueue()
        //{
        //    HttpResponse response = Task.Run(() => { return DownloadSourceCodeSync(this.MainUrl); }).Result;
        //    var vrUrls = GetUrlsFromSourceCode(response.SourceCode, this.MainUrl);
        //    if(vrUrls != null)
        //    {
        //        foreach(var vrUrl in vrUrls)
        //        {
        //            if(UrlOperations.DoTheyHaveSameHost(vrUrl, this.Host))
        //            {
        //                if (!DoesExistInQueue(vrUrl))
        //                {
        //                    var vrQueue = new Queue() { Url = vrUrl, Host = this.Host };
        //                    Enqueue(vrQueue);
        //                }
        //            }
        //        }
        //    }
        //}

        //public void InitializeQueue(IEnumerable<string> Param)
        //{
        //    if (Param != null)
        //    {
        //        foreach (var vrUrl in Param)
        //        {
        //            if (UrlOperations.DoTheyHaveSameHost(vrUrl, this.Host))
        //            {
        //                if (!DoesExistInQueue(vrUrl))
        //                {
        //                    var vrQueue = new Queue() { Url = vrUrl, Host = this.Host };
        //                    Enqueue(vrQueue);
        //                }
        //            }
        //        }
        //    }
        //}

        //public void Scanner()
        //{
        //    if (RecursionFlag)
        //    {
        //        using (CrawlerContext context = new())
        //        {
        //            var vrUrls = context.Scan.ToList().Where(p => p.DoTheyHaveSameHost(this.Host)).Select(p => p.Url).Distinct();
        //            var vrParentUrls = context.Scan.ToList().Where(p => p.DoTheyHaveSameHost(this.Host)).Select(p => p.ParentUrl).Distinct();
        //            var vrSubParentUrls = vrUrls.Where(p => !vrParentUrls.Contains(p));
        //            InitializeQueue(vrSubParentUrls);
        //        }
        //    }
        //    else
        //    {
        //        InitializeQueue();
        //    }
        //    var Queue = new CrawlerContext().Queue.Where(p => p.DoTheyHaveSameHost(this.Host)).Select(p => p.Url);
        //    if (Queue.Count() == 0 || Queue == null)
        //        return;
        //    foreach (var ParentUrl in Queue)
        //    {
        //        Dequeue(ParentUrl);
        //        var vrResponse = Task.Run(() => { return DownloadSourceCodeSync(ParentUrl); }).Result;
        //        GetUrlsFromSourceCode(vrResponse.SourceCode, ParentUrl);
        //        if(vrSubUrls != null)
        //        {
        //            foreach (var vrSubUrl in vrSubUrls)
        //            {
        //                if (!DoesExistInScan(vrSubUrl))
        //                {
        //                    var vrNode = CreateScanNode(vrSubUrl, ParentUrl, this.Host, this.DepthLevel);
        //                    if (vrNode != null)
        //                    {
        //                        using (CrawlerContext context = new())
        //                        {
        //                            context.Scan.Add(vrNode);
        //                            context.SaveChanges();
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    this.DepthLevel++;
        //    this.RecursionFlag = true;
        //    Scanner();
        //}
    }
}
