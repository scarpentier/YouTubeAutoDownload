using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace YouTubeAutoDownload.Commands
{
    public class Playlist : ManyConsole.ConsoleCommand
    {
        private string Key { get; set; }
        private string PlaylistId { get; set; }
        private bool IsAudioOnly { get; set; }

        private const string ApiUrl = "https://www.googleapis.com/youtube/v3/playlistItems?key={0}&part=contentDetails&playlistId={1}&maxResults=50&pageToken={2}";

        public Playlist()
        {
            // https://www.googleapis.com/youtube/v3/playlistItems?part=contentDetails&playlistId=FLGYu4FtQvM2Tmm58SLtKX_A&key=AIzaSyCfa44pDcLR4w3Wrk2jyiqPRqc8Xr8oxFg&maxResults=50&pageToken=CMgBEAA
            this.IsCommand("playlist", "Downloads youtube videos from a playlist");
            this.HasRequiredOption("k|key=", "API Key", k => Key = k);
            this.HasRequiredOption("p|playlist=", "Playlist Id", p => PlaylistId = p);
            this.HasOption("audioonly", "Downloads only audio", a => IsAudioOnly = !string.IsNullOrEmpty(a));
        }

        public override int Run(string[] remainingArguments)
        {
            var list = new List<string>();
            var jsonData = new JObject();
            var nextPage = string.Empty;

            while (true)
            {
                // Download json data
                jsonData = JObject.Parse(new WebClient().DownloadString(string.Format(ApiUrl, Key, PlaylistId, nextPage)));

                list.AddRange(jsonData["items"].Select(item => (string)item["contentDetails"]["videoId"]));

                if (jsonData["nextPageToken"] == null) break;
                nextPage = (string)jsonData["nextPageToken"];
            }

            this.DownloadVideos(list);

            return 1;
        }

        private void DownloadVideos(IEnumerable<string> videos)
        {
            var saveFileName = string.Format("playlist-{0}.json", PlaylistId);

            // Load json file
            var savedata = new List<string>();
            if (File.Exists(saveFileName))
                savedata = JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(saveFileName));

            var firstDownloaded = string.Empty;

            // Do we want to download video+audio or audio only?
            string args;
            if (IsAudioOnly)
                args = "-f bestaudio https://www.youtube.com/watch?v={0}";
            else
                args = "-f bestvideo+bestaudio https://www.youtube.com/watch?v={0}";

            foreach (var v in videos.Where(v => !savedata.Contains(v)))
            {
                if (string.IsNullOrEmpty(firstDownloaded)) firstDownloaded = v;

                Console.WriteLine("Downloading {0}... ", v);
                try
                {
                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "youtube-dl",
                            Arguments = string.Format(args, v)
                        }
                    };
                    process.Start();
                    process.WaitForExit();

                    savedata.Add(v);
                    File.WriteAllText(saveFileName, JsonConvert.SerializeObject(savedata, Formatting.Indented));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[ERROR]: {0}", ex.Message);
                }
            }
        }
    }
}
