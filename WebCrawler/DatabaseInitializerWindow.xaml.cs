using DBEntity;
using DBEntity.Context;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using WebCrawler.Other;

namespace WebCrawler
{
    /// <summary>
    /// Interaction logic for DatabaseInitializerWindow.xaml
    /// </summary>
    public partial class DatabaseInitializerWindow : Window
    {
        public DatabaseInitializerWindow()
        {
            InitializeComponent();
        }

        private void btnInit_Click(object sender, RoutedEventArgs e)
        {
            btnInit.IsEnabled = false;
            btnInit.Content = "Database is being initialized.";
            try
            {
                RegistryOperations registry = new();
                int irDatabaseProvider = cmbDatabaseProvider.SelectedIndex;
                int irAuthentication = cmbAuthentication.SelectedIndex;
                string DatabaseProvider = cmbDatabaseProvider.SelectedItem.ToString().Split(':').Last().Trim();
                string ConnectionString = string.Empty;
                switch (irDatabaseProvider)
                {
                    case 0: //MsSQL
                        var vrTemp = txtbxServerName.Text.Contains("\\\\") ? txtbxServerName.Text : txtbxServerName.Text.Replace("\\", "\\\\");
                        switch (irAuthentication)
                        {
                            case 0: //Windows
                                ConnectionString = $"Server={vrTemp};Database={txtbxDatabase.Text};Integrated Security=true;";
                                break;
                            case 1: //Username-Password
                                ConnectionString = $"Server={vrTemp};Database={txtbxDatabase.Text};User id={txtbxUsername.Text};Password={pswdbxPassword.Password};Integrated Security=true;";
                                break;
                        }
                        break;
                    case 1: //SQLite
                        ConnectionString = $"Data Source={Directory.GetCurrentDirectory()}\\{txtbxDatabase.Text}.db;";
                        break;
                    case 2: //In-Memory
                        ConnectionString = $"Data Source=:memory:;Version=3;New=True;";
                        break;

                }
                registry.SaveRegistry(DatabaseProvider, ConnectionString);
                CrawlerContext context = new();
                this.Close();
            }
            catch (AggregateException Ex)
            {
                string ErrorMessage = string.Empty;
                foreach (var vrInnerException in Ex.InnerExceptions)
                {
                    ErrorMessage += vrInnerException.Message;
                }
                MessageBox.Show(ErrorMessage, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch(Exception Ex)
            {
                MessageBox.Show(Ex.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                btnInit.IsEnabled = true;
                btnInit.Content = "Initialize Database";
            }
        }
        private void wndwDatabaseInitializer_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainWindow window = new();
            window.Show();
        }
    }
}
