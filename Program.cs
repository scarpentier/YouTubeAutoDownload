using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using YoutubeExtractor;

namespace YouTubeAutoDownload
{
    class Program
    {
        static void Main(string[] args)
        {
            var videos = GetFavorites(args[0]);

            foreach (var vid in videos)
            {
                Console.Write("Downloading \"{0}\"... ", vid.Title);
                DownloadVideo(vid.Url, args[1]);
                Console.WriteLine("Complete!");
            }
        }

        public static IEnumerable<Video> GetFavorites(string username)
        {
            var fav = "http://gdata.youtube.com/feeds/api/users/{0}/favorites";

            var data = new System.Net.WebClient().DownloadString(String.Format(fav, username));

            XNamespace ns = XNamespace.Get("http://www.w3.org/2005/Atom");
            var xml = XDocument.Load(string.Format(fav, username));
            var videos = (from x in xml.Descendants(ns + "entry")
                          select new Video {
                              Title = x.Element(ns + "title").Value,
                              Url = x.Element(ns + "link").Attribute("href").Value
                          });

            return videos;                         
        }

        public static void DownloadVideo(string url, string destination)
        {
            if (string.IsNullOrEmpty(destination))
                destination = Environment.CurrentDirectory;

            // Get single video
            IEnumerable<VideoInfo> videoInfos = DownloadUrlResolver.GetDownloadUrls(url);

            // Get the highest MP4 resolution
            VideoInfo video = videoInfos.OrderByDescending(v => v.Resolution).First(v => v.VideoType == VideoType.Mp4);

            var filename = Path.Combine(destination, video.Title + video.VideoExtension);
            if (File.Exists(filename))
                return;

            var videoDownloader = new VideoDownloader(video, filename);

            videoDownloader.Execute();
        }

        public class Video
        {
            public string Title { get; set; }
            public string Url { get; set; }
        }
    }
}
