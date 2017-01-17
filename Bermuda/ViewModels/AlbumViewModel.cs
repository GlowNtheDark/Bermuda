using Bermuda.DataModels;
using Bermuda.Services;
using GoogleMusicApi.UWP.Common;
using GoogleMusicApi.UWP.Structure;
using GoogleMusicApi.UWP.Structure.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Bermuda.ViewModels
{
    public class AlbumViewModel : INotifyPropertyChanged
    {
        public AlbumListViewModel ALViewModel;

        MessagingViewModel MessageViewModel;

        public Album album { get; private set; }

        public string Name => album.Name;

        public string Artist => album.Artist;

        public string AlbumID => album.AlbumId;

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

        public void openCloseMenu()
        {
            MenuOpen = !MenuOpen;
        }

        public async void showCheckMark(int index)
        {
            if (index == 0)
            {
                IsVisibleZero = Visibility.Visible;
                await Task.Delay(2000);
                IsVisibleZero = Visibility.Collapsed;
            }

            else if (index == 1)
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

        public async void searchMenuItemClicked(object sender, ItemClickEventArgs e)
        {
            GridView gv = sender as GridView;
            StackPanel sp = e.ClickedItem as StackPanel;
            int index = gv.Items.IndexOf(sp.Parent);
            var itemviewmodel = gv.DataContext as AlbumViewModel;

            if (itemviewmodel != null)
            {

                if (index == 2) //start radio
                {
                    var feed = await NewMain.Current.mc.GetStationFeed(ExplicitType.Explicit,
                        new StationFeedStation
                        {
                            LibraryContentOnly = false,
                            NumberOfEntries = -1,
                            RecentlyPlayed = new Track[0],
                            Seed = new StationSeed
                            {
                                SeedType = 4,//4
                            AlbumId = itemviewmodel.AlbumID
                            }
                        }
                    );

                    if (feed.Data.Stations[0].Tracks != null)
                    {

                        PlayerService.Instance.CurrentPlaylist.Clear();
                        PlayerService.Instance.previousSongIndex = 0;
                        PlayerService.Instance.currentSongIndex = 0;

                        foreach (Track track in feed.Data.Stations[0].Tracks)
                        {
                            if(track != null)
                                PlayerService.Instance.CurrentPlaylist.Add(track);
                        }

                        PlayerService.Instance.isRadioMode = true;
                        PlayerService.Instance.radioSeed = itemviewmodel.AlbumID;
                        PlayerService.Instance.radioType = "Album";
                        PlayerService.Instance.Player.Source = new MediaPlaybackItem(MediaSource.CreateFromUri(await GetStreamUrl(NewMain.Current.mc, feed.Data.Stations[0].Tracks[0])));
                        PlayerService.Instance.Player.Play();

                    }

                    itemviewmodel.showCheckMark(2);
                }

                else
                {

                    Album album = await NewMain.Current.mc.GetAlbumAsync(itemviewmodel.AlbumID);

                    if (index == 0) //Add to end of queue
                    {
                        foreach (Track track in album.Tracks)
                        {
                            if(track != null)
                                PlayerService.Instance.CurrentPlaylist.Add(track);
                        }

                        if (PlayerService.Instance.Player.Source == null)
                        {
                            PlayerService.Instance.Player.Source = new MediaPlaybackItem(MediaSource.CreateFromUri(await GetStreamUrl(NewMain.Current.mc, PlayerService.Instance.CurrentPlaylist[0])));
                            PlayerService.Instance.Player.Play();
                        }
                        PlayerService.Instance.isRadioMode = false;
                        itemviewmodel.showCheckMark(0);
                    }

                    else if (index == 1) //Clear queue and play
                    {
                        PlayerService.Instance.CurrentPlaylist.Clear();
                        PlayerService.Instance.previousSongIndex = 0;
                        PlayerService.Instance.currentSongIndex = 0;

                        foreach (Track track in album.Tracks)
                        {
                            if(track != null)
                                PlayerService.Instance.CurrentPlaylist.Add(track);
                        }
                        PlayerService.Instance.isRadioMode = false;
                        PlayerService.Instance.Player.Source = new MediaPlaybackItem(MediaSource.CreateFromUri(await GetStreamUrl(NewMain.Current.mc, PlayerService.Instance.CurrentPlaylist[0])));
                        PlayerService.Instance.Player.Play();

                        itemviewmodel.showCheckMark(1);
                    }
                }
            }

            else
            {
                MessageViewModel.MLViewModel.Add(new MessageItemViewModel("Null result found."));
                MessageViewModel.ShowAlert();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public AlbumViewModel(AlbumListViewModel alviewmodel, Album album, MessagingViewModel MessageViewModel, SolidColorBrush brush)
        {
            this.ALViewModel = alviewmodel;
            this.album = album;
            this.MessageViewModel = MessageViewModel;
            this.BorderBrush = brush;
            RaisePropertyChanged("Name");

            PreviewImage = new BitmapImage();

            if (album.AlbumArtRef != null)
                PreviewImage.UriSource = new Uri(album.AlbumArtRef);
            else
                PreviewImage.UriSource = new Uri("ms-appx:///Assets/no_image.png", UriKind.Absolute);

            MenuOpen = false;
            IsVisibleZero = Visibility.Collapsed;
            IsVisibleOne = Visibility.Collapsed;
            IsVisibleTwo = Visibility.Collapsed;
        }

        public static async Task<Uri> GetStreamUrl(MobileClient mc, Track track)
        {
            Uri data;

            data = await mc.GetStreamUrlAsync(track);

            return data;
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
