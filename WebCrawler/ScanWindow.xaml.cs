using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using WebCrawler.Other;

namespace WebCrawler
{
    /// <summary>
    /// Interaction logic for ScanWindow.xaml
    /// </summary>
    public partial class ScanWindow : Window
    {
        private string Url { get; init; }
        private int irThreadAmount { get; init; }
        private Crawler crawler { get; init; }
        private List<Task> Tasks { get; set; }
        public ScanWindow(string _Url, int _irThreadAmount)
        {
            InitializeComponent();
            Url = _Url;
            irThreadAmount = _irThreadAmount;
            crawler = new(Url);
            Tasks = new();
        }

        private void wndwScan_Loaded(object sender, RoutedEventArgs e)
        {
            this.Title = $"{Url} is scanning...";
            //crawler.InternalScan();
            crawler.ExternalScan();
        }
    }
}
