using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using Newtonsoft.Json;

namespace YouTubeAutoDownload
{
    internal class Job
    {
        /// <summary>
        /// URL to the YouTube API
        /// </summary>
        private const string YouTubeApiUrl = @"https://gdata.youtube.com/feeds/api/playlists/{0}?v=2";

        /// <summary>
        /// Atom XML Namespace required to read data comming from the YouTube API
        /// </summary>
        private static readonly XNamespace NsAtom = XNamespace.Get("http://www.w3.org/2005/Atom");

        private const string SaveFileName = "playlists.json";

        /// <summary>
        /// YouTube playlist id
        /// </summary>
        public string PlaylistId { get; set; }

        public Job(string playlistId)
        {
            this.PlaylistId = playlistId;
        }

        /// <summary>
        /// Starts the download process
        /// </summary>
        public void Start()
        {
            // Load json file
            var savedata = new Dictionary<string, string>();
            if (File.Exists(SaveFileName))
                savedata = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(SaveFileName));

            if (string.IsNullOrEmpty(this.PlaylistId))
                throw new ArgumentNullException(this.PlaylistId);

            var videos = GetVideos(this.PlaylistId);

            var firstDownloaded = string.Empty;

            foreach (var v in videos)
            {
                if (savedata.ContainsKey(this.PlaylistId) && savedata[this.PlaylistId] == v.Url) return;

                if (string.IsNullOrEmpty(firstDownloaded)) firstDownloaded = v.Url;

                Console.WriteLine("Downloading {0}... ", v.Title);
                try
                {
                    var process = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = "youtube-dl",
                            Arguments = "-f mp4/bestvideo+bestaudio " + v.Url
                        }
                    };
                    process.Start();
                    process.WaitForExit();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[ERROR]: {0}", ex.Message);
                }
            }

            savedata[this.PlaylistId] = firstDownloaded;
            File.WriteAllText(SaveFileName, JsonConvert.SerializeObject(savedata, Formatting.Indented));
        }

        /// <summary>
        /// Gets a list of video from a playlist ID
        /// </summary>
        /// <param name="playlistId">Id of playlist</param>
        /// <returns>List of videos</returns>
        public static List<Video> GetVideos(string playlistId)
        {
            Console.WriteLine("Getting videos...");
            
            var xml = XDocument.Load(string.Format(YouTubeApiUrl, playlistId));

            // Get the videos
            var list = (from x in xml.Descendants(NsAtom + "entry")
                        select new Video
                        {
                            Title = x.Element(NsAtom + "title").Value,
                            Url = x.Element(NsAtom + "link").Attribute("href").Value
                        }).ToList();

            return list;
        }
    }

    [Serializable]
    internal class Video
    {
        public string Title { get; set; }
        public string Url { get; set; }
    }
}
