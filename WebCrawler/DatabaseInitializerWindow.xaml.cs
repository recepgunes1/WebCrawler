using DBEntity.Context; //2021112204
using System.IO;
using System.Linq;
using System.Windows;
using WebCrawler.Other;

namespace WebCrawler
{
    /// <summary>
    /// Interaction logic for DatabaseInitializerWindow.xaml
    /// </summary>
    // 2021112210
    public partial class DatabaseInitializerWindow : Window
    {
        public DatabaseInitializerWindow()
        {
            InitializeComponent();
        }

        private void btnInit_Click(object sender, RoutedEventArgs e) //2021112242
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
            this.Close(); //2021112212
        }
        private void wndwDatabaseInitializer_Closing(object sender, System.ComponentModel.CancelEventArgs e) //2021112242
        {
            MainWindow window = new();
            window.Show();
        }
    }
}
