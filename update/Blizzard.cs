using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Blizzard
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

        public List<Card> LoadCards() {
            var pages = GetLatestPages();
            return pages
                .Select(c => c.Path)
                .Select(p => File.ReadAllText(p))
                .Select(c => JsonConvert.DeserializeObject<CardsResponse>(c))
                .SelectMany(r => r.Cards)
                .ToList();
        }

        public async Task<List<Card>> UpdateCards() {
            var pages = GetLatestPages();
            var etags = pages.Select(c => c.Entry.ETag).ToArray();

            var result = await FetchCards(etags);
            if (result.Modified) {
                int timestamp = (int)DateTimeOffset.Now.ToUnixTimeSeconds();
                for (int i = 0; i < result.Responses.Count; i++) {
                    var response = result.Responses[i];
                    var entry = new CacheEntry {
                        Timestamp = timestamp,
                        ETag = response.ETag,
                        Page = i + 1,
                    };

                    string path = entry.ToPath(CacheDir);
                    File.WriteAllText(path, response.Content);
                }
            }

            return LoadCards();
        }

        IEnumerable<(string Path, CacheEntry Entry)> GetLatestPages() {
            var cache = Directory.EnumerateFileSystemEntries(CacheDir).Select<string, (string Path, CacheEntry Entry)>(path => {
                return (path, CacheEntry.FromPath(path));
            });

            int latest = cache.Any() ? cache.Max(c => c.Entry.Timestamp) : 0;
            var pages = cache
                .Where(c => c.Entry.Timestamp == latest)
                .OrderBy(c => c.Entry.Page);

            return pages;
        }

        async Task<(bool Modified, List<(string ETag, string Content)> Responses)> FetchCards(string[] etags) {
            bool modified = false;
            var responses = new List<(string, string)>();

            for (int page = 1; ; page++) {
                string etag = etags.Length >= page ? etags[page - 1] : "";
                var result = await FetchCards(etag, page);

                modified = modified || result.Modified;
                responses.Add((result.ETag, result.Content));

                if (result.Modified) {
                    var pagination = JsonConvert.DeserializeObject<PageResponse>(result.Content);
                    Console.WriteLine($"Page {pagination.Page} of {pagination.PageCount}.");
                    if (pagination.Page == pagination.PageCount) {
                        break;
                    }
                } else if (!modified && page == etags.Length) {
                    break;
                }
            }

            return (modified, responses);
        }

        async Task<(bool Modified, string ETag, string Content)> FetchCards(string etag, int page) {
            return await _throttle.Invoke(async () => {
                var builder = new UriBuilder("https://api.blizzard.com/hearthstone/cards") {
                    Query = await new FormUrlEncodedContent(new Dictionary<string, string> {
                        { "page", page.ToString() },
                        { "collectible", "0,1" }
                    }).ReadAsStringAsync()
                };

                var uri = builder.Uri;
                using (var request = new HttpRequestMessage(HttpMethod.Get, uri)) {
                    string authToken = await FetchAuthToken();
                    request.Headers.Add("Authorization", $"Bearer {authToken}");

                    if (!string.IsNullOrEmpty(etag)) {
                        request.Headers.Add("If-None-Match", etag);
                    }

                    Console.WriteLine($"Fetch {uri}.");
                    using (var response = await _client.SendAsync(request)) {
                        if (response.StatusCode != HttpStatusCode.NotModified && response.StatusCode != HttpStatusCode.OK) {
                            throw new Exception($"Unexpected response status: {response.StatusCode}.");
                        }

                        if (response.StatusCode == HttpStatusCode.NotModified) {
                            Console.WriteLine("304 Not Modified.");
                            return (false, "", "");
                        }

                        Console.WriteLine("200 OK.");
                        string newETag = response.Headers.ETag.Tag;
                        return (true, newETag, await response.Content.ReadAsStringAsync());
                    }
                }
            });
        }

        async Task<string> FetchAuthToken() {
            if (!string.IsNullOrEmpty(_authToken)) {
                return _authToken;
            }

            Func<Task<string>> fetch = async () => {
                string content = await _client.GetStringAsync("https://playhearthstone.com/en-us/cards");

                var pattern = new Regex("cardApiSettings=\"(.*?)\"");
                var match = pattern.Match(content);
                if (!match.Success) {
                    return null;
                }

                string encoded = match.Groups[1].Value;
                if (string.IsNullOrEmpty(encoded)) {
                    return null;
                }

                var settings = JObject.Parse(HttpUtility.HtmlDecode(encoded));
                string token = settings["token"]["access_token"].ToString();
                if (string.IsNullOrEmpty(token)) {
                    return null;
                }

                return token;
            };

            _authToken = await fetch();
            if (string.IsNullOrEmpty(_authToken)) {
                throw new Exception("Authorization token not found.");
            }

            Console.WriteLine($"Use auth token {_authToken}.");
            return _authToken;
        }

        HttpClient _client;
        string _authToken;
        Throttle _throttle = new Throttle(TimeSpan.FromSeconds(1));

        readonly string CacheDir = Path.Join(".cache", "blizzard");
    }

    public enum Locale
    {
        en_US,
        de_DE,
        es_ES,
        es_MX,
        fr_FR,
        it_IT,
        ja_JP,
        ko_KR,
        pl_PL,
        pt_BR,
        ru_RU,
        th_TH,
        zh_CN,
        zh_TW
    }

    public class CacheEntry
    {
        [JsonProperty("timestamp")]
        public int Timestamp;

        [JsonProperty("etag")]
        public string ETag;

        [JsonProperty("page")]
        public int Page;

        public static CacheEntry FromPath(string path) {
            string fileName = Path.GetFileNameWithoutExtension(path);
            string json = Encoding.UTF8.GetString(Convert.FromBase64String(fileName));
            return JsonConvert.DeserializeObject<CacheEntry>(json);
        }

        public string ToPath(string path) {
            string json = JsonConvert.SerializeObject(this);
            var bytes = Encoding.UTF8.GetBytes(json);
            string fileName = Convert.ToBase64String(bytes, Base64FormattingOptions.None);
            return Path.Join(path, $"{fileName}.json");
        }
    }

    public class PageResponse
    {
        [JsonProperty("pageCount")]
        public int? PageCount;

        [JsonProperty("page")]
        public int Page;
    }

    public class CardsResponse
    {
        [JsonProperty("cards")]
        public Card[] Cards = new Card[0];

        [JsonProperty("cardCount")]
        public int CardCount;

        [JsonProperty("pageCount")]
        public int? PageCount;

        [JsonProperty("page")]
        public int Page;
    }

    public class Card
    {
        [JsonProperty("id")]
        public int DbfId;

        [JsonProperty("image")]
        public Dictionary<Locale, string> Images;
    }
}
