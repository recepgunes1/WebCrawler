using System.IO;
using System.IO.Compression;
using System.Text;

namespace WebCrawler.Other
{
    internal class CompressionOperation
    {
        public static byte[] Zip(string uncompressed)
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
    }
}
