using GoogleMusicApi.UWP.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace Bermuda.ViewModels
{
    public class PlaylistItemViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public Playlist playlist { get; private set; }

        public string Name => playlist.Name;

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

        public PlaylistItemViewModel(Playlist playlist)
        {
            this.playlist = playlist;

            RaisePropertyChanged("Name");

            ListImage = new BitmapImage();
            ListImage.UriSource = new Uri(playlist.OwnerProfilePhotoUrl);
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
