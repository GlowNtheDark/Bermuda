using Bermuda.Services;
using GoogleMusicApi.UWP.Common;
using GoogleMusicApi.UWP.Requests.Data;
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
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Bermuda.ViewModels 
{
    public class TrackViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public TrackListViewModel listViewModel;

        public Track song { get; private set; }

        public Playlist playlist { get; private set; }

        public string Title => song.Title;

        public string Artist => song.Artist;

        public string Album => song.Album;

        public SolidColorBrush tileColor => song.tileColor;

        public double currentSongDuration;

        public bool menuOpen;

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

        public string TrackId => song.Nid;

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

            else if (index == 1)
            {
                IsVisibleOne = Visibility.Visible;
                await Task.Delay(3000);
                IsVisibleOne = Visibility.Collapsed;
            }

            else
            {
                IsVisibleTwo = Visibility.Visible;
                await Task.Delay(3000);
                IsVisibleTwo = Visibility.Collapsed;
            }

            MenuOpen = false;
        }

        public TrackViewModel(TrackListViewModel trackViewModel, Track song)
        {
            this.listViewModel = trackViewModel;
            this.song = song;

            RaisePropertyChanged("Title");

            PreviewImage = new BitmapImage();

            if (song.AlbumArtReference != null)
                PreviewImage.UriSource = new Uri(song.AlbumArtReference[0].Url);

            CurrentSongDuration = song.DurationMillis;

            MenuOpen = false;
            IsVisibleZero = Visibility.Collapsed;
            IsVisibleOne = Visibility.Collapsed;
            IsVisibleTwo = Visibility.Collapsed;
        }

        public TrackViewModel(TrackListViewModel trackViewModel, Track song, Playlist playlist)
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

            this.playlist = playlist;
        }

        public async void menuItemClicked(object sender, ItemClickEventArgs e)
        {
            GridView gv = sender as GridView;
            StackPanel sp = e.ClickedItem as StackPanel;
            int index = gv.Items.IndexOf(sp.Parent);
            var itemviewmodel = gv.DataContext as TrackViewModel;

            if(index == 0) //Clear queue and play
            {
                PlayerService.Instance.CurrentPlaylist.Clear();
                PlayerService.Instance.CurrentPlaylist.Add(itemviewmodel.song);
                PlayerService.Instance.Player.Source = new MediaPlaybackItem(MediaSource.CreateFromUri(await GetStreamUrl(NewMain.Current.mc, PlayerService.Instance.CurrentPlaylist[0])));
                PlayerService.Instance.Player.Play();
            }

            else if(index == 1) //Add to end of queue
            {
                PlayerService.Instance.CurrentPlaylist.Add(itemviewmodel.song);

                if (PlayerService.Instance.Player.Source == null)
                {
                    PlayerService.Instance.Player.Source = new MediaPlaybackItem(MediaSource.CreateFromUri(await GetStreamUrl(NewMain.Current.mc, PlayerService.Instance.CurrentPlaylist[0])));
                    PlayerService.Instance.Player.Play();
                }
            }

            else // Delete song from playlist
            {
                Plentry plentry = NewMain.Current.mc.GetTrackPlaylistEntry(itemviewmodel.playlist, itemviewmodel.song);
                MutateResponse response = await NewMain.Current.mc.RemoveSongsFromPlaylist(plentry);
                itemviewmodel.listViewModel.Remove(itemviewmodel);
            }

            MenuOpen = false;
        }

        public async void searchMenuItemClicked(object sender, ItemClickEventArgs e)
        {
            GridView gv = sender as GridView;
            StackPanel sp = e.ClickedItem as StackPanel;
            int index = gv.Items.IndexOf(sp.Parent);
            var itemviewmodel = gv.DataContext as TrackViewModel;
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
                            SeedType = 1,//1
                            TrackId = itemviewmodel.TrackId
                        }
                    }
                );

                if (feed.Data.Stations[0].Tracks != null)
                {

                    PlayerService.Instance.CurrentPlaylist.Clear();
                    PlayerService.Instance.previousSongIndex = 0;
                    PlayerService.Instance.currentSongIndex = 0;

                    foreach (Track track in feed.Data.Stations[0].Tracks)
                        PlayerService.Instance.CurrentPlaylist.Add(track);

                    PlayerService.Instance.Player.Source = new MediaPlaybackItem(MediaSource.CreateFromUri(await GetStreamUrl(NewMain.Current.mc, feed.Data.Stations[0].Tracks[0])));
                    PlayerService.Instance.Player.Play();

                }

                itemviewmodel.showCheckMark(2);
            }

            else
            {
                if (index == 0) //Add to end of queue
                {
                    PlayerService.Instance.CurrentPlaylist.Add(itemviewmodel.song);

                    if (PlayerService.Instance.Player.Source == null)
                    {
                        PlayerService.Instance.Player.Source = new MediaPlaybackItem(MediaSource.CreateFromUri(await GetStreamUrl(NewMain.Current.mc, PlayerService.Instance.CurrentPlaylist[0])));
                        PlayerService.Instance.Player.Play();
                    }

                    itemviewmodel.showCheckMark(0);
                }

                else if (index == 1) //Clear queue and play
                {
                    PlayerService.Instance.CurrentPlaylist.Clear();
                    PlayerService.Instance.previousSongIndex = 0;
                    PlayerService.Instance.currentSongIndex = 0;
                    PlayerService.Instance.CurrentPlaylist.Add(itemviewmodel.song);
                    PlayerService.Instance.Player.Source = new MediaPlaybackItem(MediaSource.CreateFromUri(await GetStreamUrl(NewMain.Current.mc, PlayerService.Instance.CurrentPlaylist[0])));
                    PlayerService.Instance.Player.Play();

                    itemviewmodel.showCheckMark(1);
                }
            }
        }

        public async void setTileColorDefault()
        {
            await listViewModel.dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                song.tileColor = new SolidColorBrush(Colors.Transparent);
                RaisePropertyChanged("tileColor");
            });
        }

        public async void Update()
        {
            await listViewModel.dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                RaisePropertyChanged("tileColor");
            });
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
