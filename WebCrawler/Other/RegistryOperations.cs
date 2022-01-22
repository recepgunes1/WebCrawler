using Microsoft.Win32;
using System;

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
            using (RegistryKey? registryKey = Registry.CurrentUser.OpenSubKey("WebCrawler"))
            {
                if (registryKey == null)
                    return false;
                return true;
            }
        }
    }
}
// DESKTOP-QKVAVQO