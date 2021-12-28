using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace WebCrawler.Other
{
    internal class RegistryOperations
    {
        public void SaveRegistry(string DatabaseProvider, string ConnectionString)
        {
            RegistryKey registryKey = Registry.CurrentUser.CreateSubKey("WebCrawler");
            registryKey.SetValue("Database Provider", DatabaseProvider);
            registryKey.SetValue("Connection String", ConnectionString);
            registryKey.Close();
        }
        public bool DoesDatabaseExist()
        {
            RegistryKey? registryKey = Registry.CurrentUser.OpenSubKey("WebCrawler");
            if (registryKey == null)
                return false;
            return true;
        }
    }
}
