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
    public class ListenNowItemViewModel : INotifyPropertyChanged
    {
        public ListenNowItem item { get; private set; }

        public QuickPlayViewModel QPViewModel;

        public string QuickPlayText;

        public bool menuOpen;

        public BitmapImage itemImage;

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

        public BitmapImage ItemImage
        {
            get { return itemImage; }

            private set
            {
                if (itemImage != value)
                {
                    itemImage = value;
                    RaisePropertyChanged("ItemImage");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void openCloseMenu()
        {
            MenuOpen = !MenuOpen;
        }

        public ListenNowItemViewModel(ListenNowItem Item, QuickPlayViewModel qpviewmodel)
        {
            this.item = Item;
            this.QPViewModel = qpviewmodel;

            if (item.Type == "1")
                QuickPlayText = item.Album.Id.Artist + "\n" + item.Album.Id.Title;
            else
                QuickPlayText = item.RadioStation.Title + " Radio";

            RaisePropertyChanged("QuickPlayText");

            ItemImage = new BitmapImage();
            ItemImage.UriSource = new Uri(Item.Images[0].Url);

            MenuOpen = false;
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
