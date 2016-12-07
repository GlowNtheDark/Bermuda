using Bermuda.DataModels;
using Bermuda.Services;
using Bermuda.ViewModels;
using GoogleMusicApi.UWP.Common;
using GoogleMusicApi.UWP.Requests.Data;
using GoogleMusicApi.UWP.Structure;
using GoogleMusicApi.UWP.Structure.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Bermuda
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SuggestionsScene : Page
    {
        public SuggestionsScene()
        {
            this.InitializeComponent();
        }

        private List<ListenNowItem> listenNowList = new List<ListenNowItem>();

        MediaPlayer Player => PlayerService.Instance.Player;

        MediaPlaybackItem PlaybackItem
        {
            get { return Player.Source as MediaPlaybackItem; }
            set { Player.Source = value; }
        }

        TrackList MediaList
        {
            get { return PlayerService.Instance.CurrentPlaylist; }
            set { PlayerService.Instance.CurrentPlaylist = value; }
        }

        private async void getListenNow()
        {

            try
            {
                if (recentsGridView.Items.Any())
                    recentsGridView.Items.Clear();

                ListListenNowTracksResponse listenNowResult = await NewMain.Current.mc.ListListenNowTracksAsync();

                if (listenNowResult != null)
                {

                    foreach (ListenNowItem item in listenNowResult.Items)
                    {
                        int index = 0;

                        listenNowList.Add(item);

                        if (item.Type == "1")
                        {
                            if (item.Images != null)
                                create_ListenNow(index, item.Album.Id.Title, item.Album.Id.Artist, item.Images[0].Url);
                            else
                                create_ListenNow(index, item.Album.Id.Title, item.Album.Id.Artist, "ms-appx:///Assets/no_image.png");
                        }

                        else if (item.Type == "3")
                        {
                            if (item.Images != null)
                                create_ListenNow(index, item.RadioStation.Title, item.Images[0].Url);
                            else
                                create_ListenNow(index, item.RadioStation.Title, "ms-appx:///Assets/no_image.png");
                        }

                        index++;
                    }

                    recentsGridView.IsItemClickEnabled = true;
                    recentsGridView.ItemClick += new ItemClickEventHandler(GridView_ItemClick);
                }

                else
                {
                    
                }
            }

            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);

            }
        }

        private async void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            int index = this.recentsGridView.Items.IndexOf(e.ClickedItem);

            try
            {
                if (listenNowList[index].Type == "1") //Album Listing
                {

                    Album album = await getAlbum(NewMain.Current.mc, listenNowList[index].Album.Id.MetajamCompactKey.ToString());

                    foreach (Track track in album.Tracks)
                        MediaList.Add(track);

                    PlaybackItem = new MediaPlaybackItem(MediaSource.CreateFromUri(await GetStreamUrl(NewMain.Current.mc, album.Tracks[0])));
                    Player.Play();

                }

                else if (listenNowList[index].Type == "3") //Radio Listing
                {

                    if (listenNowList[index].RadioStation.Id.Seeds[0].SeedType.ToString() == "3")
                    {
                        RadioFeed feed = await getArtistRadioStation(NewMain.Current.mc, listenNowList[index].RadioStation.Id.Seeds[0].ArtistId);

                        MediaPlaybackList temp2 = new MediaPlaybackList();

                        if (feed.Data.Stations[0].Tracks != null)
                        {
                            foreach(Track track in feed.Data.Stations[0].Tracks)
                                MediaList.Add(track);
                            PlaybackItem = new MediaPlaybackItem(MediaSource.CreateFromUri(await GetStreamUrl(NewMain.Current.mc, feed.Data.Stations[0].Tracks[0])));
                            Player.Play();
                        }

                    }
                    else if (listenNowList[index].RadioStation.Id.Seeds[0].SeedType.ToString() == "5") // Genre Listing
                    {
                        RadioFeed feed = await getGenreRadioStation(NewMain.Current.mc, listenNowList[index].RadioStation.Id.Seeds[0].GenreId);

                        if (feed.Data.Stations[0].Tracks != null)
                        {
                            foreach (Track track in feed.Data.Stations[0].Tracks)
                                MediaList.Add(track);
                            PlaybackItem = new MediaPlaybackItem(MediaSource.CreateFromUri(await GetStreamUrl(NewMain.Current.mc, feed.Data.Stations[0].Tracks[0])));
                            Player.Play();
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }

        }

        public static async Task<Uri> GetStreamUrl(MobileClient mc, Track track)
        {
            Uri data;

            data = await mc.GetStreamUrlAsync(track);

            return data;
        }

        private void create_ListenNow(int index, string Title, string Artist, string imagePath)
        {
            var img = new Windows.UI.Xaml.Controls.Image();
            var grid = new Grid();
            var textBlock = new TextBlock();

            // Create column definitions for first grid
            ColumnDefinition columnDefinition1 = new ColumnDefinition();
            columnDefinition1.Width = new GridLength(200, GridUnitType.Pixel);

            // Create row definitions for second grid
            RowDefinition rowDefinition1 = new RowDefinition();
            RowDefinition rowDefinition2 = new RowDefinition();
            rowDefinition1.Height = new GridLength(1, GridUnitType.Star);
            rowDefinition2.Height = new GridLength(50, GridUnitType.Pixel);

            // Attached definitions to grids
            grid.ColumnDefinitions.Add(columnDefinition1);
            grid.RowDefinitions.Add(rowDefinition1);
            grid.RowDefinitions.Add(rowDefinition2);
            if (imagePath != null)
                img.Source = new BitmapImage(new Uri(imagePath, UriKind.Absolute));
            else
                img.Source = new BitmapImage(new Uri("ms-appx:///Assets/no_image.png", UriKind.Absolute));

            img.Stretch = Stretch.Fill;
            img.Width = Double.NaN;
            img.Height = Double.NaN;
            img.Margin = new Thickness(0, 0, 0, 0);
            img.Tag = index.ToString();
            //img.PointerPressed += new PointerEventHandler(image_onClick);

            textBlock.Text = Title + "\n" + Artist;
            textBlock.FontSize = 14;
            textBlock.Foreground = new SolidColorBrush(Colors.White);
            textBlock.HorizontalAlignment = HorizontalAlignment.Left;
            textBlock.VerticalAlignment = VerticalAlignment.Center;
            textBlock.Margin = new Thickness(0, 0, 0, 0);

            grid.Margin = new Thickness(0, 0, 5, 0);

            grid.Children.Add(textBlock);
            grid.Children.Add(img);

            Grid.SetColumn(img, 0);
            Grid.SetColumn(textBlock, 0);
            Grid.SetRow(img, 0);
            Grid.SetRow(textBlock, 1);

            recentsGridView.Items.Add(grid);
        }

        private void create_ListenNow(int index, string Title, string imagePath)
        {
            var img = new Windows.UI.Xaml.Controls.Image();
            var grid = new Grid();
            var textBlock = new TextBlock();

            // Create column definitions for first grid
            ColumnDefinition columnDefinition1 = new ColumnDefinition();
            columnDefinition1.Width = new GridLength(200, GridUnitType.Pixel);

            // Create row definitions for second grid
            RowDefinition rowDefinition1 = new RowDefinition();
            RowDefinition rowDefinition2 = new RowDefinition();
            rowDefinition1.Height = new GridLength(1, GridUnitType.Star);
            rowDefinition2.Height = new GridLength(50, GridUnitType.Pixel);

            // Attached definitions to grids
            grid.ColumnDefinitions.Add(columnDefinition1);
            grid.RowDefinitions.Add(rowDefinition1);
            grid.RowDefinitions.Add(rowDefinition2);

            if (imagePath != null)
                img.Source = new BitmapImage(new Uri(imagePath, UriKind.Absolute));
            else
                img.Source = new BitmapImage(new Uri("ms-appx:///Assets/no_image.png", UriKind.Absolute));

            img.Stretch = Stretch.Fill;
            img.Width = Double.NaN;
            img.Height = Double.NaN;
            img.Margin = new Thickness(0, 0, 0, 0);
            img.Tag = index.ToString();

            textBlock.Text = Title + " Radio";
            textBlock.FontSize = 14;
            textBlock.Foreground = new SolidColorBrush(Colors.White);
            textBlock.HorizontalAlignment = HorizontalAlignment.Left;
            textBlock.VerticalAlignment = VerticalAlignment.Center;
            textBlock.Margin = new Thickness(0, 0, 0, 0);

            grid.Margin = new Thickness(0, 0, 5, 0);

            grid.Children.Add(textBlock);
            grid.Children.Add(img);

            Grid.SetColumn(img, 0);
            Grid.SetColumn(textBlock, 0);
            Grid.SetRow(img, 0);
            Grid.SetRow(textBlock, 1);

            recentsGridView.Items.Add(grid);
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

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            listenNowList = null;
            GC.Collect();
            AppSettings.localSettings.Values["lastPage"] = "Suggestions";
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            getListenNow();
        }
    }
}
