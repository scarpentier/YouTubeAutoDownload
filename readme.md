# YouTube Auto Downloader
This simple console application will download the last 25 videos from any public Youtube playlist

## Usage
	YouTubeAutoDownload playlistId

`playlistId` is the playlist you want to download

### Schedule
You can schedule the app with the Windows Task Scheduler or by typing the following line in a command prompt:

	at 1:00 /every:M,T,W,Th,F,S,Su c:\bin\YouTubeAutoDownload 8BCDD04DE8F771B2

## Additional information

* Requires [youtube-dl](https://github.com/rg3/youtube-dl) and [ffmpeg](https://www.ffmpeg.org/)
 * Protip: download from [youtube-dl](https://chocolatey.org/packages/youtube-dl) and [ffmpeg](ffmpeg) from [Chocolatey](https://chocolatey.org) ;) 