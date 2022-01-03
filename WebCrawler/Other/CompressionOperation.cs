using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

namespace WebCrawler.Other
{
    public static class CompressionOperation
    {
        public static byte[] Zip(this string uncompressed)
        {
            using (var outputMemory = new MemoryStream())
            {
                using (var gz = new GZipStream(outputMemory, CompressionLevel.Optimal))
                {
                    using (var sw = new StreamWriter(gz, Encoding.UTF8))
                    {
                        sw.Write(uncompressed);
                    }
                }
                return outputMemory.ToArray();
            }
        }

        public static string Unzip(byte[] compressed)
        {
            string ret = string.Empty;
            using (var inputMemory = new MemoryStream(compressed))
            {
                using (var gz = new GZipStream(inputMemory, CompressionMode.Decompress))
                {
                    using (var sr = new StreamReader(gz, Encoding.UTF8))
                    {
                        ret = sr.ReadToEnd();
                    }
                }
            }
            return ret;
        }
        public static string EncryptSHA256(this string plaintext)
        {
            using (SHA256 sha = SHA256.Create())
            {
                StringBuilder builder = new();
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(plaintext));
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("X2"));
                }
                return builder.ToString();
            }
        }
    }
}
