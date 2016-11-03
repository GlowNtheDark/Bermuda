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
        int currentItemIndex = 0;
        bool disposed;
        bool initializing;

        public event PropertyChangedEventHandler PropertyChanged;

        public List<Track> songList { get; private set; }
        public MediaPlaybackList playbackList { get; private set; }
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

        public PlaylistViewModel(List<Track> sl, MediaPlaybackList pbl, CoreDispatcher dispatcher)
        {
            songList = sl;
            playbackList = pbl;
            this.dispatcher = dispatcher;
            currentItemIndex = (int)playbackList.CurrentItemIndex;

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

            playbackList.CurrentItemChanged += PlaybackList_CurrentItemChanged;
        }

        private async void PlaybackList_CurrentItemChanged(MediaPlaybackList sender, CurrentMediaPlaybackItemChangedEventArgs args)
        {
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {


                var playbackItem = args.NewItem;

                if (playbackItem == null)
                {
                    currentMediaItem = null;
                }
                else
                {
                    // Find the single item in this list with a playback item
                    // matching the one we just received the event for.
                    currentMediaItem = songList[(int)playbackList.CurrentItemIndex];
                    currentMediaImg = new BitmapImage();
                    currentMediaImg.UriSource = new Uri(currentMediaItem.AlbumArtReference[0].Url);
                    currentMediaDuration = currentMediaItem.DurationMillis;
                    RaisePropertyChanged("currentMediaItem");
                }
            });
        }

        public void addTracksToPlaylist(Track[] trackList)
        {
            foreach (Track track in trackList)
                songList.Add(track);
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
