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

        public BitmapImage tile1;

        public BitmapImage Tile1
        {
            get { return tile1; }
            set
            {
                if (tile1 != value)
                {
                    tile1 = value;
                    RaisePropertyChanged("Tile1");
                }
            }
        }

        public BitmapImage tile2;

        public BitmapImage Tile2
        {
            get { return tile2; }
            set
            {
                if (tile2 != value)
                {
                    tile2 = value;
                    RaisePropertyChanged("Tile2");
                }
            }
        }

        public BitmapImage tile3;

        public BitmapImage Tile3
        {
            get { return tile3; }
            set
            {
                if (tile3 != value)
                {
                    tile3 = value;
                    RaisePropertyChanged("Tile3");
                }
            }
        }

        public BitmapImage tile4;

        public BitmapImage Tile4
        {
            get { return tile4; }
            set
            {
                if (tile4 != value)
                {
                    tile4 = value;
                    RaisePropertyChanged("Tile4");
                }
            }
        }

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

        public bool optionsEnabled;

        public bool OptionsEnabled
        {
            get { return optionsEnabled; }

            private set
            {
                if (optionsEnabled != value)
                {
                    optionsEnabled = value;
                    RaisePropertyChanged("OptionsEnabled");
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

            createQuiltAsync(playlist);

            MenuOpen = false;
            IsVisibleZero = Visibility.Collapsed;
            IsVisibleOne = Visibility.Collapsed;
            IsVisibleTwo = Visibility.Collapsed;
        }

        public async void createQuiltAsync( Playlist playlist)
        {
            bool whatever = await createQuilt(playlist);

            if (whatever)
                OptionsEnabled = true;
            else
                OptionsEnabled = false;
        }

        public async Task<bool> createQuilt(Playlist playlist)
        {
            Random rand = new Random();

            List<Track> templist = new List<Track>();

            List<Track> finalList = new List<Track>();

            templist = await NewMain.Current.mc.ListTracksFromPlaylist(playlist);

            if (templist != null)
            {

                foreach (Track track in templist)
                {
                    if (track != null)
                    {
                        Plentry plentry = NewMain.Current.mc.GetTrackPlaylistEntry(playlist, track);

                        if (plentry != null)
                            if (!plentry.Deleted)
                                finalList.Add(track);
                    }
                }


                if (finalList != null)
                {

                    if (finalList.Count > 0)
                    {
                        string url = finalList[rand.Next(0, finalList.Count)].AlbumArtReference[0].Url;
                        BitmapImage TempImage = new BitmapImage();

                        if (url != null)
                            TempImage.UriSource = new Uri(url);
                        else
                            TempImage.UriSource = new Uri("ms-appx:///Assets/no_image.png", UriKind.Absolute);

                        Tile1 = TempImage;
                        RaisePropertyChanged("Tile1");

                        url = finalList[rand.Next(0, finalList.Count)].AlbumArtReference[0].Url;
                        TempImage = new BitmapImage();

                        if (url != null)
                            TempImage.UriSource = new Uri(url);
                        else
                            TempImage.UriSource = new Uri("ms-appx:///Assets/no_image.png", UriKind.Absolute);

                        Tile2 = TempImage;
                        RaisePropertyChanged("Tile2");

                        url = finalList[rand.Next(0, finalList.Count)].AlbumArtReference[0].Url;
                        TempImage = new BitmapImage();

                        if (url != null)
                            TempImage.UriSource = new Uri(url);
                        else
                            TempImage.UriSource = new Uri("ms-appx:///Assets/no_image.png", UriKind.Absolute);

                        Tile3 = TempImage;
                        RaisePropertyChanged("Tile3");

                        url = finalList[rand.Next(0, finalList.Count)].AlbumArtReference[0].Url;
                        TempImage = new BitmapImage();

                        if (url != null)
                            TempImage.UriSource = new Uri(url);
                        else
                            TempImage.UriSource = new Uri("ms-appx:///Assets/no_image.png", UriKind.Absolute);

                        Tile4 = TempImage;
                        RaisePropertyChanged("Tile4");

                        return true;
                    }

                    else
                    {
                        BitmapImage TempImage = new BitmapImage();

                        TempImage.UriSource = new Uri("ms-appx:///Assets/no_image.png", UriKind.Absolute);

                        Tile1 = TempImage;
                        RaisePropertyChanged("Tile1");

                        Tile2 = TempImage;
                        RaisePropertyChanged("Tile2");


                        Tile3 = TempImage;
                        RaisePropertyChanged("Tile3");


                        Tile4 = TempImage;
                        RaisePropertyChanged("Tile4");

                        return false;
                    }
                }

                else
                    return false;

            }

            else
                return false;
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
