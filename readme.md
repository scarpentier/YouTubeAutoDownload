# YouTube Favorites Downloader
This simple console application will download a YouTube user's last 50 favorite videos to your hard drive.

## Usage
	YouTubeFavDownload username [destination]

`username` is the user's YouTube username.

`destination` (optional) is the destination on your hard drive. If it's not set, it will use the current directory.

### Schedule
You can schedule the app with the Windows Task Scheduler or by typing the following line in a command prompt:

	at 1:00 /every:M,T,W,Th,F,S,Su c:\bin\YouTubeFavDownload UserBroDude d:\videos\youtube\

## Additional information

* If the video is already downloaded, it will skip it
* Requires .NET 3.5
* This application makes use of the most excellent [YouTubeExtractor](https://github.com/flagbug/YoutubeExtractor) library.