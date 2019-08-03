using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using CommandLine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ShellProgressBar;

namespace HearthstoneCardImages
{
    [Verb("update-cards", HelpText = "Update card database using Blizzard's Hearthstone API.")]
    class UpdateCards { }

    [Verb("download-images", HelpText = "Download card images using current card database.")]
    class DownloadImages { }

    [Verb("copy-images", HelpText = "Copy downloaded images to the cards folder.")]
    class CopyImages { }

    [Verb("check-images", HelpText = "Ensure all images in the cards folder are valid.")]
    class CheckImages { }

    [Verb("create-manifests", HelpText = "Create manifest files from current commit.")]
    class CreateManifests { }

    class Program
    {
        static async Task Main(string[] args) {
            object task = null;
            Parser.Default.ParseArguments<UpdateCards, DownloadImages, CopyImages, CheckImages, CreateManifests>(args)
                .WithParsed<UpdateCards>(t => task = t)
                .WithParsed<DownloadImages>(t => task = t)
                .WithParsed<CopyImages>(t => task = t)
                .WithParsed<CheckImages>(t => task = t)
                .WithParsed<CreateManifests>(t => task = t);

            if (task == null) {
                return;
            }

            var type = task.GetType();
            if (type == typeof(UpdateCards)) {
                await UpdateCards();
            } else if (type == typeof(DownloadImages)) {
                await DownloadImages();
            } else if (type == typeof(CopyImages)) {
                CopyImages();
            } else if (type == typeof(CheckImages)) {
                CheckImages();
            } else if (type == typeof(CreateManifests)) {
                CreateManifests();
            }
        }

        static async Task UpdateCards() {
            var client = new Blizzard.Client();
            await client.UpdateCards();
        }

        static async Task DownloadImages() {
            var images = new Images.Client();
            var client = new Blizzard.Client();
            var cards = client.LoadCards();
            var urls = images.GetMissingImages(cards.SelectMany(c => c.Images.Values)).ToArray();

            var options = new ProgressBarOptions { ForegroundColor = ConsoleColor.Yellow };
            using (var progress = new ProgressBar(urls.Length, "", options)) {
                foreach (string url in urls) {
                    progress.Tick($"Download {url}.");
                    await images.LoadImage(url);
                }
            }
        }

        static void CopyImages() {
            var images = new Images.Client();
            var client = new Blizzard.Client();
            var cards = client.LoadCards();

            int total = cards.Sum(c => c.Images.Count);
            var options = new ProgressBarOptions { ForegroundColor = ConsoleColor.Yellow };
            using (var progress = new ProgressBar(total, "", options)) {
                Parallel.ForEach(cards, card => {
                    foreach (var (locale, url) in card.Images) {
                        string dir = Path.Join("cards", locale.ToString());
                        string path = Path.Join(dir, $"{card.DbfId}.png");
                        progress.Tick($"Copy to {path}.");

                        var imageData = images.LoadImage(url).Result;
                        File.WriteAllBytes(path, imageData);
                    }
                });
            }
        }

        static void CheckImages() {
            var imagePaths = Directory.EnumerateFiles("cards", "*.png", SearchOption.AllDirectories)
                .Select(path => path.Trim())
                .ToArray();

            var options = new ProgressBarOptions { ForegroundColor = ConsoleColor.Yellow };
            using (var progress = new ProgressBar(imagePaths.Length, "", options)) {
                Parallel.ForEach(imagePaths, imagePath => {
                    progress.Tick($"Check {imagePath}.");
                    using (var image = new Bitmap(imagePath)) {
                        if (image.Width < 100 || image.Height < 100) {
                            throw new Exception($"Invalid image resolution: {imagePath}.");
                        }
                    }
                });
            }
        }

        static void CreateManifests() {
            var versionedFiles = Run("git", "ls-files")
                .Where(p => !string.IsNullOrEmpty(p))
                .Select(p => p.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar))
                .ToHashSet();

            var release = new Dictionary<string, Dictionary<int, string>>();
            foreach (var locale in Enum.GetNames(typeof(Blizzard.Locale))) {
                release[locale] = new Dictionary<int, string>();
            }

            var md5 = new MD5CryptoServiceProvider();
            var imageFileNames = Directory.EnumerateFiles("cards", "*.png", SearchOption.AllDirectories).ToArray();
            var options = new ProgressBarOptions { ForegroundColor = ConsoleColor.Yellow };
            using (var progress = new ProgressBar(imageFileNames.Length, "", options)) {
                foreach (var fileName in imageFileNames) {
                    progress.Tick($"Process {fileName}.");

                    var checkFileName = fileName.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
                    if (!versionedFiles.Contains(checkFileName)) {
                        throw new Exception($"Unversioned file: {checkFileName}.");
                    }

                    string locale = fileName.Split(Path.DirectorySeparatorChar)[1];
                    int dbfId = Convert.ToInt32(Path.GetFileNameWithoutExtension(fileName));
                    var bytes = md5.ComputeHash(File.ReadAllBytes(fileName));
                    var hash = BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
                    release[locale][dbfId] = hash;
                }
            }

            var package = JObject.Parse(File.ReadAllText("package.json"));
            string version = package["version"].ToString();
            var combined = new CombinedImageDatabase {
                Config = new CombinedConfig {
                    Version = version,
                    Base = "https://raw.githubusercontent.com/schmich/hearthstone-card-images"
                },
                Cards = release
            };

            string content = JsonConvert.SerializeObject(combined);
            File.WriteAllText(Path.Join("manifest", "all.json"), content);

            foreach (var (locale, cards) in release) {
                var single = new ImageDatabase {
                    Config = new Config {
                        Version = version,
                        Base = combined.Config.Base,
                        Locale = locale
                    },
                    Cards = release[locale]
                };

                content = JsonConvert.SerializeObject(single);
                File.WriteAllText(Path.Join("manifest", $"{locale}.json"), content);
            }

            var hashes = combined.Cards.SelectMany(p => p.Value.Values);
            foreach (var group in hashes.GroupBy(h => h)) {
                if (group.Count() > 1) {
                    throw new Exception($"Hash conflict for \"{group.Key}\".");
                }
            }
        }

        static List<string> Run(string command, string arguments) {
            var lines = new List<string>();

            var process = new Process {
                StartInfo = new ProcessStartInfo {
                    FileName = command,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    UseShellExecute = false
                },
                EnableRaisingEvents = true
            };

            process.OutputDataReceived += (_, e) => {
                lines.Add(e.Data);
            };

            process.Start();
            process.BeginOutputReadLine();
            process.WaitForExit();

            return lines;
        }
    }

    public class ImageDatabase
    {
        [JsonProperty("config")]
        public Config Config;

        [JsonProperty("cards")]
        public Dictionary<int, string> Cards;
    }

    public class Config
    {
        [JsonProperty("version")]
        public string Version;

        [JsonProperty("base")]
        public string Base;

        [JsonProperty("locale")]
        public string Locale;
    }

    public class CombinedImageDatabase
    {
        [JsonProperty("config")]
        public CombinedConfig Config;

        [JsonProperty("cards")]
        public Dictionary<string, Dictionary<int, string>> Cards;
    }

    public class CombinedConfig
    {
        [JsonProperty("version")]
        public string Version;

        [JsonProperty("base")]
        public string Base;
    }
}
