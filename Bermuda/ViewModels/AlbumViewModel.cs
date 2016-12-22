using Bermuda.Services;
using GoogleMusicApi.UWP.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Bermuda.ViewModels
{
    public class AlbumViewModel : INotifyPropertyChanged
    {
        public AlbumListViewModel ALViewModel;

        public Album album { get; private set; }

        public string Name => album.Name;

        public string Artist => album.Artist;

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

        public bool menuOpen;

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

        public async void menuItemClicked(object sender, ItemClickEventArgs e)
        {
            GridView gv = sender as GridView;
            StackPanel sp = e.ClickedItem as StackPanel;
            int index = gv.Items.IndexOf(sp.Parent);
            var itemviewmodel = gv.DataContext as AlbumViewModel;

            if (index == 0) //Clear queue and play
            {
                /*PlayerService.Instance.CurrentPlaylist.Clear();
                PlayerService.Instance.CurrentPlaylist.Add(itemviewmodel.song);
                PlayerService.Instance.Player.Source = new MediaPlaybackItem(MediaSource.CreateFromUri(await GetStreamUrl(NewMain.Current.mc, PlayerService.Instance.CurrentPlaylist[0])));
                PlayerService.Instance.Player.Play();*/

            }

            else if (index == 1) //Add to end of queue
            {
                //PlayerService.Instance.CurrentPlaylist.Add(itemviewmodel.song);
            }

            else 
            {
                /*Plentry plentry = NewMain.Current.mc.GetTrackPlaylistEntry(itemviewmodel.playlist, itemviewmodel.song);
                MutateResponse response = await NewMain.Current.mc.RemoveSongsFromPlaylist(plentry);
                itemviewmodel.listViewModel.Remove(itemviewmodel);*/
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public AlbumViewModel(AlbumListViewModel alviewmodel, Album album)
        {
            this.ALViewModel = alviewmodel;
            this.album = album;

            RaisePropertyChanged("Name");

            PreviewImage = new BitmapImage();
            PreviewImage.UriSource = new Uri(album.AlbumArtRef);
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
