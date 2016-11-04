using Bermuda.Services;
using GoogleMusicApi.UWP.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Playback;
using Windows.UI.Core;
using Windows.UI.Xaml.Media.Imaging;

namespace Bermuda.ViewModels
{
    public class PlaylistViewModel : INotifyPropertyChanged
    {
        CoreDispatcher dispatcher;

        bool disposed;
        bool initializing;

        public event PropertyChangedEventHandler PropertyChanged;

        public Track[] songList { get; private set; }
        public MediaPlaybackItem playbackItem { get; private set; }
        public Track currentMediaItem { get; private set; }
        BitmapImage CurrentMediaImg;
        double CurrentMediaDuration;

        public BitmapImage currentMediaImg
        {
            get { return CurrentMediaImg; }

            private set
            {
                if (CurrentMediaImg != value)
                {
                    CurrentMediaImg = value;
                    RaisePropertyChanged("currentMediaImg");
                }
            }
        }

        public double currentMediaDuration
        {
            get { return CurrentMediaDuration; }

            private set
            {
                if (CurrentMediaDuration != value)
                {
                    CurrentMediaDuration = value;
                    RaisePropertyChanged("currentMediaDuration");
                }
            }
        }

        public PlaylistViewModel(Track[] sl, MediaPlaybackItem pbi, CoreDispatcher dispatcher)
        {
            songList = sl;
            playbackItem = pbi;
            this.dispatcher = dispatcher;

            if(currentMediaItem != null)
            {
                currentMediaImg = new BitmapImage();
                currentMediaImg.UriSource = new Uri(currentMediaItem.AlbumArtReference[0].Url);
                currentMediaDuration = currentMediaItem.DurationMillis;
            }

            else
            {
                currentMediaItem = songList[PlayerService.Instance.currentSongIndex];
                currentMediaImg = new BitmapImage();
                currentMediaImg.UriSource = new Uri(currentMediaItem.AlbumArtReference[0].Url);
                currentMediaDuration = currentMediaItem.DurationMillis;
            }

        }

        public async Task setCurrentMediaItem()
        {
                PlayerService.Instance.currentSongIndex++;
                currentMediaItem = songList[PlayerService.Instance.currentSongIndex];
                currentMediaImg = new BitmapImage();
                currentMediaImg.UriSource = new Uri(currentMediaItem.AlbumArtReference[0].Url);
                currentMediaDuration = currentMediaItem.DurationMillis;
                RaisePropertyChanged("currentItemIndex");
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {

        }

    }
}
