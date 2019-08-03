using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Images
{
    class Client
    {
        public Client() {
            _client = new HttpClient(new HttpClientHandler {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            });

            _client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip,deflate");
            _client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.142 Safari/537.36");
        }

        public async Task<byte[]> LoadImage(string url) {
            string fileName = GetCacheFileName(url);
            if (!File.Exists(fileName)) {
                var content = await FetchImage(url);
                File.WriteAllBytes(fileName, content);
                return content;
            }

            return File.ReadAllBytes(fileName);
        }

        public IEnumerable<string> GetMissingImages(IEnumerable<string> urls) {
            return urls.Where(url => !File.Exists(GetCacheFileName(url)));
        }

        string GetCacheFileName(string url) {
            var bytes = Encoding.UTF8.GetBytes(url.Trim().ToLowerInvariant());
            var baseName = Convert.ToBase64String(bytes, Base64FormattingOptions.None);
            return Path.Join(CacheDir, $"{baseName}.png");
        }

        async Task<byte[]> FetchImage(string url) {
            return await _throttle.Invoke(async () => {
                return await _client.GetByteArrayAsync(url);
            });
        }

        HttpClient _client;
        Throttle _throttle = new Throttle(TimeSpan.FromMilliseconds(150));

        readonly string CacheDir = Path.Join(".cache", "images");
    }
}