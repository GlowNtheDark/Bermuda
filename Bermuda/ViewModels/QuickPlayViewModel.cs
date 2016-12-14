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
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace Bermuda.ViewModels
{
    public class QuickPlayViewModel : INotifyPropertyChanged, IDisposable
    {
        MediaPlayer Player;
        CoreDispatcher dispatcher;
        QuickPlayListViewModel quickplaylistviewmodel;
        TrackList MediaList;

        public event PropertyChangedEventHandler PropertyChanged;

        public QuickPlayViewModel(MediaPlayer player, TrackList medialist, CoreDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
            this.Player = player;
            this.MediaList = medialist;
        }

        public QuickPlayListViewModel QuickPlayListViewModel
        {
            get { return quickplaylistviewmodel; }

            set
            {
                if (quickplaylistviewmodel != value)
                {
                    quickplaylistviewmodel = value;
                    RaisePropertyChanged("QuickPlayListViewModel");
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

            if (menuIndex == 0)
            {

                try
                {
                    if (item.Type == "1") //Album Listing
                    {

                        Album album = await getAlbum(NewMain.Current.mc, item.Album.Id.MetajamCompactKey.ToString());

                        foreach (Track track in album.Tracks)
                            MediaList.Add(track);

                        if (Player.Source == null)
                        {
                            Player.Source = new MediaPlaybackItem(MediaSource.CreateFromUri(await GetStreamUrl(NewMain.Current.mc, album.Tracks[0])));
                            Player.Play();
                        }

                    }

                    else if (item.Type == "3") //Radio Listing
                    {

                        if (item.RadioStation.Id.Seeds[0].SeedType.ToString() == "3")
                        {
                            RadioFeed feed = await getArtistRadioStation(NewMain.Current.mc, item.RadioStation.Id.Seeds[0].ArtistId);

                            MediaPlaybackList temp2 = new MediaPlaybackList();

                            if (feed.Data.Stations[0].Tracks != null)
                            {
                                foreach (Track track in feed.Data.Stations[0].Tracks)
                                    MediaList.Add(track);

                                if (Player.Source == null)
                                {
                                    Player.Source = new MediaPlaybackItem(MediaSource.CreateFromUri(await GetStreamUrl(NewMain.Current.mc, feed.Data.Stations[0].Tracks[0])));
                                    Player.Play();
                                }
                            }

                        }
                        else if (item.RadioStation.Id.Seeds[0].SeedType.ToString() == "5") // Genre Listing
                        {
                            RadioFeed feed = await getGenreRadioStation(NewMain.Current.mc, item.RadioStation.Id.Seeds[0].GenreId);

                            if (feed.Data.Stations[0].Tracks != null)
                            {
                                foreach (Track track in feed.Data.Stations[0].Tracks)
                                    MediaList.Add(track);

                                if (Player.Source == null)
                                {
                                    Player.Source = new MediaPlaybackItem(MediaSource.CreateFromUri(await GetStreamUrl(NewMain.Current.mc, feed.Data.Stations[0].Tracks[0])));
                                    Player.Play();
                                }
                            }
                        }
                    }
                }

                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Write(ex);
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

                    if (item.Type == "1") //Album Listing
                    {

                        Album album = await getAlbum(NewMain.Current.mc, item.Album.Id.MetajamCompactKey.ToString());

                        foreach (Track track in album.Tracks)
                            MediaList.Add(track);

                        Player.Source = new MediaPlaybackItem(MediaSource.CreateFromUri(await GetStreamUrl(NewMain.Current.mc, album.Tracks[0])));
                        Player.Play();

                    }

                    else if (item.Type == "3") //Radio Listing
                    {

                        if (item.RadioStation.Id.Seeds[0].SeedType.ToString() == "3")
                        {
                            RadioFeed feed = await getArtistRadioStation(NewMain.Current.mc, item.RadioStation.Id.Seeds[0].ArtistId);

                            MediaPlaybackList temp2 = new MediaPlaybackList();

                            if (feed.Data.Stations[0].Tracks != null)
                            {
                                foreach (Track track in feed.Data.Stations[0].Tracks)
                                    MediaList.Add(track);
                                Player.Source = new MediaPlaybackItem(MediaSource.CreateFromUri(await GetStreamUrl(NewMain.Current.mc, feed.Data.Stations[0].Tracks[0])));
                                Player.Play();
                            }

                        }
                        else if (item.RadioStation.Id.Seeds[0].SeedType.ToString() == "5") // Genre Listing
                        {
                            RadioFeed feed = await getGenreRadioStation(NewMain.Current.mc, item.RadioStation.Id.Seeds[0].GenreId);

                            if (feed.Data.Stations[0].Tracks != null)
                            {
                                foreach (Track track in feed.Data.Stations[0].Tracks)
                                    MediaList.Add(track);
                                Player.Source = new MediaPlaybackItem(MediaSource.CreateFromUri(await GetStreamUrl(NewMain.Current.mc, feed.Data.Stations[0].Tracks[0])));
                                Player.Play();
                            }
                        }
                    }
                }

                catch (Exception ex)
                {
                    System.Diagnostics.Debug.Write(ex);
                }

                itemviewmodel.showCheckMark(1);
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
                            NumberOfEntries = 50,
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
                            NumberOfEntries = 50,
                            RecentlyPlayed = new Track[0],
                            Seed = new StationSeed
                            {
                                SeedType = 6,
                                GenreId = genreId
                            }
                        }
                    );

            return data;
        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {

        }
    }
}
