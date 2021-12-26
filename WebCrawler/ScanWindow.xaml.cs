using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WebCrawler
{
    /// <summary>
    /// Interaction logic for ScanWindow.xaml
    /// </summary>
    public partial class ScanWindow : Window
    {
        public ScanWindow()
        {
            InitializeComponent();
        }

        private void wndwScan_Loaded(object sender, RoutedEventArgs e)
        {
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
