# YouTube Favorites Downloader
This simple console application will download a YouTube user's last 25 favorite videos on your hard drive.

## Usage
	YouTubeFavDownload username [destination]

`username` is the user's YouTube username.

`destination` (optional) is the destination on your hard drive. If it's not set, it will use the current directory.

## Additional information

If the video is already downloaded, it will skip it

You can schedule the app so your computer automaticaly gets a copy of your favorites YouTube videos.

Requires .NET 3.5

This application makes use of the most excellent [YouTubeExtractor](https://github.com/flagbug/YoutubeExtractor) library.