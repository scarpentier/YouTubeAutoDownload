using System;

namespace YouTubeFavDownload
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

            int maxDownloads = 20;
            if (args.Length >= 2) maxDownloads = int.Parse(args[1]);
            
            string destinationFolder = Environment.CurrentDirectory;
            if (args.Length == 3) destinationFolder = args[2];

            var job = new Job(username, destinationFolder, maxDownloads);
            job.Start();

            Console.WriteLine("All done");
        }

        /// <summary>
        /// Prints the help in the console window
        /// </summary>
        public static void PrintHelp()
        {
            Console.WriteLine("Downloads a YouTube user's last 20 favorite videos to your hard drive");
            Console.WriteLine();
            Console.WriteLine("Usage: YouTubeFavDownload username [maxDownloads] [destination]");
        }
    }
}
