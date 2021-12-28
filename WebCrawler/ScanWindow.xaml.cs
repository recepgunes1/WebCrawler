using System;
using System.Windows;
using System.Windows.Forms;

namespace WebCrawler
{
    /// <summary>
    /// Interaction logic for ScanWindow.xaml
    /// </summary>
    public partial class ScanWindow : Window
    {
        private string Url { get; init; }
        public ScanWindow(string _Url)
        {
            InitializeComponent();
            Url = _Url;
        }

        private void wndwScan_Loaded(object sender, RoutedEventArgs e)
        {
            this.Title = $"{Url} is scanning...";
            Timer timer = new();
            timer.Interval = 1000;
            timer.Tick += new EventHandler(clock_Tick);
            timer.Start();
        }
        private void clock_Tick(object sender, EventArgs e)
        {
            txtbxClock.Text = DateTime.Now.ToString("H:mm:ss");
        }
    }
}
