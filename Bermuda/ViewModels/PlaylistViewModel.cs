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
        public int currentItemIndex = 0;
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

        public PlaylistViewModel(Track[] sl, MediaPlaybackItem pbi, int csi, CoreDispatcher dispatcher)
        {
            songList = sl;
            playbackItem = pbi;
            this.dispatcher = dispatcher;
            currentItemIndex = csi;

            if(currentMediaItem != null)
            {
                currentMediaImg = new BitmapImage();
                currentMediaImg.UriSource = new Uri(currentMediaItem.AlbumArtReference[0].Url);
                currentMediaDuration = currentMediaItem.DurationMillis;
            }

            else
            {
                currentMediaItem = songList[currentItemIndex];
                currentMediaImg = new BitmapImage();
                currentMediaImg.UriSource = new Uri(currentMediaItem.AlbumArtReference[0].Url);
                currentMediaDuration = currentMediaItem.DurationMillis;
            }

        }

        public async void setCurrentMediaItem()
        {
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                currentMediaItem = songList[currentItemIndex];
                currentMediaImg = new BitmapImage();
                currentMediaImg.UriSource = new Uri(currentMediaItem.AlbumArtReference[0].Url);
                currentMediaDuration = currentMediaItem.DurationMillis;
            });
        }

        public void addTracksToPlayback()
        {
            
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
