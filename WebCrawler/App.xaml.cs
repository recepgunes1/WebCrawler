using System.Windows;
using WebCrawler.Other;


namespace WebCrawler
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private volatile bool _insideFirstChanceExceptionHandler;
        public App()
        {
            //AppDomain currentDomain = AppDomain.CurrentDomain;
            //currentDomain.FirstChanceException += OnFirstChanceException;
        }

        //private void OnFirstChanceException(object sender, FirstChanceExceptionEventArgs e)
        //{
        //    System.Diagnostics.Debug.WriteLine(e.Exception.ToString());
        //    if (_insideFirstChanceExceptionHandler)
        //        return;
        //    try
        //    {
        //        MessageBox.Show(e.Exception.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
        //        _insideFirstChanceExceptionHandler = true;
        //        //logger will be added
        //    }
        //    catch
        //    {

        //    }
        //    finally
        //    {
        //        //_insideFirstChanceExceptionHandler = false;
        //    }
        //    //Microsoft.Data.SqlClient.SqlException delete registry data
        //}

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
