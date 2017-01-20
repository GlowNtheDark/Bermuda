using Bermuda.DataModels;
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
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace Bermuda.ViewModels
{
    public class QuickPlayViewModel : INotifyPropertyChanged, IDisposable
    {
        MediaPlayer Player;
        CoreDispatcher dispatcher;
        QuickPlayRadioViewModel qpradioviewmodel;
        QuickPlayAlbumViewModel qpalbumviewmodel;
        MessagingViewModel MessageViewModel;
        TrackList MediaList;
        public ColorListViewModel colorlistviewmodel;

        public event PropertyChangedEventHandler PropertyChanged;

        public QuickPlayViewModel(MediaPlayer player, TrackList medialist, CoreDispatcher dispatcher, MessagingViewModel MessageViewModel)
        {
            this.dispatcher = dispatcher;
            this.Player = player;
            this.MediaList = medialist;
            this.MessageViewModel = MessageViewModel;
            colorlistviewmodel = new ColorListViewModel();
        }

        public QuickPlayRadioViewModel QPRadioViewModel
        {
            get { return qpradioviewmodel; }

            set
            {
                if (qpradioviewmodel != value)
                {
                    qpradioviewmodel = value;
                    RaisePropertyChanged("QPRadioViewModel");
                }
            }
        }

        public QuickPlayAlbumViewModel QPAlbumViewModel
        {
            get { return qpalbumviewmodel; }

            set
            {
                if (qpalbumviewmodel != value)
                {
                    qpalbumviewmodel = value;
                    RaisePropertyChanged("QPAlbumViewModel");
                }
            }
        }

        public void ItemClick(object sender, ItemClickEventArgs e)
        {
            ListenNowItemViewModel item = e.ClickedItem as ListenNowItemViewModel;
            item.openCloseMenu();
            RaisePropertyChanged("QuickPlayListViewModel");
        }

        public async void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            GridView gv = sender as GridView;
            StackPanel sp = e.ClickedItem as StackPanel;

            int menuIndex = gv.Items.IndexOf(sp.Parent);
            var itemviewmodel = gv.DataContext as ListenNowItemViewModel;
            var item = itemviewmodel.item;

            if (item != null)
            {
                //Album Item
                if (item.Type == "1") //Album Listing
                {
                    if (menuIndex == 0) // Add to queue
                    {

                        try
                        {
                            Album album = await getAlbum(NewMain.Current.mc, item.Album.Id.MetajamCompactKey.ToString());

                            if (Player.Source == null)
                            {
                                foreach (Track track in album.Tracks)
                                {
                                    if (track != null)
                                        MediaList.Add(track);
                                }

                                Player.Source = new MediaPlaybackItem(MediaSource.CreateFromUri(await GetStreamUrl(NewMain.Current.mc, album.Tracks[0])));
                                Player.Play();
                            }

                            else if (PlayerService.Instance.isRadioMode)
                            {
                                PlayerService.Instance.CurrentPlaylist.Clear();
                                PlayerService.Instance.previousSongIndex = 0;
                                PlayerService.Instance.currentSongIndex = 0;
                                PlayerService.Instance.isRadioMode = false;

                                foreach (Track track in album.Tracks)
                                {
                                    if (track != null)
                                        MediaList.Add(track);
                                }

                                PlayerService.Instance.Player.Source = new MediaPlaybackItem(MediaSource.CreateFromUri(await GetStreamUrl(NewMain.Current.mc, PlayerService.Instance.CurrentPlaylist[0])));
                                PlayerService.Instance.Player.Play();
                            }
                        }

                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.Write(ex);
                            MessageViewModel.MLViewModel.Add(new MessageItemViewModel("Unexpected error -- " + ex));
                            MessageViewModel.ShowAlert();
                        }

                        itemviewmodel.showCheckMark(0);
                    }

                    else
                    {
                        try
                        {
                            MediaList.Clear();
                            PlayerService.Instance.previousSongIndex = 0;
                            PlayerService.Instance.currentSongIndex = 0;

                            Album album = await getAlbum(NewMain.Current.mc, item.Album.Id.MetajamCompactKey.ToString());

                            foreach (Track track in album.Tracks)
                            {
                                if (track != null)
                                    MediaList.Add(track);
                            }

                            PlayerService.Instance.isRadioMode = false;
                            Player.Source = new MediaPlaybackItem(MediaSource.CreateFromUri(await GetStreamUrl(NewMain.Current.mc, album.Tracks[0])));
                            Player.Play();
                        }

                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.Write(ex);
                            MessageViewModel.MLViewModel.Add(new MessageItemViewModel("Unexpected error -- " + ex));
                            MessageViewModel.ShowAlert();
                        }

                        itemviewmodel.showCheckMark(1);
                    }
                }

                //Radio item
                else
                {
                    MediaList.Clear();
                    PlayerService.Instance.previousSongIndex = 0;
                    PlayerService.Instance.currentSongIndex = 0;


                    if (item.RadioStation.Id.Seeds[0].SeedType.ToString() == "3")
                    {
                        RadioFeed feed = await getArtistRadioStation(NewMain.Current.mc, item.RadioStation.Id.Seeds[0].ArtistId);

                        if (feed.Data.Stations[0].Tracks != null)
                        {
                            foreach (Track track in feed.Data.Stations[0].Tracks)
                            {
                                if (track != null)
                                    MediaList.Add(track);
                            }

                            PlayerService.Instance.isRadioMode = true;
                            PlayerService.Instance.radioSeed = item.RadioStation.Id.Seeds[0].ArtistId;
                            PlayerService.Instance.radioType = "Artist";
                            Player.Source = new MediaPlaybackItem(MediaSource.CreateFromUri(await GetStreamUrl(NewMain.Current.mc, feed.Data.Stations[0].Tracks[0])));
                            Player.Play();                          
                        }

                    }

                    else // Genre Listing
                    {
                        RadioFeed feed = await getGenreRadioStation(NewMain.Current.mc, item.RadioStation.Id.Seeds[0].GenreId);

                        if (feed.Data.Stations[0].Tracks != null)
                        {
                            foreach (Track track in feed.Data.Stations[0].Tracks)
                            {
                                if (track != null)
                                    MediaList.Add(track);
                            }

                            PlayerService.Instance.isRadioMode = true;
                            PlayerService.Instance.radioSeed = item.RadioStation.Id.Seeds[0].GenreId;
                            PlayerService.Instance.radioType = "Genre";
                            Player.Source = new MediaPlaybackItem(MediaSource.CreateFromUri(await GetStreamUrl(NewMain.Current.mc, feed.Data.Stations[0].Tracks[0])));
                            Player.Play();
                            
                        }
                    }

                    itemviewmodel.showCheckMark(0);
                }
            }
            else
            {
                MessageViewModel.MLViewModel.Add(new MessageItemViewModel("Clicked Item was null."));
                MessageViewModel.ShowAlert();
            }
        }

        public static async Task<Uri> GetStreamUrl(MobileClient mc, Track track)
        {
            Uri data;

            data = await mc.GetStreamUrlAsync(track);

            return data;
        }

        public async Task<Album> getAlbum(MobileClient mc, string albumId)
        {
            Album data;
            data = await mc.GetAlbumAsync(albumId);
            return data;
        }

        private async Task<RadioFeed> getArtistRadioStation(MobileClient mc, String artistId)
        {
            var data = await mc.GetStationFeed(ExplicitType.Explicit,
                        new StationFeedStation
                        {
                            LibraryContentOnly = false,
                            NumberOfEntries = -1,
                            RecentlyPlayed = new Track[0],
                            Seed = new StationSeed
                            {
                                SeedType = 3,
                                ArtistId = artistId
                            }
                        }
                    );

            return data;
        }

        private async Task<RadioFeed> getGenreRadioStation(MobileClient mc, String genreId)
        {
            var data = await mc.GetStationFeed(ExplicitType.Explicit,
                        new StationFeedStation
                        {
                            LibraryContentOnly = false,
                            NumberOfEntries = -1,
                            RecentlyPlayed = new Track[0],
                            Seed = new StationSeed
                            {
                                SeedType = 5,
                                GenreId = genreId
                            }
                        }
                    );

            return data;
        }

        public async void getListenNow()
        {

            try
            {
                ListListenNowTracksResponse listenNowResult = await NewMain.Current.mc.ListListenNowTracksAsync();

                if (listenNowResult != null)
                {
                    foreach (var item in listenNowResult.Items)
                    {
                        if (item != null)
                        {
                            if (item.Type == "1") //Album
                                QPAlbumViewModel.Add(new ListenNowItemViewModel(item, this, this.MessageViewModel, colorlistviewmodel[colorlistviewmodel.index].Color));
                            else // Radio
                                QPRadioViewModel.Add(new ListenNowItemViewModel(item, this, this.MessageViewModel, colorlistviewmodel[colorlistviewmodel.index].Color));
                        }
                    }
                }

                else
                {
                    MessageViewModel.MLViewModel.Add(new MessageItemViewModel("Quickplay request failed. Null result."));
                    MessageViewModel.ShowAlert();
                }
            }

            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
                MessageViewModel.MLViewModel.Add(new MessageItemViewModel("Unexpected error -- " + ex));
                MessageViewModel.ShowAlert();
            }
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            MediaList = null;
            MessageViewModel = null;
            QPRadioViewModel.Dispose();
            QPAlbumViewModel.Dispose();
            QPRadioViewModel = null;
            QPAlbumViewModel = null;
            colorlistviewmodel = null;
        }
    }
}
