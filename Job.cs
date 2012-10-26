using System;
using System.Collections.Generic;
using System.Linq;

namespace YouTubeFavDownload
{
    using System.IO;
    using System.Xml.Linq;

    using YoutubeExtractor;

    internal class Job
    {
        /// <summary>
        /// URL to the YouTube API
        /// </summary>
        private const string YouTubeApiUrl = @"https://gdata.youtube.com/feeds/api/users/{0}/favorites?start-index={2}&safeSearch=none&v=2&max-results={1}";

        /// <summary>
        /// Atom XML Namespace required to read data comming from the YouTube API
        /// </summary>
        private static readonly XNamespace NsAtom = XNamespace.Get("http://www.w3.org/2005/Atom");

        /// <summary>
        /// YouTube User Name that we'll get the favorites from
        /// </summary>
        public string UserName { get; set; }

        private string destinationFolder;

        /// <summary>
        /// Destination folder in which we'll put the downloaded videos
        /// </summary>
        public string DestinationFolder
        {
            get
            {
                if (string.IsNullOrEmpty(destinationFolder)) destinationFolder = Environment.CurrentDirectory;
                return destinationFolder;
            }
            set
            {
                destinationFolder = value;
            }
        }

        public Job()
        {
            
        }

        public Job(string username, string destinationFolder)
        {
            UserName = username;
            DestinationFolder = destinationFolder;
        }

        /// <summary>
        /// Starts the download process
        /// </summary>
        public void Start()
        {
            if (string.IsNullOrEmpty(UserName))
                throw new ArgumentNullException(UserName);

            var vids = new List<Video>();
            if (File.Exists("videos.xml")) vids.LoadXml("videos.xml");

            // We're gonna get favorites until we hit a file we've already downloaded.
                      
            var favs = GetFavorites(UserName);

            foreach (var fav in favs.TakeWhile(fav => !vids.Contains(fav)))
            {
                Console.Write("Downloading {0}... ", fav.Title);
                DownloadVideo(fav.Url, DestinationFolder);
                vids.Add(fav);
                Console.WriteLine("[OK]");
            }

            // Save the file
            vids.SaveXml("videos.xml");
        }

        /// <summary>
        /// Gets the list of the user's favorites YouTube videos
        /// </summary>
        /// <param name="username">Youtube Username</param>
        /// <param name="startIndex"> </param>
        /// <returns>List of favorites</returns>
        public static List<Video> GetFavorites(string username, int startIndex = 1)
        {
            const int ApiMaxResults = 50; // Maximum results allowed by YouTube API

            var xml = XDocument.Load(string.Format(YouTubeApiUrl, username, ApiMaxResults, startIndex));

            // Get the videos
            var list = (from x in xml.Descendants(NsAtom + "entry")
                        select new Video
                        {
                            Title = x.Element(NsAtom + "title").Value,
                            Url = x.Element(NsAtom + "link").Attribute("href").Value
                        }).ToList();

            return list;
        }

        /// <summary>
        /// Downloads a YouTube video to the specified location
        /// </summary>
        /// <param name="url">Public URL of the video as a user would see it</param>
        /// <param name="destination">Destination path</param>
        public static void DownloadVideo(string url, string destination)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException(url);

            if (string.IsNullOrEmpty(destination)) destination = Environment.CurrentDirectory;

            // Get single video
            IEnumerable<VideoInfo> videoInfos = DownloadUrlResolver.GetDownloadUrls(url);

            // Get the highest MP4 resolution
            VideoInfo video = videoInfos.OrderByDescending(v => v.Resolution).First(v => v.VideoType == VideoType.Mp4);

            string filename = Path.Combine(destination, video.Title + video.VideoExtension);
            if (File.Exists(filename))
                return;

            var videoDownloader = new VideoDownloader(video, filename);

            videoDownloader.Execute();
        }
    }

    [Serializable]
    public class Video
    {
        public string Title { get; set; }
        public string Url { get; set; }
    }
}
