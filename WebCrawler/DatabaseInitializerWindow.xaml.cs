using DBEntity;
using DBEntity.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace WebCrawler
{
    /// <summary>
    /// Interaction logic for DatabaseInitializerWindow.xaml
    /// </summary>
    public partial class DatabaseInitializerWindow : Window
    {
        private string? ConnectionString { get; set; }
        public DatabaseInitializerWindow()
        {
            InitializeComponent();
        }

        private void btnInit_Click(object sender, RoutedEventArgs e)
        {
            ConnectionString = "";
            Task.Run(() => { CrawlerContext context = new(); }); 
        }
    }
}
