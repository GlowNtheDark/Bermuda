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
    public class ArtistViewModel : INotifyPropertyChanged
    {
        public ArtistListViewModel ALViewModel;

        MessagingViewModel MessageViewModel;

        public Artist artist { get; private set; }

        public string Name => artist.Name;

        public string ArtistID => artist.ArtistId;

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

        public Visibility isvisiblezero;

        public Visibility isvisibleone;

        public Visibility isvisibletwo;

        public Visibility isvisiblethree;

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

        public Visibility IsVisibleThree
        {
            get { return isvisiblethree; }

            private set
            {
                if (isvisiblethree != value)
                {
                    isvisiblethree = value;
                    RaisePropertyChanged("IsVisibleThree");
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

            else if (index == 2)
            {
                IsVisibleTwo = Visibility.Visible;
                await Task.Delay(2000);
                IsVisibleTwo = Visibility.Collapsed;
            }

            else
            {
                IsVisibleThree = Visibility.Visible;
                await Task.Delay(2000);
                IsVisibleThree = Visibility.Collapsed;
            }

            MenuOpen = false;
        }

        public async void searchMenuItemClicked(object sender, ItemClickEventArgs e)
        {
            GridView gv = sender as GridView;
            StackPanel sp = e.ClickedItem as StackPanel;
            int index = gv.Items.IndexOf(sp.Parent);
            var itemviewmodel = gv.DataContext as ArtistViewModel;

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
                                SeedType = 3,
                                ArtistId = itemviewmodel.ArtistID
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
                            if (track != null)
                                PlayerService.Instance.CurrentPlaylist.Add(track);
                        }

                        PlayerService.Instance.isRadioMode = true;
                        PlayerService.Instance.radioSeed = itemviewmodel.ArtistID;
                        PlayerService.Instance.radioType = "Artist";
                        PlayerService.Instance.Player.Source = new MediaPlaybackItem(MediaSource.CreateFromUri(await GetStreamUrl(NewMain.Current.mc, feed.Data.Stations[0].Tracks[0])));
                        PlayerService.Instance.Player.Play();
                    }

                    itemviewmodel.showCheckMark(2);
                }

                else if (index == 3) //shuffle artist
                {
                    var feed = await NewMain.Current.mc.GetStationFeed(ExplicitType.Explicit,
                        new StationFeedStation
                        {
                            LibraryContentOnly = false,
                            NumberOfEntries = -1,
                            RecentlyPlayed = new Track[0],
                            Seed = new StationSeed
                            {
                                SeedType = 7,
                                ArtistId = itemviewmodel.ArtistID
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
                            if (track != null)
                                PlayerService.Instance.CurrentPlaylist.Add(track);
                        }

                        PlayerService.Instance.isRadioMode = false;

                        PlayerService.Instance.Player.Source = new MediaPlaybackItem(MediaSource.CreateFromUri(await GetStreamUrl(NewMain.Current.mc, feed.Data.Stations[0].Tracks[0])));
                        PlayerService.Instance.Player.Play();
                    }

                    itemviewmodel.showCheckMark(3);
                }

                else
                {
                    AlbumList albumlist = new AlbumList();
                    SearchResponse albumresponse = await NewMain.Current.mc.SearchAsync(itemviewmodel.Name, 3);

                    if (albumresponse.Entries != null)
                    {

                        foreach (SearchResult result in albumresponse.Entries)
                        {
                            if(result != null)
                                if (result.Album.ArtistId[0] == itemviewmodel.ArtistID)
                                    albumlist.Add(result.Album);
                        }

                        if (index == 0) //Add all to end of queue
                        {

                            if (PlayerService.Instance.Player.Source == null)
                            {
                                foreach (Album album in albumlist)
                                {
                                    if (album != null)
                                    {
                                        Album temp = await NewMain.Current.mc.GetAlbumAsync(album.AlbumId);

                                        if (temp != null)
                                        {
                                            foreach (Track track in temp.Tracks)
                                            {
                                                if (track != null)
                                                    PlayerService.Instance.CurrentPlaylist.Add(track);
                                            }
                                        }
                                    }
                                }
                                PlayerService.Instance.Player.Source = new MediaPlaybackItem(MediaSource.CreateFromUri(await GetStreamUrl(NewMain.Current.mc, PlayerService.Instance.CurrentPlaylist[0])));
                                PlayerService.Instance.Player.Play();
                            }

                            else if(PlayerService.Instance.isRadioMode)
                            {
                                PlayerService.Instance.CurrentPlaylist.Clear();
                                PlayerService.Instance.previousSongIndex = 0;
                                PlayerService.Instance.currentSongIndex = 0;
                                PlayerService.Instance.isRadioMode = false;

                                foreach (Album album in albumlist)
                                {
                                    if (album != null)
                                    {
                                        Album temp = await NewMain.Current.mc.GetAlbumAsync(album.AlbumId);

                                        if (temp != null)
                                        {
                                            foreach (Track track in temp.Tracks)
                                            {
                                                if (track != null)
                                                    PlayerService.Instance.CurrentPlaylist.Add(track);
                                            }
                                        }
                                    }
                                }

                                PlayerService.Instance.Player.Source = new MediaPlaybackItem(MediaSource.CreateFromUri(await GetStreamUrl(NewMain.Current.mc, PlayerService.Instance.CurrentPlaylist[0])));
                                PlayerService.Instance.Player.Play();
                            }

                            
                            itemviewmodel.showCheckMark(0);
                        }

                        else if (index == 1) //Clear queue and play all
                        {
                            PlayerService.Instance.CurrentPlaylist.Clear();
                            PlayerService.Instance.previousSongIndex = 0;
                            PlayerService.Instance.currentSongIndex = 0;

                            foreach (Album album in albumlist)
                            {
                                if (album != null)
                                {
                                    Album temp = await NewMain.Current.mc.GetAlbumAsync(album.AlbumId);

                                    if (temp != null)
                                    {
                                        foreach (Track track in temp.Tracks)
                                        {
                                            if(track != null)
                                                PlayerService.Instance.CurrentPlaylist.Add(track);
                                        }
                                    }
                                }
                            }
                            PlayerService.Instance.isRadioMode = false;
                            PlayerService.Instance.Player.Source = new MediaPlaybackItem(MediaSource.CreateFromUri(await GetStreamUrl(NewMain.Current.mc, PlayerService.Instance.CurrentPlaylist[0])));
                            PlayerService.Instance.Player.Play();

                            itemviewmodel.showCheckMark(1);
                        }
                    }

                    else
                    {
                        MessageViewModel.MLViewModel.Add(new MessageItemViewModel("Fetching albums came back null."));
                        MessageViewModel.ShowAlert();
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

        public ArtistViewModel(ArtistListViewModel alviewmodel, Artist artist, MessagingViewModel MessageViewModel, SolidColorBrush brush)
        {
            this.ALViewModel = alviewmodel;
            this.artist = artist;
            this.MessageViewModel = MessageViewModel;
            this.BorderBrush = brush;
            RaisePropertyChanged("Name");

            PreviewImage = new BitmapImage();

            if (artist.ArtistArtRef != null)
                PreviewImage.UriSource = new Uri(artist.ArtistArtRef);
            else
                PreviewImage.UriSource = new Uri("ms-appx:///Assets/no_image.png", UriKind.Absolute);

            MenuOpen = false;
            IsVisibleZero = Visibility.Collapsed;
            IsVisibleOne = Visibility.Collapsed;
            IsVisibleTwo = Visibility.Collapsed;
            IsVisibleThree = Visibility.Collapsed;
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
