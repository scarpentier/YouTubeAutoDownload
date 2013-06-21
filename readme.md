# YouTube Favorites Downloader
This simple console application will download a YouTube user's last 20 favorite videos to your hard drive.

## Usage
	YouTubeFavDownload username [maxDownloads] [destination]

`username` is the user's YouTube username.

`maxDownloads` (optional) is the maximum amount of videos you want to download.

`destination` (optional) is the destination on your hard drive. If it's not set, it will use the current directory.

### Schedule
You can schedule the app with the Windows Task Scheduler or by typing the following line in a command prompt:

	at 1:00 /every:M,T,W,Th,F,S,Su c:\bin\YouTubeFavDownload UserBroDude 20 d:\videos\youtube\

## Additional information

* If the video is already downloaded, it will skip it
* Requires .NET 3.5
* This application makes use of the most excellent [YouTubeExtractor](https://github.com/flagbug/YoutubeExtractor) library.