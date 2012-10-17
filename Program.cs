using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.IO;

using YoutubeExtractor;

namespace YouTubeFavDownload
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null || args.Length == 0) 
            {
                PrintHelp();
                return;
            }

            var videos = GetFavorites(args[0]);

            foreach (var vid in videos)
            {
                Console.Write("Downloading \"{0}\"... ", vid.Title);
                DownloadVideo(vid.Url, args[1]);
                Console.WriteLine("Complete!");
            }

            Console.WriteLine("All done");
        }

        /// <summary>
        /// Gets the list of the user's favorites YouTube videos
        /// </summary>
        /// <param name="username">Youtube Username</param>
        /// <returns>List of favorites</returns>
        public static IEnumerable<Video> GetFavorites(string username)
        {
            const string Fav = "http://gdata.youtube.com/feeds/api/users/{0}/favorites";

            var ns = XNamespace.Get("http://www.w3.org/2005/Atom"); // It's important to specify the XML namespace
            var xml = XDocument.Load(string.Format(Fav, username));
            var videos = (from x in xml.Descendants(ns + "entry")
                          select new Video {
                              Title = x.Element(ns + "title").Value,
                              Url = x.Element(ns + "link").Attribute("href").Value
                          });

            return videos;                         
        }

        /// <summary>
        /// Downloads a YouTube video to the specified location
        /// </summary>
        /// <param name="url">Public URL of the video as a user would see it</param>
        /// <param name="destination">Destination path</param>
        public static void DownloadVideo(string url, string destination)
        {
            if (string.IsNullOrEmpty(destination))
                destination = Environment.CurrentDirectory;

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

        /// <summary>
        /// Prints the help in the console window
        /// </summary>
        public static void PrintHelp()
        {
            Console.WriteLine("YouTubeAutoDownload downloads all the YouTube videos from a user's favorites to your hard drive");
            Console.WriteLine();
            Console.WriteLine("Usage: YouTubeFavDownload username [destination]");
        }

        public class Video
        {
            public string Title { get; set; }
            public string Url { get; set; }
        }
    }
}
