using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;

namespace WebCrawler.Other
{
    //2021112208
    public static class CompressionOperation
    {
        public static byte[] Zip(this string uncompressed) //2021112207
        {
            using (var outputMemory = new MemoryStream()) //2021112230
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
            using (var inputMemory = new MemoryStream(compressed)) //2021112230
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

        public static string EncryptSHA256(this string plaintext) //2021112207
        {
            using (SHA256 sha = SHA256.Create())
            {
                StringBuilder builder = new(); //2021112233
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
