using Bermuda.DataModels;
using GoogleMusicApi.UWP.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Bermuda.ViewModels
{
    public class PlaylistItemViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Playlist playlist { get; private set; }

        public PlaylistViewModel PLViewModel;

        public QuiltListViewModel albumsquilt;

        public string Name => playlist.Name;

        public bool menuOpen;

        public SolidColorBrush borderBrush;

        public SolidColorBrush BorderBrush
        {
            get { return borderBrush; }

            private set
            {
                if (borderBrush != value)
                {
                    borderBrush = value;
                    RaisePropertyChanged("BorderBrush");
                }
            }
        }

        public Visibility isvisiblezero;

        public Visibility isvisibleone;

        public Visibility isvisibletwo;

        public Visibility IsVisibleZero
        {
            get { return isvisiblezero; }

            private set
            {
                if (isvisiblezero != value)
                {
                    isvisiblezero = value;
                    RaisePropertyChanged("IsVisibleZero");
                }
            }
        }

        public Visibility IsVisibleOne
        {
            get { return isvisibleone; }

            private set
            {
                if (isvisibleone != value)
                {
                    isvisibleone = value;
                    RaisePropertyChanged("IsVisibleOne");
                }
            }
        }

        public Visibility IsVisibleTwo
        {
            get { return isvisibletwo; }

            private set
            {
                if (isvisibletwo != value)
                {
                    isvisibletwo = value;
                    RaisePropertyChanged("IsVisibleTwo");
                }
            }
        }

        public async void showCheckMark(int index)
        {
            if (index == 0)
            {
                IsVisibleZero = Visibility.Visible;
                await Task.Delay(2000);
                IsVisibleZero = Visibility.Collapsed;
            }

            else if(index == 1)
            {
                IsVisibleOne = Visibility.Visible;
                await Task.Delay(2000);
                IsVisibleOne = Visibility.Collapsed;
            }

            else
            {
                IsVisibleTwo = Visibility.Visible;
                await Task.Delay(2000);
                IsVisibleTwo = Visibility.Collapsed;
            }

            MenuOpen = false;
        }

        public QuiltListViewModel AlbumsQuilt
        {
            get { return albumsquilt; }

            private set
            {
                if (albumsquilt != value)
                {
                    albumsquilt = value;
                    RaisePropertyChanged("AlbumsQuilt");
                }
            }
        }

        public bool MenuOpen
        {
            get { return menuOpen; }

            private set
            {
                if (menuOpen != value)
                {
                    menuOpen = value;
                    RaisePropertyChanged("MenuOpen");
                }
            }
        }

        public void openCloseMenu()
        {
            MenuOpen = !MenuOpen;
        }

        public PlaylistItemViewModel(Playlist playlist, PlaylistViewModel plviewmodel, SolidColorBrush brush)
        {
            this.playlist = playlist;
            this.PLViewModel = plviewmodel;
            this.BorderBrush = brush;
            RaisePropertyChanged("Name");
            AlbumsQuilt = new QuiltListViewModel();
            createQuiltAsync(playlist);

            MenuOpen = false;
            IsVisibleZero = Visibility.Collapsed;
            IsVisibleOne = Visibility.Collapsed;
            IsVisibleTwo = Visibility.Collapsed;
        }

        public async void createQuiltAsync( Playlist playlist)
        {
            bool whatever = await createQuilt(playlist);
        }

        public async Task<bool> createQuilt(Playlist playlist)
        {
            Random rand = new Random();

            List<Track> templist = new List<Track>();

            templist = await NewMain.Current.mc.ListTracksFromPlaylist(playlist);

            if (templist.Count > 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    string url = templist[rand.Next(0, templist.Count)].AlbumArtReference[0].Url;
                    BitmapImage TempImage = new BitmapImage();

                    if (url != null)
                        TempImage.UriSource = new Uri(url);
                    else
                        TempImage.UriSource = new Uri("ms-appx:///Assets/no_image.png", UriKind.Absolute);

                    AlbumsQuilt.Add(new QuiltItemViewModel(TempImage));
                    RaisePropertyChanged("AlbumsQuilt");
                }
            }

            else
            {
                for (int i = 0; i < 4; i++)
                {
                    BitmapImage TempImage = new BitmapImage();
                    TempImage.UriSource = new Uri("ms-appx:///Assets/no_image.png", UriKind.Absolute);

                    AlbumsQuilt.Add(new QuiltItemViewModel(TempImage));
                    RaisePropertyChanged("AlbumsQuilt");
                }

            }

            return true;
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
