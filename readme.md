# YouTube Auto Downloader
This simple console application will download any public Youtube playlist to your hard drive.

## Usage
	YouTubeAutoDownload favorite -k {YOUR API KEY} -p {PLAYLIST ID}

`-k` is your personal Google API key, available from the [Google Developers Console](https://console.developers.google.com/project)
`-p` is the playlist you want to download

### Schedule
You can schedule the app with the Windows Task Scheduler

## Additional information

* Requires [youtube-dl](https://github.com/rg3/youtube-dl) and [ffmpeg](https://www.ffmpeg.org/)
 * Protip: download from [youtube-dl](https://chocolatey.org/packages/youtube-dl) and [ffmpeg](ffmpeg) from [Chocolatey](https://chocolatey.org) ;) 