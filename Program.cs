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

            string username = args[0];
           
            string destinationFolder = Environment.CurrentDirectory;
            if (args.Length == 2) destinationFolder = args[1];

            var job = new Job(username, destinationFolder);
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
