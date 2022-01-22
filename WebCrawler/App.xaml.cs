using DBEntity.Context;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Data.SqlClient;
using System;
using System.Runtime.ExceptionServices;
using System.Windows;
using WebCrawler.Other;
using System.IO;

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
                if (File.Exists("C:\\true"))
                {
                    LogOperation log = new();
                    log.InsertLog(e.Exception);
                }
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
