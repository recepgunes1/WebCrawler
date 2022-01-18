using DBEntity.Context;
using System;
using System.IO;
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
        private StreamWriter swLogger;
        public App()
        {
            AppDomain currentDomain = AppDomain.CurrentDomain;
            InitializeLogger();
            currentDomain.FirstChanceException += OnFirstChanceException;
        }

        private void OnFirstChanceException(object? sender, FirstChanceExceptionEventArgs e)
        {
            if (_insideFirstChanceExceptionHandler)
                return;
            try
            {
                string log = CreateLogText(e.Exception);
                if (log != null)
                {
                    swLogger.WriteLine(log);
                    swLogger.Flush();
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

        private void InitializeLogger()
        {
            swLogger = new StreamWriter($@"{DateTime.Now.ToString("yyyyMMddHHmmss")}.html", append: true);
            swLogger.WriteLine("<link rel=\"stylesheet\" href=\"https://stackpath.bootstrapcdn.com/bootstrap/4.3.1/css/bootstrap.min.css\" integrity=\"sha384-ggOyR0iXCbMQv3Xipma34MD+dH/1fQ784/j6cY/iJTQUOhcWr7x9JvoRxT2MZw1T\" crossorigin=\"anonymous\">");
            swLogger.Flush();
        }

        private string CreateLogText(Exception exception)
        {
            string log = $"<table class=\"table table-bordered col-sm-11\" style=\"margin: 2%;\">" +
    $"<tr class=\"d-flex\"><th class=\"col-1\">Occurred Date</th><td class=\"col-11\">{DateTime.Now.ToString("F")}</td></tr>" +
    $"<tr class=\"d-flex\"><th class=\"col-1\">Exception</th><td class=\"col-11\">{exception}</td></tr>" +
    $"<tr class=\"d-flex\"><th class=\"col-1\">Exception Message</th><td class=\"col-11\">{exception.Message}</td></tr>" +
    $"<tr class=\"d-flex\"><th class=\"col-1\">Exception Source</th><td class=\"col-11\">{exception.Source}</td></tr>" +
    $"<tr class=\"d-flex\"><th class=\"col-1\">Exception Stack Tree</th><td class=\"col-11\">{exception.StackTrace}</td></tr>" +
    $"<tr class=\"d-flex\"><th class=\"col-1\">Exception HResult</th><td class=\"col-11\">{exception.HResult}</td></tr>" +
    $"<tr class=\"d-flex\"><th class=\"col-1\">Exception Data</th><td class=\"col-11\">{exception.Data}</td></tr>" +
    $"<tr class=\"d-flex\"><th class=\"col-1\">Inner Exception</th><td class=\"col-11\">{exception?.InnerException}</td></tr>" +
    $"<tr class=\"d-flex\"><th class=\"col-1\">Inner Exception Message</th><td class=\"col-11\">{exception?.InnerException?.Message}</td></tr>" +
    $"<tr class=\"d-flex\"><th class=\"col-1\">Inner Exception Source</th><td class=\"col-11\">{exception?.InnerException?.Source}</td></tr>" +
    $"<tr class=\"d-flex\"><th class=\"col-1\">Inner Exception Stack Tree</th><td class=\"col-11\">{exception?.InnerException?.StackTrace}</td></tr>" +
    $"<tr class=\"d-flex\"><th class=\"col-1\">Inner Exception HResult</th><td class=\"col-11\">{exception?.InnerException?.HResult}" +
    $"<tr class=\"d-flex\"><th class=\"col-1\">Inner Exception Data</th><td class=\"col-11\">{exception?.InnerException?.Data}</td></tr>" +
    $"</table><hr>";
            return log;
        }
    }
}
