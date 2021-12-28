using System.Windows;
using WebCrawler.Other;


namespace WebCrawler
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void MainApp_Startup(object sender, StartupEventArgs e)
        {
            RegistryOperations registryOperations = new();
            if (registryOperations.DoesDatabaseExist())
            {
                MainWindow mainWindow = new();
                mainWindow.Show();
            }
            else
            {
                DatabaseInitializerWindow databaseInitializerWindow = new();
                databaseInitializerWindow.Show();
            }
        }
    }
}
