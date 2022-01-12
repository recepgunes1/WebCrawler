using DBEntity.Context;
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
    public partial class ScanWindow : Window
    {
        private string Url { get; init; }
        private int irThreadAmount { get; init; }
        private SitemapScan scan { get; init; }
        private DispatcherTimer dispatcherTimer { get; init; }

        public ScanWindow(string _Url, int _irThreadAmount)
        {
            InitializeComponent();
            Url = _Url;
            irThreadAmount = _irThreadAmount;
            scan = new(Url, this.irThreadAmount);
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 1);
            dispatcherTimer.Tick += new EventHandler(UpdateTaskManager);
            dispatcherTimer.Tick += new EventHandler(UpdateScanResult);
        }

        private void wndwScan_Loaded(object sender, RoutedEventArgs e)
        {
            this.Title = $"{Url} is scanning...";
            dispatcherTimer.Start();
            Task.Factory.StartNew(() =>
            {
                scan.Scanner();
            });

        }

        private void UpdateTaskManager(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                List<StatusOfTask> statuses = new();
                if (scan.ListOfTasks != null)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        StatusOfTask status = new()
                        {
                            Status = ((TaskStatus)i).ToString(),
                            Amount = scan.ListOfTasks.Count(p => p.Status == (TaskStatus)i)
                        };
                        statuses.Add(status);
                    }
                }
                dtgrdStatusOfTasks.ItemsSource = statuses;
            }));
            GC.Collect();
        }

        private void UpdateScanResult(object sender, EventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() =>
            {
                using (CrawlerContext crawler = new())
                {
                    dtgrdScanResult.ItemsSource = crawler.Scan.Where(p => p.Host == scan.Host).OrderByDescending(p => p.DiscoveryDate).ToList();
                }
            }));
            GC.Collect();
        }

    }
}
