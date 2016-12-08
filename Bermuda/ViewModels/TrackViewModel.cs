using GoogleMusicApi.UWP.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Bermuda.ViewModels 
{
    public class TrackViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        //TrackViewModel trackViewModel;

        TrackListViewModel listViewModel;

        public Track song { get; private set; }

        public string Title => song.Title;

        public string Artist => song.Artist;

        public string Album => song.Album;

        public SolidColorBrush tileColor => song.tileColor;

        public double currentSongDuration;

        BitmapImage previewImage;

        public BitmapImage PreviewImage
        {
            get { return previewImage; }

            private set
            {
                if (previewImage != value)
                {
                    previewImage = value;
                    RaisePropertyChanged("PreviewImage");
                }
            }
        }

        public double CurrentSongDuration
        {
            get { return currentSongDuration; }

            private set
            {
                if (currentSongDuration != value)
                {
                    currentSongDuration = value;
                    RaisePropertyChanged("CurrentSongDuration");
                }
            }
        }

        public TrackViewModel(TrackListViewModel trackViewModel, Track song)
        {
            this.listViewModel = trackViewModel;
            this.song = song;

            RaisePropertyChanged("Title");

            // This app caches all images by loading the BitmapImage
            // when the item is created, but production apps would
            // use a more resource friendly paging mechanism or
            // just use PreviewImageUri directly.
            //
            // The reason we cache here is to avoid audio gaps 
            // between tracks on transitions when changing artwork.
            PreviewImage = new BitmapImage();
            PreviewImage.UriSource = new Uri(song.AlbumArtReference[0].Url);
            CurrentSongDuration = song.DurationMillis;
        }

        public async void setSongActiveColor()
        {
            await listViewModel.dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                song.tileColor = new Windows.UI.Xaml.Media.SolidColorBrush(Colors.Green);
                RaisePropertyChanged("tileColor");
            });
        }

        public async void setSongInactiveColor()
        {
            await listViewModel.dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                song.tileColor = new Windows.UI.Xaml.Media.SolidColorBrush(Colors.Transparent);
                RaisePropertyChanged("tileColor");
            });
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
