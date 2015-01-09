using System;

namespace YouTubeAutoDownload
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null || args.Length < 1) 
            {
                PrintHelp();
                return;
            }

            string playlistid = args[0];
           
            var job = new Job(playlistid);
            job.Start();

            Console.WriteLine("All done");
        }

        /// <summary>
        /// Prints the help in the console window
        /// </summary>
        public static void PrintHelp()
        {
            Console.WriteLine("Downloads the last 25 videos from any public Youtube playlist");
            Console.WriteLine();
            Console.WriteLine("Usage: YouTubeFavDownload playlistId [destination]");
        }
    }
}
