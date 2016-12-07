using Bermuda.Services;
using GoogleMusicApi.UWP.Structure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Playback;
using Windows.UI;
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

        //public Track[] SongList => PlayerService.Instance.songList;
        public ObservableCollection<Track> SongList;
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

        /*public Track[] SongList
        {
            get { return songList; }

            private set
            {
                if (songList != value)
                {
                    songList = value;
                    RaisePropertyChanged("songList");
                }
            }
        }*/

        public PlaylistViewModel(ObservableCollection<Track> sl, MediaPlaybackItem pbi, CoreDispatcher dsp)
        {
            try
            {
                this.SongList = sl;
                this.playbackItem = pbi;
                this.dispatcher = dsp;

                if (currentMediaItem != null)
                {
                    currentMediaImg = new BitmapImage();
                    currentMediaImg.UriSource = new Uri(currentMediaItem.AlbumArtReference[0].Url);
                    currentMediaDuration = currentMediaItem.DurationMillis;
                }

                else
                {
                    currentMediaItem = SongList[PlayerService.Instance.currentSongIndex];
                    currentMediaImg = new BitmapImage();
                    currentMediaImg.UriSource = new Uri(currentMediaItem.AlbumArtReference[0].Url);
                    currentMediaDuration = currentMediaItem.DurationMillis;
                }
            }

            catch (Exception e)
            {

            }

        }

        public async void setCurrentMediaItem()
        {
            try
            {
                currentMediaItem = SongList[PlayerService.Instance.currentSongIndex];

                await this.dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {

                    //RaisePropertyChanged("SongList");
                    currentMediaImg = new BitmapImage();
                    currentMediaImg.UriSource = new Uri(currentMediaItem.AlbumArtReference[0].Url);
                    currentMediaDuration = currentMediaItem.DurationMillis;
                    RaisePropertyChanged("currentMediaDuration");
                }); 
                

            }
            catch(Exception e)
            {

            }
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
