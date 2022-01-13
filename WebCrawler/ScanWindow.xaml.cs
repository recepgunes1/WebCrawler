using DBEntity.Context; //2021112204
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using WebCrawler.Classes;
using WebCrawler.Crawler;

namespace WebCrawler
{
    /// <summary>
    /// Interaction logic for ScanWindow.xaml
    /// </summary>
    // 2021112210
    public partial class ScanWindow : Window
    {
        //2021112201
        private string Url { get; init; }
        private int irThreadAmount { get; init; }
        private IScanner Scan { get; set; }
        private ScanType TypeOfScan { get; init; } //2021112211
        private DispatcherTimer dispatcherTimer { get; init; }

        public ScanWindow(string _Url, int _irThreadAmount, int Type)
        {
            InitializeComponent();
            this.Url = _Url; //2021112212
            this.irThreadAmount = _irThreadAmount;
            this.TypeOfScan = (ScanType)Type;
            InitializeScan();
            dispatcherTimer = new DispatcherTimer(); //2021112241
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 1); //2021112240
            dispatcherTimer.Tick += new EventHandler(UpdateTaskManager);
            dispatcherTimer.Tick += new EventHandler(UpdateScanResult);
        }

        private void wndwScan_Loaded(object sender, RoutedEventArgs e) //2021112242
        {
            this.Title = $"{Url} is scanning... | {Scan}";
            dispatcherTimer.Start();
            Task.Factory.StartNew(() =>
            {
                Scan.Scanner();
            });

        }

        private void UpdateTaskManager(object sender, EventArgs e) //2021112242
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                List<StatusOfTask> statuses = new();
                if (Scan.ListOfTasks != null)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        StatusOfTask status = new()
                        {
                            Status = ((TaskStatus)i).ToString(),
                            Amount = Scan.ListOfTasks.Count(p => p.Status == (TaskStatus)i)
                        };
                        statuses.Add(status);
                    }
                }
                dtgrdStatusOfTasks.ItemsSource = statuses;
            }));
            GC.Collect();
        }

        private void UpdateScanResult(object sender, EventArgs e) //2021112242
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                using (CrawlerContext crawler = new()) //2021112230
                {
                    dtgrdScanResult.ItemsSource = crawler.Scan.Where(p => p.Host == Scan.Host).OrderByDescending(p => p.DiscoveryDate).ToList(); //2021112226 - 2021112250
                }
            }));
            GC.Collect();
        }

        private void InitializeScan()
        {
            switch (this.TypeOfScan)
            {
                case 0:
                    Scan = new InternalScan(this.Url, this.irThreadAmount);
                    break;
                case (ScanType)1:
                    Scan = new ExternalScan(this.Url, this.irThreadAmount);
                    break;
                case (ScanType)2:
                    Scan = new SitemapScan(this.Url, this.irThreadAmount);
                    break;
            }
        }
    }
}
