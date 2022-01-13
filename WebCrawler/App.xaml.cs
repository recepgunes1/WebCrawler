using DBEntity.Context;
using System;
using System.Runtime.ExceptionServices;
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
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.FirstChanceException += OnFirstChanceException;
        }

        private void OnFirstChanceException(object? sender, FirstChanceExceptionEventArgs e)
        {
            if (_insideFirstChanceExceptionHandler)
                return;
            try
            {
                MessageBox.Show(e.Exception.Message, "ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
                _insideFirstChanceExceptionHandler = true;
            }
            catch
            {

            }
            finally
            {
                _insideFirstChanceExceptionHandler = false;
            }
        }

        private void MainApp_Startup(object sender, StartupEventArgs e)
        {
            RegistryOperations registryOperations = new();
            if (registryOperations.DoesDatabaseExist())
            {
                using (CrawlerContext context = new())
                {
                    context.Database.EnsureCreated();
                }
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
