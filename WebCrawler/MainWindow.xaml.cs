using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using WebCrawler.Classes;
using WebCrawler.Other;

namespace WebCrawler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int irIndex { get; set; }
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
            IEnumerable<string> Urls = txtbxRootUrls.Text.Split(';').Where(p => p.IsValidUrl());
            foreach (var vrUrl in Urls)
            {
                WindowStarter(vrUrl, Convert.ToInt32(nmrcAmountOfThreads.Value), this.irIndex);
            }
        }

        private void miExitWithBackup_Click(object sender, RoutedEventArgs e)
        {
            Backup backup = new() { InputData = txtbxRootUrls.Text, SelectedScanType = irIndex, AmountOfTasks = (int)nmrcAmountOfThreads.Value };
            BackupOperation operation = new();
            operation.SaveIt(backup);
            System.Environment.Exit(1);
        }

        private void miExitWithoutBackup_Click(object sender, RoutedEventArgs e)
        {
            System.Environment.Exit(1);
        }

        private void miLoadLastBackup_Click(object sender, RoutedEventArgs e)
        {
            BackupOperation operation = new();
            Backup? backup = operation.GetBackup();
            if (backup != null)
            {
                this.txtbxRootUrls.Text = backup.InputData;
                this.irIndex = backup.SelectedScanType;
                this.nmrcAmountOfThreads.Value = backup.AmountOfTasks;
                SetRadioButton();
            }
        }
        private void SetScanTypeIndex(object sender, RoutedEventArgs e)
        {
            if (rdbtnInternalScan.IsChecked == true)
                this.irIndex = 0;
            else if (rdbtnExternalScan.IsChecked == true)
                this.irIndex = 1;
            else if (rdbtnRSSorSitemap.IsChecked == true)
                this.irIndex = 2;
        }

        private void WindowStarter(string Url, int Amount, int Type)
        {
            Thread thread = new(() =>
            {
                ScanWindow window = new(Url, Amount, Type);
                window.Show();
                System.Windows.Threading.Dispatcher.Run();
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.IsBackground = true;
            thread.Start();
        }

        private void SetRadioButton()
        {
            switch (this.irIndex)
            {
                case 0:
                    rdbtnInternalScan.IsChecked = true;
                    break;
                case 1:
                    rdbtnExternalScan.IsChecked = true;
                    break;
                case 2:
                    rdbtnRSSorSitemap.IsChecked = true;
                    break;
            }
        }
    }
}
