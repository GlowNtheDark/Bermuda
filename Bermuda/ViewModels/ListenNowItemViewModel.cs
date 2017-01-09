using GoogleMusicApi.UWP.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace Bermuda.ViewModels
{
    public class ListenNowItemViewModel : INotifyPropertyChanged
    {
        public ListenNowItem item { get; private set; }

        public QuickPlayViewModel QPViewModel;

        public string QuickPlayText;

        public bool menuOpen;

        public Visibility isvisiblezero;

        public Visibility isvisibleone;

        public BitmapImage itemImage;

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

        public async void showCheckMark(int index)
        {
            if (index == 0)
            {
                IsVisibleZero = Visibility.Visible;
                await Task.Delay(3000);
                IsVisibleZero = Visibility.Collapsed;
            }

            else
            {
                IsVisibleOne = Visibility.Visible;
                await Task.Delay(3000);
                IsVisibleOne = Visibility.Collapsed;
            }

            MenuOpen = false;
        }

        public ListenNowItemViewModel(ListenNowItem Item, QuickPlayViewModel qpviewmodel)
        {
            try
            {
                this.item = Item;
                this.QPViewModel = qpviewmodel;

                if (item.Type == "1")
                    QuickPlayText = item.Album.Id.Artist + "\n" + item.Album.Id.Title;
                else
                    QuickPlayText = item.RadioStation.Title + " Radio";

                RaisePropertyChanged("QuickPlayText");

                ItemImage = new BitmapImage();

                if (Item.Images != null)
                    ItemImage.UriSource = new Uri(Item.Images[0].Url);

                MenuOpen = false;
                IsVisibleZero = Visibility.Collapsed;
                IsVisibleOne = Visibility.Collapsed;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
