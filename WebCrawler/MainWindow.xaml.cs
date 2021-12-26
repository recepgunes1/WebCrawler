using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;

namespace WebCrawler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void wndwMain_PreviewDrop(object sender, DragEventArgs e)
        {
            var vrTemp = (string[]?)e.Data.GetData(DataFormats.FileDrop);
            if (vrTemp != null)
            {
                var vrFilePath = vrTemp.First();
                var vrFileName = vrFilePath.Split(Path.DirectorySeparatorChar).Last();
                var vrFileExtension = vrFileName.Split('.').Last();
                if (vrFileExtension == "txt")
                {
                    var vrIEnumerable = File.ReadAllLines(vrFilePath).Select(p => p.Trim().ToLower()).Distinct().Select(p => p.Last() == '/' ? p.Remove(p.Length - 1) : p);
                    txtbxRootUrls.Text = string.Join(";", vrIEnumerable);
                    e.Handled = true;
                }
                else
                {
                    MessageBox.Show($"Your file have to be txt file not {vrFileExtension}.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }

        }
    }
}
