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

        BitmapImage listImage;

        public BitmapImage ListImage
        {
            get { return listImage; }

            private set
            {
                if (listImage != value)
                {
                    listImage = value;
                    RaisePropertyChanged("ListImage");
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

            ListImage = new BitmapImage();

            if(playlist.OwnerProfilePhotoUrl != null)
                ListImage.UriSource = new Uri(playlist.OwnerProfilePhotoUrl);
            else
                ListImage.UriSource = new Uri("ms-appx:///Assets/no_image.png", UriKind.Absolute);


            MenuOpen = false;
            IsVisibleZero = Visibility.Collapsed;
            IsVisibleOne = Visibility.Collapsed;
            IsVisibleTwo = Visibility.Collapsed;
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
