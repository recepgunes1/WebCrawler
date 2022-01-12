using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;

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

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            string[] arrUrls = txtbxRootUrls.Text.Split(';');
            foreach (var vrUrl in arrUrls)
            {

                Thread thread = new(() =>
                {
                    WindowStarter(vrUrl, Convert.ToInt32(nmrcAmountOfThreads.Value));
                });
                thread.SetApartmentState(ApartmentState.STA);
                thread.IsBackground = true;
                thread.Start();
            }
        }

        private void WindowStarter(string Url, int Amount)
        {
            ScanWindow window = new(Url, Amount);
            window.Show();
            System.Windows.Threading.Dispatcher.Run();

        }
    }
}
