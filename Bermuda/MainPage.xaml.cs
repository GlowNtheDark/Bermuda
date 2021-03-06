﻿using System;
using System.Linq;
using Windows.UI;
using Windows.Media;
using Windows.Media.Playback;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using GoogleMusicApi.UWP.Common;
using GoogleMusicApi.UWP.Structure;
using GoogleMusicApi.UWP.Structure.Enums;
using System.Threading.Tasks;
using Windows.Media.Core;
using System.Collections.Generic;
using Windows.Storage.Streams;
using GoogleMusicApi.UWP.Requests.Data;
using Windows.Foundation.Metadata;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls.Primitives;
using System.Threading;
using Windows.ApplicationModel.Background;
using Windows.Storage;
using Windows.UI.Core;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Bermuda
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();

            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.ApplicationView"))
            {
                var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                if (titleBar != null)
                {
                    titleBar.ButtonBackgroundColor = Colors.Black;
                    titleBar.ButtonForegroundColor = Colors.White;
                    titleBar.BackgroundColor = Colors.Black;
                    titleBar.ForegroundColor = Colors.White;
                }
            }



            if (!checkForBGTask(networkBackgroundTask))
                createNetworkAwarenessTask(networkBackgroundTask);
            else
                reregisterNetworkAwarenessTask(networkBackgroundTask);

            if (!checkForBGTask(servicingBackgroundTask))
                createServicingTask(servicingBackgroundTask);
            else
                reregisterServicingTask(servicingBackgroundTask);

        }

        private MobileClient mc;
        private SystemMediaTransportControls systemMediaTransportControls;
        private DispatcherTimer songTimer = new DispatcherTimer();
        private MediaPlayer player = new MediaPlayer();
        private List<Track> tracklist = new List<Track>();
        private List<Album> albumlist = new List<Album>();
        private List<Artist> artistList = new List<Artist>();
        private List<Album> artistAlbumList = new List<Album>();
        private List<ListenNowItem> listenNowList = new List<ListenNowItem>();
        private int trackPageNumber = 0;
        private int albumPageNumber = 0;
        private int artistPageNumber = 0;
        private bool isFirstPlaySinceOpen = true;
        private CancellationTokenSource cts = null;
        private CancellationTokenSource cts2 = null;
        private string lastNetworkState;
        private string networkBackgroundTask = "Network-Awareness-Task";
        private string servicingBackgroundTask = "Servicing-Complete-Task";
        public NowPlaying np = new NowPlaying();
       
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

        private async Task<RadioFeed> getAlbumRadioStation(MobileClient mc, String albumId)
        {
            var data = await mc.GetStationFeed(ExplicitType.Explicit,
                        new StationFeedStation
                        {
                            LibraryContentOnly = false,
                            NumberOfEntries = 50,
                            RecentlyPlayed = new Track[0],
                            Seed = new StationSeed
                            {
                                SeedType = 4,
                                AlbumId = albumId
                            }
                        }
                    );

            return data;
        }

        private async Task<RadioFeed> getTrackRadioStation(MobileClient mc, String trackId)
        {
            var data = await mc.GetStationFeed(ExplicitType.Explicit,
                        new StationFeedStation
                        {
                            LibraryContentOnly = false,
                            NumberOfEntries = 50,
                            RecentlyPlayed = new Track[0],
                            Seed = new StationSeed
                            {
                                SeedType = 1,
                                TrackId = trackId
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

        private void PlaybackSession_PlaybackStateChanged(MediaPlaybackSession sender, object args)
        {
            switch (player.PlaybackSession.PlaybackState)
            {
                case MediaPlaybackState.Playing:
                    systemMediaTransportControls.PlaybackStatus = MediaPlaybackStatus.Playing;
                    break;
                case MediaPlaybackState.Paused:
                    systemMediaTransportControls.PlaybackStatus = MediaPlaybackStatus.Paused;
                    break;
                case MediaPlaybackState.None:
                    systemMediaTransportControls.PlaybackStatus = MediaPlaybackStatus.Closed;
                    break;
            }
        }

        private async void SystemMediaTransportControls_ButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            switch (args.Button)
            {
                case SystemMediaTransportControlsButton.Play:
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        playButton.Visibility = Visibility.Collapsed;
                        pauseButton.Visibility = Visibility.Visible;
                        if (np.isSongEnded)
                            playSong(np.Songs[np.currentSongIndex]);
                        else
                            player.Play();
                    });
                    break;
                case SystemMediaTransportControlsButton.Pause:
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        pauseButton.Visibility = Visibility.Collapsed;
                        playButton.Visibility = Visibility.Visible;
                        player.Pause();
                    });
                    break;
                case SystemMediaTransportControlsButton.Next:
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        if (np.currentSongIndex < np.Songs.Count() - 1)
                        {
                            playSong(np.GetNextSong());
                        }
                    });
                    break;
                case SystemMediaTransportControlsButton.Previous:
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        if (np.currentSongIndex > 0)
                        {
                            playSong(np.GetPreviousSong());
                        }
                        else
                            player.PlaybackSession.Position = TimeSpan.Zero;
                    });
                    break;
                default:
                    break;
            }
        }

        private async void getListenNow()
        {
            listenNowProgressRing.IsActive = true;
            cts2 = new CancellationTokenSource();

            await Task.Delay(3000);

            try
            {
                if (recentsGridView.Items.Any())
                    recentsGridView.Items.Clear();
                    
                ListListenNowTracksResponse listenNowResult = await mc.ListListenNowTracksAsync();

                if (listenNowResult != null)
                {

                    foreach (ListenNowItem item in listenNowResult.Items)
                    {
                        int index = 0;

                        listenNowList.Add(item);

                        if (item.Type == "1")
                        {
                            if(item.Images != null)
                                create_ListenNow(index, item.Album.Id.Title, item.Album.Id.Artist, item.Images[0].Url);
                            else
                                create_ListenNow(index, item.Album.Id.Title, item.Album.Id.Artist, "ms-appx:///Assets/no_image.png");
                        }

                        else if (item.Type == "3")
                        {
                            if(item.Images != null)
                                create_ListenNow(index, item.RadioStation.Title, item.Images[0].Url);
                            else
                                create_ListenNow(index, item.RadioStation.Title, "ms-appx:///Assets/no_image.png");
                        }

                        index++;
                    }

                    recentsGridView.IsItemClickEnabled = true;
                    recentsGridView.ItemClick += new ItemClickEventHandler(GridView_ItemClick);
                    listenNowProgressRing.IsActive = false;
                }

                else
                {
                    generalFlyout.Text = "Suggestions was null. Try reloading the app.";
                    FlyoutBase.ShowAttachedFlyout(appTitleTextBox);
                }
            }

            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
                generalFlyout.Text = "Something went wrong with your request.";
                FlyoutBase.ShowAttachedFlyout(appTitleTextBox);
            }
        }

        private async void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            int index = this.recentsGridView.Items.IndexOf(e.ClickedItem);

            try
            {
                if (listenNowList[index].Type == "1") //Album Listing
                {
                    if(np.Songs.Any())
                        np.Songs.Clear();
                    np.currentSongIndex = 0;

                    Album album = await getAlbum(mc, listenNowList[index].Album.Id.MetajamCompactKey.ToString());

                    if (album.Tracks != null)
                    {

                        np.PopulateSongs(album.Tracks);

                        mainPivotMenu.SelectedIndex = 2;

                        playCurrentPlaylist(np.currentSongIndex);
                    }

                }

                else if (listenNowList[index].Type == "3") //Radio Listing
                {
                    if (np.Songs != null)
                        np.Songs.Clear();
                    np.currentSongIndex = 0;

                    if (listenNowList[index].RadioStation.Id.Seeds[0].SeedType.ToString() == "3")
                    {
                        RadioFeed feed = await getArtistRadioStation(mc, listenNowList[index].RadioStation.Id.Seeds[0].ArtistId);

                        if (feed.Data.Stations[0].Tracks != null)
                        {
                            np.PopulateSongs(feed.Data.Stations[0].Tracks);

                            mainPivotMenu.SelectedIndex = 2;

                            playCurrentPlaylist(np.currentSongIndex);
                        }

                    }
                    else if (listenNowList[index].RadioStation.Id.Seeds[0].SeedType.ToString() == "5")
                    {
                        RadioFeed feed = await getGenreRadioStation(mc, listenNowList[index].RadioStation.Id.Seeds[0].GenreId);


                        if (feed.Data.Stations[0].Tracks != null)
                        {
                            np.PopulateSongs(feed.Data.Stations[0].Tracks);

                            mainPivotMenu.SelectedIndex = 2;

                            playCurrentPlaylist(np.currentSongIndex);
                        }
                    }
                }
            }

            catch(Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
                generalFlyout.Text = "Something went wrong with your request.";
                FlyoutBase.ShowAttachedFlyout(appTitleTextBox);
            }
            
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
            if(imagePath != null)
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

        private async void searchBox_QuerySubmitted_1(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (mc != null)
            {
                string query = args.QueryText; //Get search text
                SearchResponse response;
                if (cts != null)
                {
                    cts.Cancel();
                    cts.Dispose();
                    cts = null;
                    cts = new CancellationTokenSource();
                }
                else
                    cts = new CancellationTokenSource();

                //clear tracklist for new search
                if (tracklist.Any())
                    tracklist.Clear();
                //clear albumlist for new search
                if (albumlist.Any())
                    albumlist.Clear();
                //clear albumlist for new search
                if (artistList.Any())
                    artistList.Clear();

                //Clear Search list boxes
                if (searchSongList.Items.Any())
                {
                    searchSongList.Items.Clear();
                    tracklistNextBtn.Visibility = Visibility.Collapsed;
                    tracklistPrevBtn.Visibility = Visibility.Collapsed;
                }

                if (searchAlbumList.Items.Any())
                {
                    searchAlbumList.Items.Clear();
                    albumlistNextBtn.Visibility = Visibility.Collapsed;
                    albumlistPrevBtn.Visibility = Visibility.Collapsed;
                }

                if (searchArtistList.Items.Any())
                {
                    searchArtistList.Items.Clear();
                    artistlistNextBtn.Visibility = Visibility.Collapsed;
                    artistlistPrevBtn.Visibility = Visibility.Collapsed;
                }

                response = await Search(query);

                if (response != null)
                {
                    try
                    {
                        parseAlbums(response, cts.Token);
                        parseTracks(response, cts.Token);
                        parseArtists(response, cts.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        System.Diagnostics.Debug.WriteLine("Sorting Cancelled.");
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine(e);
                    }
                }
            }

            else
            {
                FlyoutBase.ShowAttachedFlyout(appTitleTextBox);
            }
        }

        async void parseTracks(SearchResponse response, CancellationToken ct)
        {
            int trackListCount = 0;
            trackListProgressRing.IsActive = true;

            //Parse search results into lists
            for (int i = 0; i < response.Entries.Count(); i++)
            {

                if (response.Entries.ElementAt(i).Type == "1") //Tracks
                {
                    try
                    {
                        tracklist.Add(await getTrack(mc, response.Entries.ElementAt(i).Track.Nid.ToString(), ct));
                    }

                    catch (OperationCanceledException)
                    {
                        System.Diagnostics.Debug.WriteLine("Operation cancelled! -- Tracks");
                        break;
                    }
                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine(e);
                        break;
                    }

                    if (trackListCount < 10)
                    {
                        searchSongList.Items.Add(createsearchElement(tracklist[trackListCount], trackListCount));
                        trackListCount++;
                    }
                }
            }

            if (tracklist.Count > 10)
            {
                tracklistNextBtn.Visibility = Visibility.Visible;
                tracklistPrevBtn.Visibility = Visibility.Visible;
                
            }

            trackListProgressRing.IsActive = false;
        }

        async void parseAlbums(SearchResponse response, CancellationToken ct)
        {
            int albumListCount = 0;
            albumListProgressRing.IsActive = true;

            //Parse search results into lists
            for (int i = 0; i < response.Entries.Count(); i++)
            {

                if (response.Entries.ElementAt(i).Type == "3")
                {

                    try
                    {
                        albumlist.Add(await getAlbum(mc, response.Entries.ElementAt(i).Album.AlbumId.ToString(), ct));
                    }


                    catch (OperationCanceledException)
                    {
                        System.Diagnostics.Debug.WriteLine("Operation cancelled! -- Albums");
                        break;
                    }

                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine(e);
                        break;
                    }

                    if (albumListCount < 10 && albumListCount != albumlist.Count)
                    {
                        searchAlbumList.Items.Add(createsearchElement(albumlist[albumListCount], albumListCount));
                        albumListCount++;
                    }
                }

            }

            if (albumlist.Count > 10)
            {
                albumlistNextBtn.Visibility = Visibility.Visible;
                albumlistPrevBtn.Visibility = Visibility.Visible;

            }

            albumListProgressRing.IsActive = false;

        }

        private void parseArtists(SearchResponse response, CancellationToken ct)
        {
            int artistListCount = 0;
            artistListProgressRing.IsActive = true;

            //Parse search results into lists
            for (int i = 0; i < response.Entries.Count(); i++)
            {
                if (response.Entries.ElementAt(i).Type == "2")//Artists
                {
                    try
                    {
                        artistList.Add(response.Entries.ElementAt(i).Artist);
                    }

                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine(e);
                        break;
                    }

                    if ((artistListCount < 10) && (artistListCount != artistList.Count))
                    {
                        searchArtistList.Items.Add(createsearchElement(artistList[artistListCount], artistListCount));
                        artistListCount++;
                    }
                }
            }

            if (artistList.Count > 10)
            {
                artistlistNextBtn.Visibility = Visibility.Visible;
                artistlistPrevBtn.Visibility = Visibility.Visible;

            }

            artistListProgressRing.IsActive = false;
        }

        public async void playSong(Track track)
        {
            if (lastNetworkState != "No Internet Access")
            {
                np.isLoadingSong = true;
                Uri uri;

                if (track != null)
                {

                    if (isFirstPlaySinceOpen)
                        setupPlayer();

                    try
                    {
                        uri = await GetStreamUrl(mc, track);
                    }

                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.Write(e);
                        uri = null;
                    }

                    if (uri != null)
                    {
                        await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {
                            try
                            {
                                setNowPlayingAnimation(np.currentSongIndex);

                                if (track.AlbumArtReference != null)
                                    albumArtImage.Source = new BitmapImage(new Uri(track.AlbumArtReference[0].Url));
                                else
                                    albumArtImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/logo2480x1200.png", UriKind.Absolute));

                                player.Source = MediaSource.CreateFromUri(uri);
                                trackPlayProgressBar.Maximum = (double)track.DurationMillis / 1000;
                                trackPlayProgressBar.Value = 0;

                                if (track.Artist.ToString().Length > 20)
                                {
                                    clartistnametextBlock.Text = track.Artist.ToString().Substring(0, 19) + "...";
                                }
                                else
                                    clartistnametextBlock.Text = track.Artist.ToString();

                                if (track.Title.ToString().Length > 20)
                                {
                                    clsongnametextBlock.Text = track.Title.ToString().Substring(0, 19) + "...";
                                }
                                else
                                    clsongnametextBlock.Text = track.Title.ToString();


                                np.isSongEnded = false;
                                songTimer.Start();
                                player.Play();

                                if (stackPanel1.Visibility == Visibility.Collapsed)
                                    stackPanel1.Visibility = Visibility.Visible;
                                if (playButton.Visibility == Visibility.Visible)
                                    playButton.Visibility = Visibility.Collapsed;
                                if (pauseButton.Visibility == Visibility.Collapsed)
                                    pauseButton.Visibility = Visibility.Visible;
                                if (clCanvas.Visibility == Visibility.Collapsed)
                                    clCanvas.Visibility = Visibility.Visible;
                                if (shuffleButton.Visibility == Visibility.Collapsed)
                                    shuffleButton.Visibility = Visibility.Visible;
                                if (volumeSlider.Visibility == Visibility.Collapsed)
                                    volumeSlider.Visibility = Visibility.Visible;
                            }

                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.Write(ex);
                            }

                        });
                    }

                    else
                    {
                        playSong(np.GetNextSong());
                        await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {
                            generalFlyout.Text = "Moving on. Something screwy happened with that last one.";
                            FlyoutBase.ShowAttachedFlyout(appTitleTextBox);
                        });
                    }
                }

                else
                {
                    playSong(np.GetNextSong());
                    await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        generalFlyout.Text = "Moving on. Something screwy happened with that last one.";
                        FlyoutBase.ShowAttachedFlyout(appTitleTextBox);
                    });
                }

                np.isLoadingSong = false;
            }
        }

        private void setupPlayer()
        {

            systemMediaTransportControls = player.SystemMediaTransportControls;
            player.CommandManager.IsEnabled = false;
            systemMediaTransportControls.IsEnabled = true;
            systemMediaTransportControls.IsPlayEnabled = true;
            systemMediaTransportControls.IsPauseEnabled = true;
            systemMediaTransportControls.IsNextEnabled = true;
            systemMediaTransportControls.IsPreviousEnabled = true;
            songTimer.Interval = new TimeSpan(0, 0, 0, 0, 17);
            player.Volume = .5;

            songTimer.Tick += SongTimer_Tick;
            systemMediaTransportControls.ButtonPressed += SystemMediaTransportControls_ButtonPressed;
            player.MediaOpened += Player_MediaOpened;
            player.MediaEnded += Player_MediaEnded;
            player.PlaybackSession.PlaybackStateChanged += PlaybackSession_PlaybackStateChanged;

            isFirstPlaySinceOpen = false;
        }

        private void Player_MediaOpened(MediaPlayer sender, object args)
        {
            // Get the updater.
            SystemMediaTransportControlsDisplayUpdater updater = systemMediaTransportControls.DisplayUpdater;
            updater.Type = MediaPlaybackType.Music;
            updater.MusicProperties.AlbumArtist = np.GetCurrentSong().Artist.ToString();
            updater.MusicProperties.AlbumTitle = np.GetCurrentSong().Album.ToString();
            updater.MusicProperties.Title = np.GetCurrentSong().Title.ToString();

            // Set the album art thumbnail.
            // RandomAccessStreamReference is defined in Windows.Storage.Streams
            if(np.GetCurrentSong().AlbumArtReference[0].Url != null)
                updater.Thumbnail = RandomAccessStreamReference.CreateFromUri(new Uri(np.GetCurrentSong().AlbumArtReference[0].Url));
            else
                updater.Thumbnail = RandomAccessStreamReference.CreateFromUri(new Uri("Assets/logo2480x1200.png", UriKind.Relative));

            updater.Update();

        }

        private void SongTimer_Tick(object sender, object e)
        {
            trackPlayProgressBar.Value = player.PlaybackSession.Position.TotalSeconds;
        }

        private async void Player_MediaEnded(MediaPlayer sender, object args)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => { songTimer.Stop(); });

            np.isSongEnded = true;

            if (np.currentSongIndex < np.Songs.Count() - 1)
            {
                np.isSongEnded = false;
                playSong(np.GetNextSong());
            }

            else
            {
                await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    pauseButton.Visibility = Visibility.Collapsed;
                    playButton.Visibility = Visibility.Visible;
                    clCanvas.Visibility = Visibility.Collapsed;

                    if(np.currentSongIndex == np.Songs.Count() - 1)
                    {
                        generalFlyout.Text = "Whatever you were listening to is stale or reached its end. Pick something new!";
                        FlyoutBase.ShowAttachedFlyout(appTitleTextBox);
                    }
                });
            }
        }

        public void playCurrentPlaylist(int startingTrack)
        {
            int index = 0;

            if(currentPlaylistGridView.Items.Any())
                currentPlaylistGridView.Items.Clear();

            foreach (Track song in np.Songs)
            {
                createNowPlayingListItem(song, index);
                index++;
            }

            playSong(np.GetCurrentSong());
        }

        public async Task<Uri> GetStreamUrl(MobileClient mc, Track track)
        {
            Uri data;

            data = await mc.GetStreamUrlAsync(track);

            return data;
        }

        public async Task<Track> getTrack(MobileClient mc, string trackId, CancellationToken ct)
        {
            Track data;
            data = await mc.GetTrackAsync(trackId);
            return data;
        }

        public async Task<Track> getTrack(MobileClient mc, string trackId)
        {
            Track data;
            data = await mc.GetTrackAsync(trackId);
            return data;
        }

        public async Task<Album> getAlbum(MobileClient mc, string albumId, CancellationToken ct)
        {
            Album data;
            data = await mc.GetAlbumAsync(albumId);
            return data;
        }

        public async Task<Album> getAlbum(MobileClient mc, string albumId)
        {
            Album data;
            data = await mc.GetAlbumAsync(albumId);
            return data;
        }

        private Grid createsearchElement(Track track, int index)
        {
            Grid grid1 = new Grid();
            Grid grid2 = new Grid();
            Grid grid3 = new Grid();

            // Create column definitions for first grid
            ColumnDefinition columnDefinition1 = new ColumnDefinition();
            ColumnDefinition columnDefinition2 = new ColumnDefinition();
            ColumnDefinition columnDefinition3 = new ColumnDefinition();
            columnDefinition1.Width = new GridLength(1, GridUnitType.Star);
            //columnDefinition2.Width = new GridLength(300, GridUnitType.Pixel);
            columnDefinition2.Width = new GridLength(120, GridUnitType.Pixel); //xbox
            columnDefinition3.Width = new GridLength(1, GridUnitType.Star);

            // Create row definitions for second grid
            RowDefinition rowDefinition1 = new RowDefinition();
            RowDefinition rowDefinition2 = new RowDefinition();
            rowDefinition1.Height = new GridLength(1, GridUnitType.Star);
            rowDefinition2.Height = new GridLength(1, GridUnitType.Star);

            // Create row definitions for third grid
            RowDefinition rowDefinition3 = new RowDefinition();
            RowDefinition rowDefinition4 = new RowDefinition();
            RowDefinition rowDefinition5 = new RowDefinition();
            rowDefinition3.Height = new GridLength(1, GridUnitType.Star);
            rowDefinition4.Height = new GridLength(1, GridUnitType.Star);
            rowDefinition5.Height = new GridLength(1, GridUnitType.Star);

            // Attached definitions to grids
            grid1.ColumnDefinitions.Add(columnDefinition1);
            grid1.ColumnDefinitions.Add(columnDefinition2);
            grid1.ColumnDefinitions.Add(columnDefinition3);
            grid2.RowDefinitions.Add(rowDefinition1);
            grid2.RowDefinitions.Add(rowDefinition2);
            grid3.RowDefinitions.Add(rowDefinition3);
            grid3.RowDefinitions.Add(rowDefinition4);
            grid3.RowDefinitions.Add(rowDefinition5);

            //Create elements
            Windows.UI.Xaml.Controls.Image image = new Windows.UI.Xaml.Controls.Image();
            TextBlock trackName = new TextBlock();
            TextBlock albumName = new TextBlock();
            TextBlock artistName = new TextBlock();
            Button trackRadioButton = new Button();
            Button trackPlayButton = new Button();

            //Set image properties
            image.Width = 100;//double.NaN; //150 for pc
            image.Height = 100;//double.NaN; //150 for pc
            image.Stretch = Stretch.Fill;
            image.Margin = new Thickness(0, 0, 0, 0);
            image.HorizontalAlignment = HorizontalAlignment.Stretch;
            image.VerticalAlignment = VerticalAlignment.Stretch;
            if (track.AlbumArtReference[0].Url != null)
                image.Source = new BitmapImage(new Uri(track.AlbumArtReference[0].Url));
            else
                image.Source = new BitmapImage(new Uri("ms-appx:///Assets/no_image.png", UriKind.Absolute));

            //Set trackName properties
            trackName.Width = 225;
            trackName.Height = double.NaN;
            trackName.Margin = new Thickness(5, 0, 0, 0);
            trackName.HorizontalAlignment = HorizontalAlignment.Left;
            trackName.VerticalAlignment = VerticalAlignment.Center;
            trackName.FontSize = 10; //15 pc
            trackName.Text = track.Title.ToString();
            trackName.Foreground = new SolidColorBrush(Colors.White);

            //Set albumName properties
            albumName.Width = 225;
            albumName.Height = double.NaN;
            albumName.Margin = new Thickness(5, 0, 0, 0);
            albumName.HorizontalAlignment = HorizontalAlignment.Left;
            albumName.VerticalAlignment = VerticalAlignment.Center;
            albumName.FontSize = 10; // 15 pc
            albumName.Text = track.Album.ToString();
            albumName.Foreground = new SolidColorBrush(Colors.White);

            //Set artistName properties
            artistName.Width = 225;
            artistName.Height = double.NaN;
            artistName.Margin = new Thickness(5, 0, 0, 0);
            artistName.HorizontalAlignment = HorizontalAlignment.Left;
            artistName.VerticalAlignment = VerticalAlignment.Center;
            artistName.FontSize = 10; //15 pc
            artistName.Text = track.Artist.ToString();
            artistName.Foreground = new SolidColorBrush(Colors.White);

            //Set radioButton properties
            trackRadioButton.Name = "RadioButton";
            trackRadioButton.Width = 75; //100 pc
            trackRadioButton.Height = double.NaN;
            trackRadioButton.Margin = new Thickness(0, 0, 0, 0);
            trackRadioButton.HorizontalAlignment = HorizontalAlignment.Right;
            trackRadioButton.VerticalAlignment = VerticalAlignment.Stretch;
            trackRadioButton.Content = "Start Radio";
            trackRadioButton.FontSize = 10; //15 pc
            trackRadioButton.Tag = index;
            trackRadioButton.Foreground = new SolidColorBrush(Colors.White);
            trackRadioButton.Click += TrackRadioButton_Click;
            trackRadioButton.Style = (Style)this.Resources["customButton"];

            //Set trackPlayButton properties
            trackPlayButton.Width = 75; //100 pc
            trackPlayButton.Height = double.NaN;
            trackPlayButton.Margin = new Thickness(0, 0, 0, 0);
            trackPlayButton.HorizontalAlignment = HorizontalAlignment.Right;
            trackPlayButton.VerticalAlignment = VerticalAlignment.Stretch;
            trackPlayButton.Content = "Play";
            trackPlayButton.FontSize = 10; //15 pc
            trackPlayButton.Foreground = new SolidColorBrush(Colors.White);
            trackPlayButton.Tag = index;
            trackPlayButton.Click += trackPlayButton_Click;
            trackPlayButton.Style = (Style)this.Resources["customButton"];

            grid1.Margin = new Thickness(0, 3, 0, 0);


            //Assign elements to grid and row/columns
            grid1.Children.Add(image);
            grid1.Children.Add(grid2);
            grid1.Children.Add(grid3);

            grid2.Children.Add(trackRadioButton);
            grid2.Children.Add(trackPlayButton);

            grid3.Children.Add(trackName);
            grid3.Children.Add(albumName);
            grid3.Children.Add(artistName);

            Grid.SetColumn(image, 0);
            Grid.SetColumn(grid3, 1);
            Grid.SetColumn(grid2, 2);

            Grid.SetRow(trackName, 0);
            Grid.SetRow(albumName, 1);
            Grid.SetRow(artistName, 2);
            Grid.SetRow(trackRadioButton, 0);
            Grid.SetRow(trackPlayButton, 1);

            return grid1;
        }

        private async void TrackRadioButton_Click(object sender, RoutedEventArgs e)
        {
            if (np.Songs.Any())
                np.Songs.Clear();
            np.currentSongIndex = 0;

            Button button = sender as Button;
            int index = (int)button.Tag;

            RadioFeed feed = await getTrackRadioStation(mc, tracklist[index].Nid.ToString());

            np.PopulateSongs(feed.Data.Stations[0].Tracks);

            mainPivotMenu.SelectedIndex = 2;

            playCurrentPlaylist(np.currentSongIndex);
        }

        private Grid createsearchElement(Album album, int index)
        {
            Grid grid1 = new Grid();
            Grid grid2 = new Grid();
            Grid grid3 = new Grid();

            // Create column definitions for first grid
            ColumnDefinition columnDefinition1 = new ColumnDefinition();
            ColumnDefinition columnDefinition2 = new ColumnDefinition();
            ColumnDefinition columnDefinition3 = new ColumnDefinition();
            columnDefinition1.Width = new GridLength(1, GridUnitType.Star);
            columnDefinition2.Width = new GridLength(120, GridUnitType.Pixel);
            columnDefinition3.Width = new GridLength(1, GridUnitType.Star);

            // Create row definitions for second grid
            RowDefinition rowDefinition1 = new RowDefinition();
            RowDefinition rowDefinition2 = new RowDefinition();
            rowDefinition1.Height = new GridLength(1, GridUnitType.Star);
            rowDefinition2.Height = new GridLength(1, GridUnitType.Star);

            // Create row definitions for third grid
            RowDefinition rowDefinition3 = new RowDefinition();
            RowDefinition rowDefinition4 = new RowDefinition();
            RowDefinition rowDefinition5 = new RowDefinition();
            rowDefinition3.Height = new GridLength(1, GridUnitType.Star);
            rowDefinition4.Height = new GridLength(1, GridUnitType.Star);

            // Attached definitions to grids
            grid1.ColumnDefinitions.Add(columnDefinition1);
            grid1.ColumnDefinitions.Add(columnDefinition2);
            grid1.ColumnDefinitions.Add(columnDefinition3);
            grid2.RowDefinitions.Add(rowDefinition1);
            grid2.RowDefinitions.Add(rowDefinition2);
            grid3.RowDefinitions.Add(rowDefinition3);
            grid3.RowDefinitions.Add(rowDefinition4);
            grid3.RowDefinitions.Add(rowDefinition5);

            //Create elements
            Windows.UI.Xaml.Controls.Image image = new Windows.UI.Xaml.Controls.Image();
            TextBlock albumName = new TextBlock();
            TextBlock artistName = new TextBlock();
            Button albumRadioButton = new Button();
            Button playAlbumButton = new Button();

            //Set image properties
            image.Width = 100;//double.NaN;
            image.Height = 100;//double.NaN;
            image.Stretch = Stretch.Fill;
            image.Margin = new Thickness(0, 0, 0, 0);
            image.HorizontalAlignment = HorizontalAlignment.Stretch;
            image.VerticalAlignment = VerticalAlignment.Stretch;
            if (album.AlbumArtRef != null)
                image.Source = new BitmapImage(new Uri(album.AlbumArtRef));
            else
                image.Source = new BitmapImage(new Uri("ms-appx:///Assets/no_image.png", UriKind.Absolute));

            //Set albumName properties
            albumName.Width = 225;
            albumName.Height = double.NaN;
            albumName.Margin = new Thickness(5, 0, 0, 0);
            albumName.HorizontalAlignment = HorizontalAlignment.Left;
            albumName.VerticalAlignment = VerticalAlignment.Center;
            albumName.FontSize = 10;
            albumName.Text = album.Name.ToString();
            albumName.Foreground = new SolidColorBrush(Colors.White);

            //Set artistName properties
            artistName.Width = 225;
            artistName.Height = double.NaN;
            artistName.Margin = new Thickness(5, 0, 0, 0);
            artistName.HorizontalAlignment = HorizontalAlignment.Left;
            artistName.VerticalAlignment = VerticalAlignment.Center;
            artistName.FontSize = 10;
            artistName.Text = album.Artist.ToString();
            artistName.Foreground = new SolidColorBrush(Colors.White);

            //Set radioButton properties
            albumRadioButton.Width = 100;
            albumRadioButton.Height = double.NaN;
            albumRadioButton.Margin = new Thickness(0, 0, 0, 0);
            albumRadioButton.HorizontalAlignment = HorizontalAlignment.Right;
            albumRadioButton.VerticalAlignment = VerticalAlignment.Stretch;
            albumRadioButton.Content = "Start Radio";
            albumRadioButton.FontSize = 10;
            albumRadioButton.Tag = index;
            albumRadioButton.Foreground = new SolidColorBrush(Colors.White);
            albumRadioButton.Click += AlbumRadioButton_Click;
            albumRadioButton.Style = (Style)this.Resources["customButton"];

            //Set playAlbumButton properties
            playAlbumButton.Width = 100;
            playAlbumButton.Height = double.NaN;
            playAlbumButton.Margin = new Thickness(0, 0, 0, 0);
            playAlbumButton.HorizontalAlignment = HorizontalAlignment.Right;
            playAlbumButton.VerticalAlignment = VerticalAlignment.Stretch;
            playAlbumButton.Content = "Play All";
            playAlbumButton.FontSize = 10;
            playAlbumButton.Foreground = new SolidColorBrush(Colors.White);
            playAlbumButton.Tag = index;
            playAlbumButton.Click += PlayAlbumButton_Click;
            playAlbumButton.Style = (Style)this.Resources["customButton"];

            grid1.Margin = new Thickness(0, 3, 0, 0);


            //Assign elements to grid and row/columns
            grid1.Children.Add(image);
            grid1.Children.Add(grid2);
            grid1.Children.Add(grid3);

            grid2.Children.Add(albumRadioButton);
            grid2.Children.Add(playAlbumButton);

            grid3.Children.Add(albumName);
            grid3.Children.Add(artistName);

            Grid.SetColumn(image, 0);
            Grid.SetColumn(grid3, 1);
            Grid.SetColumn(grid2, 2);

            Grid.SetRow(albumName, 0);
            Grid.SetRow(artistName, 1);
            Grid.SetRow(albumRadioButton, 0);
            Grid.SetRow(playAlbumButton, 1);

            return grid1;
        }

        private async void AlbumRadioButton_Click(object sender, RoutedEventArgs e)
        {
            if (np.Songs.Any())
                np.Songs.Clear();
            np.currentSongIndex = 0;

            Button button = sender as Button;
            int index = (int)button.Tag;

            RadioFeed feed = await getAlbumRadioStation(mc, albumlist[index].AlbumId.ToString());

            np.PopulateSongs(feed.Data.Stations[0].Tracks);

            mainPivotMenu.SelectedIndex = 2;

            playCurrentPlaylist(np.currentSongIndex);
        }

        private void PlayAlbumButton_Click(object sender, RoutedEventArgs e)
        {
            if (np.Songs.Any())
                np.Songs.Clear();
            np.currentSongIndex = 0;

            Button button = sender as Button;
            int index = (int)button.Tag;

            np.PopulateSongs(albumlist[index].Tracks);

            mainPivotMenu.SelectedIndex = 2;

            playCurrentPlaylist(np.currentSongIndex);
        }

        private Grid createsearchElement(Artist artist, int index)
        {
            Grid grid1 = new Grid();
            Grid grid2 = new Grid();
            Grid grid3 = new Grid();

            // Create column definitions for first grid
            ColumnDefinition columnDefinition1 = new ColumnDefinition();
            ColumnDefinition columnDefinition2 = new ColumnDefinition();
            ColumnDefinition columnDefinition3 = new ColumnDefinition();
            columnDefinition1.Width = new GridLength(1, GridUnitType.Star);
            columnDefinition2.Width = new GridLength(100, GridUnitType.Pixel);
            columnDefinition3.Width = new GridLength(1, GridUnitType.Star);

            // Create row definitions for second grid
            RowDefinition rowDefinition1 = new RowDefinition();
            RowDefinition rowDefinition2 = new RowDefinition();
            rowDefinition1.Height = new GridLength(1, GridUnitType.Star);
            rowDefinition2.Height = new GridLength(1, GridUnitType.Star);

            // Create row definitions for third grid
            RowDefinition rowDefinition3 = new RowDefinition();
            RowDefinition rowDefinition4 = new RowDefinition();
            RowDefinition rowDefinition5 = new RowDefinition();
            rowDefinition3.Height = new GridLength(1, GridUnitType.Star);
            rowDefinition4.Height = new GridLength(1, GridUnitType.Star);

            // Attached definitions to grids
            grid1.ColumnDefinitions.Add(columnDefinition1);
            grid1.ColumnDefinitions.Add(columnDefinition2);
            grid1.ColumnDefinitions.Add(columnDefinition3);
            grid2.RowDefinitions.Add(rowDefinition1);
            grid2.RowDefinitions.Add(rowDefinition2);
            grid3.RowDefinitions.Add(rowDefinition3);
            grid3.RowDefinitions.Add(rowDefinition4);
            grid3.RowDefinitions.Add(rowDefinition5);

            //Create elements
            Windows.UI.Xaml.Controls.Image image = new Windows.UI.Xaml.Controls.Image();
            TextBlock albumName = new TextBlock();
            TextBlock artistName = new TextBlock();
            Button artistRadioButton = new Button();
            Button artistplayButton = new Button();

            //Set image properties
            image.Width = 125;//double.NaN;
            image.Height = 100;//double.NaN;
            image.Stretch = Stretch.Fill;
            image.Margin = new Thickness(0, 0, 0, 0);
            image.HorizontalAlignment = HorizontalAlignment.Stretch;
            image.VerticalAlignment = VerticalAlignment.Stretch;
            if (artist.ArtistArtRef != null)
                image.Source = new BitmapImage(new Uri(artist.ArtistArtRef));
            else
                image.Source = new BitmapImage(new Uri("ms-appx:///Assets/no_image.png", UriKind.Absolute));

            //Set artistName properties
            artistName.Width = 225;
            artistName.Height = double.NaN;
            artistName.Margin = new Thickness(5, 0, 0, 0);
            artistName.HorizontalAlignment = HorizontalAlignment.Left;
            artistName.VerticalAlignment = VerticalAlignment.Center;
            artistName.FontSize = 10;
            artistName.Text = artist.Name.ToString();
            artistName.Foreground = new SolidColorBrush(Colors.White);

            //Set radioButton properties
            artistRadioButton.Width = 100;
            artistRadioButton.Height = double.NaN;
            artistRadioButton.Margin = new Thickness(0, 0, 0, 0);
            artistRadioButton.HorizontalAlignment = HorizontalAlignment.Right;
            artistRadioButton.VerticalAlignment = VerticalAlignment.Stretch;
            artistRadioButton.Content = "Start Radio";
            artistRadioButton.FontSize = 10;
            artistRadioButton.Tag = index;
            artistRadioButton.Foreground = new SolidColorBrush(Colors.White);
            artistRadioButton.Click += ArtistRadioButton_Click;
            artistRadioButton.Style = (Style)this.Resources["customButton"];

            //Set playAllButton properties
            artistplayButton.Width = 100;
            artistplayButton.Height = double.NaN;
            artistplayButton.Margin = new Thickness(0, 0, 0, 0);
            artistplayButton.HorizontalAlignment = HorizontalAlignment.Right;
            artistplayButton.VerticalAlignment = VerticalAlignment.Stretch;
            artistplayButton.Content = "Play All";
            artistplayButton.FontSize = 10;
            artistplayButton.Foreground = new SolidColorBrush(Colors.White);
            artistplayButton.Tag = index;
            artistplayButton.Click += artistplayButton_Click;
            artistplayButton.Style = (Style)this.Resources["customButton"];

            grid1.Margin = new Thickness(0, 3, 0, 0);


            //Assign elements to grid and row/columns
            grid1.Children.Add(image);
            grid1.Children.Add(grid2);
            grid1.Children.Add(grid3);

            grid2.Children.Add(artistRadioButton);
            grid2.Children.Add(artistplayButton);

            grid3.Children.Add(artistName);

            Grid.SetColumn(image, 0);
            Grid.SetColumn(grid3, 1);
            Grid.SetColumn(grid2, 2);

            Grid.SetRow(artistName, 0);
            Grid.SetRow(artistRadioButton, 0);
            Grid.SetRow(artistplayButton, 1);

            return grid1;
        }

        private async void ArtistRadioButton_Click(object sender, RoutedEventArgs e)
        {
            if (np.Songs.Any())
                np.Songs.Clear();
            np.currentSongIndex = 0;

            Button button = sender as Button;
            int index = (int)button.Tag;

            RadioFeed feed = await getArtistRadioStation(mc, artistList[index].ArtistId.ToString());

            np.PopulateSongs(feed.Data.Stations[0].Tracks);

            mainPivotMenu.SelectedIndex = 2;

            playCurrentPlaylist(np.currentSongIndex);

        }

        private async void artistplayButton_Click(object sender, RoutedEventArgs e)
        {
            SearchResponse response;

            if (np.Songs.Any())
                np.Songs.Clear();
            artistAlbumList.Clear();

            np.currentSongIndex = 0;

            Button button = sender as Button;
            int index = (int)button.Tag;

            response = await Search(artistList[index].Name.ToString());

            foreach (SearchResult entry in response.Entries)
            {
                if (entry.Type == "3" && entry.Album.ArtistId[0].ToString() == artistList[index].ArtistId.ToString())
                    artistAlbumList.Add(await getAlbum(mc, entry.Album.AlbumId.ToString()));
            }

            foreach (Album album in artistAlbumList)
            {
                if (album != null)
                {
                    np.PopulateSongs(album.Tracks);
                }
            }

            mainPivotMenu.SelectedIndex = 2;

            playCurrentPlaylist(np.currentSongIndex);
        }

        private void trackPlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (np.Songs.Any())
                np.Songs.Clear();
            np.currentSongIndex = 0;

            Button button = sender as Button;
            int index = (int)button.Tag;

            np.Songs.Add(tracklist[index]);

            mainPivotMenu.SelectedIndex = 2;

            playCurrentPlaylist(np.currentSongIndex);
        }

        private void playPause_Click(object sender, RoutedEventArgs e)
        {
            if (player.PlaybackSession.PlaybackState == MediaPlaybackState.Playing)
            {
                player.Pause();
                pauseButton.Visibility = Visibility.Collapsed;
                playButton.Visibility = Visibility.Visible;
                playButton.Focus(FocusState.Programmatic);
            }

            else if (player.PlaybackSession.PlaybackState == MediaPlaybackState.Paused)
            {
                playButton.Visibility = Visibility.Collapsed;
                pauseButton.Visibility = Visibility.Visible;
                pauseButton.Focus(FocusState.Programmatic);

                if (np.isSongEnded)
                    playSong(np.GetNextSong());
                else
                    player.Play();
            }
        }

        public async Task<SearchResponse> Search(string query)
        {
            SearchResponse data = await mc.SearchAsync(query);

            return data;
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            if (!np.isLoadingSong)
            {
                if (np.currentSongIndex < np.Songs.Count() - 1)
                {
                    playSong(np.GetNextSong());
                }
            }
        }

        private void previousButton_Click(object sender, RoutedEventArgs e)
        {
            if (!np.isLoadingSong)
            {
                if (np.currentSongIndex > 0)
                {
                    playSong(np.GetPreviousSong());
                }
                else
                    player.PlaybackSession.Position = TimeSpan.Zero;
            }
        }

        private void tracklistNextBtn_Click(object sender, RoutedEventArgs e)
        {
            double countCheck = ((double)tracklist.Count / 10) - 1;

            if (trackPageNumber < countCheck)
            {
                trackPageNumber++;
                int i = 0;
                int index = trackPageNumber * 10;


                searchSongList.Items.Clear();

                while (i < 10)
                {
                    if (index < tracklist.Count)
                    {
                        searchSongList.Items.Add(createsearchElement(tracklist[index], index));
                        index++;
                        i++;
                    }
                    else
                        break;
                }
            }
        }

        private void tracklistPrevBtn_Click(object sender, RoutedEventArgs e)
        {
            if (trackPageNumber > 0)
            {
                trackPageNumber--;
                int i = 0;
                int index = trackPageNumber * 10;


                searchSongList.Items.Clear();

                while (i < 10)
                {
                    if (index < tracklist.Count)
                    {
                        searchSongList.Items.Add(createsearchElement(tracklist[index], index));
                        index++;
                        i++;
                    }
                }
            }
        }

        private void albumlistPrevBtn_Click(object sender, RoutedEventArgs e)
        {
            if (albumPageNumber > 0)
            {
                albumPageNumber--;
                int i = 0;
                int index = albumPageNumber * 10;


                searchAlbumList.Items.Clear();

                while (i < 10)
                {
                    if (index < albumlist.Count)
                    {
                        searchAlbumList.Items.Add(createsearchElement(albumlist[index], index));
                        index++;
                        i++;
                    }
                }
            }
        }

        private void albumlistNextBtn_Click(object sender, RoutedEventArgs e)
        {
            double countCheck = ((double)albumlist.Count / 10) - 1;

            if (albumPageNumber < countCheck)
            {
                albumPageNumber++;
                int i = 0;
                int index = albumPageNumber * 10;

                searchAlbumList.Items.Clear();

                while (i < 10)
                {
                    if (index < albumlist.Count)
                    {
                        searchAlbumList.Items.Add(createsearchElement(albumlist[index], index));
                        index++;
                        i++;
                    }

                    else
                        break;
                }
            }
        }

        private void artistlistNextBtn_Click(object sender, RoutedEventArgs e)
        {
            double countCheck = ((double)artistList.Count / 10) - 1;

            if (artistPageNumber < countCheck)
            {
                artistPageNumber++;
                int i = 0;
                int index = artistPageNumber * 10;


                searchArtistList.Items.Clear();

                while (i < 10)
                {
                    if (index < artistList.Count)
                    {
                        searchArtistList.Items.Add(createsearchElement(artistList[index], index));
                        index++;
                        i++;
                    }
                    else
                        break;
                }
            }
        }

        private void artistlistPrevBtn_Click(object sender, RoutedEventArgs e)
        {
            if (artistPageNumber > 0)
            {
                artistPageNumber--;
                int i = 0;
                int index = artistPageNumber * 10;

                searchArtistList.Items.Clear();

                while (i < 10)
                {
                    if (index < artistList.Count)
                    {
                        searchArtistList.Items.Add(createsearchElement(artistList[index], index));
                        index++;
                        i++;
                    }
                }
            }
        }

        private void mainPivotMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (PivotItem pivotItem in mainPivotMenu.Items)
            {
                if (pivotItem == mainPivotMenu.Items[mainPivotMenu.SelectedIndex])
                {
                    ((TextBlock)pivotItem.Header).Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 255, 12));
                }
                else
                {
                    ((TextBlock)pivotItem.Header).Foreground = new SolidColorBrush(Colors.White);
                }
            }
        }

        private void shuffleButton_Click(object sender, RoutedEventArgs e)
        {
            if (np.Songs.Count > 1)
            {

                np.ShuffleSongs();
                playCurrentPlaylist(np.currentSongIndex);
            }

            else
            {
                generalFlyout.Text = "Dafuq? Trying to shuffle one song is for crazy people!";
                FlyoutBase.ShowAttachedFlyout(appTitleTextBox);
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                PassSession data = e.Parameter as PassSession;
                mc = data.session;

                if (mc != null)
                {
                    signedInAsTextBlock.Text = mc.Session.UserDetails.Email.ToString();
                    signedInAsTextBlock.Visibility = Visibility.Visible;

                    if (await testAuthorizationLevel())
                    {
                        getListenNow();
                    }
                    else
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            PivotItem piv = (PivotItem)mainPivotMenu.Items[i];
                            piv.Visibility = Visibility.Collapsed;
                            ((TextBlock)piv.Header).Visibility = Visibility.Collapsed;
                        }

                        mainPivotMenu.SelectedIndex = 3;
                        generalFlyout.Text = "Something went wrong with your request. Please make sure you have a valid Music Subscription associated with the Gmail account you logged in with.This app will not work without one.";
                        FlyoutBase.ShowAttachedFlyout(appTitleTextBox);
                    }
                }
            }

            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
                generalFlyout.Text = "Something went wrong with your request.";
                FlyoutBase.ShowAttachedFlyout(appTitleTextBox);
            }
        }

        private async Task<bool> testAuthorizationLevel()
        {
            try
            {
                Track data = await getTrack(mc, "Tolw673c2mkdbbthmo4e6vzgsdu");

                Uri data2 = await GetStreamUrl(mc, data);

                if (data2 != null)
                    return true;
                else
                    return false;
            }

            catch(Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
                return false;
            }
        }

        private void logoutButton_Click(object sender, RoutedEventArgs e)
        {
            if(player.PlaybackSession.PlaybackState == MediaPlaybackState.Playing || player.PlaybackSession.PlaybackState == MediaPlaybackState.Paused)
            {
                player.Pause();
            }

            if (cts != null)
                cts.Cancel();
            if (cts2 != null)
                cts2.Cancel();

            var vault = new Windows.Security.Credentials.PasswordVault();

            Windows.Security.Credentials.PasswordCredential credential = GetCredentialFromLocker();

            if (credential != null)
                vault.Remove(new Windows.Security.Credentials.PasswordCredential("Bermuda",credential.UserName, credential.Password));

            mc = null;

            clearLists();

            Frame.Navigate(typeof(LoginPage));

        }

        private Windows.Security.Credentials.PasswordCredential GetCredentialFromLocker()
        {
            Windows.Security.Credentials.PasswordCredential credential = null;

            var vault = new Windows.Security.Credentials.PasswordVault();

            try
            {
                var credentialList = vault.FindAllByResource("Bermuda");

                if (credentialList.Count > 0)
                {
                    if (credentialList.Count == 1)
                    {
                        credential = credentialList[0];
                        credential.RetrievePassword();
                    }
                }

                return credential;
            }

            catch
            {
                return credential;
            }
        }

        private void clearLists()
        {
            if(tracklist != null)
                tracklist.Clear();
            if(albumlist != null)
                albumlist.Clear();
            if (artistList != null)
                artistList.Clear();
            if(artistAlbumList != null)
                artistAlbumList.Clear();
            if(np.Songs != null)
                np.Songs.Clear();
            if(listenNowList != null)
                listenNowList.Clear();
    }

        private void volumeSlider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            player.Volume = volumeSlider.Value/100;
        }

        private async void createNetworkAwarenessTask(string taskName)
        {
            try
            {
                var builder = new BackgroundTaskBuilder();

                builder.Name = taskName;
                builder.TaskEntryPoint = "Tasks.NetworkAwareness";
                builder.SetTrigger(new SystemTrigger(SystemTriggerType.NetworkStateChange, false));
                BackgroundAccessStatus status = await BackgroundExecutionManager.RequestAccessAsync();

                BackgroundTaskRegistration task = builder.Register();
                task.Completed += new BackgroundTaskCompletedEventHandler(networkAwarenessOnCompleted);
            }

            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }


        }

        private async void createServicingTask(string taskName)
        {
            try
            {
                var builder = new BackgroundTaskBuilder();

                builder.Name = taskName;
                builder.TaskEntryPoint = "Tasks.ServicingComplete";
                builder.SetTrigger(new SystemTrigger(SystemTriggerType.ServicingComplete, false));
                BackgroundAccessStatus status = await BackgroundExecutionManager.RequestAccessAsync();

                BackgroundTaskRegistration task = builder.Register();
                task.Completed += new BackgroundTaskCompletedEventHandler(servicingOnCompleted);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }


        }

        private bool checkForBGTask(string taskName)
        {
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == taskName)
                {
                    return true;
                }
            }

            return false;
        }

        private async void networkAwarenessOnCompleted(IBackgroundTaskRegistration task, BackgroundTaskCompletedEventArgs args)
        {
            var settings = ApplicationData.Current.LocalSettings;
            var key = task.Name.ToString();
            string status = settings.Values[key].ToString();

            //if status == InternetAccess - enable app functionality and display flyout noting change

            //else - disable app functionality and display flyout noting change

            if (status != lastNetworkState)//Don't update for the same state twice.
            {
                if (status == "Internet Access")
                {
                    bool authTest = await testAuthorizationLevel();
                    lastNetworkState = "Internet Access";

                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        networkFlyout.Text = "Network Connected";
                        FlyoutBase.ShowAttachedFlyout(settingsHeaderTextBlock);

                        if (authTest)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                PivotItem piv = (PivotItem)mainPivotMenu.Items[i];
                                piv.Visibility = Visibility.Visible;
                                ((TextBlock)piv.Header).Visibility = Visibility.Visible;
                            } 

                            mainPivotMenu.SelectedIndex = 0;

                            getListenNow();
                        }

                    });
                }

                else
                {
                    lastNetworkState = "No Internet Access";
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                    () =>
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            PivotItem piv = (PivotItem)mainPivotMenu.Items[i];
                            piv.Visibility = Visibility.Collapsed;
                            ((TextBlock)piv.Header).Visibility = Visibility.Collapsed;
                        }

                        clCanvas.Visibility = Visibility.Collapsed;
                        networkFlyout.Text = "Network Unavailable";
                        mainPivotMenu.SelectedIndex = 3;

                        FlyoutBase.ShowAttachedFlyout(settingsHeaderTextBlock);

                        if (player.PlaybackSession.PlaybackState == MediaPlaybackState.Playing)
                        {
                            player.Pause();
                        }

                        if(np.Songs.Any())
                        {
                            var lastItem = currentPlaylistGridView.ContainerFromIndex(np.prevSongIndex) as GridViewItem;
                            var lastGrid = lastItem.FindName("grid" + np.currentSongIndex) as Grid;
                            lastGrid.Background = null;

                            if(playButton.Visibility == Visibility.Collapsed)
                                playButton.Visibility = Visibility.Visible;
                            if (pauseButton.Visibility == Visibility.Visible)
                                pauseButton.Visibility = Visibility.Collapsed;
                        }
                        

                    });
                }
            }
        }

        private void servicingOnCompleted(IBackgroundTaskRegistration task, BackgroundTaskCompletedEventArgs args)
        {
            if (checkForBGTask(networkBackgroundTask))
                unregisterTask(networkBackgroundTask);
            if (checkForBGTask(servicingBackgroundTask))
                unregisterTask(servicingBackgroundTask);

            BackgroundExecutionManager.RemoveAccess();
        }

        private void reregisterNetworkAwarenessTask(string taskName)
        {
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == taskName)
                {
                    task.Value.Unregister(true);
                    createNetworkAwarenessTask(task.Value.Name);
                }
            }
        }

        private void reregisterServicingTask(string taskName)
        {
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == taskName)
                {
                    unregisterTask(task.Value.Name);
                    createServicingTask(task.Value.Name);
                }
            }
        }

        private void unregisterTask(string taskName)
        {
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == taskName)
                {
                    task.Value.Unregister(true);
                }
            }
        }

        private void createNowPlayingListItem(Track track, int index)
        {
            Grid grid1 = new Grid();

            RowDefinition rowDefinition1 = new RowDefinition();
            rowDefinition1.Height = new GridLength(50, GridUnitType.Pixel);

            // Create column definitions for first grid
            ColumnDefinition columnDefinition1 = new ColumnDefinition();
            ColumnDefinition columnDefinition2 = new ColumnDefinition();
            columnDefinition1.Width = new GridLength(50, GridUnitType.Pixel); //xbox
            columnDefinition2.Width = new GridLength(135, GridUnitType.Pixel);


            // Attached definitions to grids
            grid1.ColumnDefinitions.Add(columnDefinition1);
            grid1.ColumnDefinitions.Add(columnDefinition2);

            //Create elements
            Windows.UI.Xaml.Controls.Image image = new Windows.UI.Xaml.Controls.Image();
            TextBlock trackName = new TextBlock();
            TextBlock albumName = new TextBlock();
            TextBlock artistName = new TextBlock();

            //Set image properties
            image.Width = double.NaN; //150 for pc
            image.Height = double.NaN; //150 for pc
            image.Stretch = Stretch.Fill;
            image.Margin = new Thickness(0, 0, 0, 0);
            image.HorizontalAlignment = HorizontalAlignment.Stretch;
            image.VerticalAlignment = VerticalAlignment.Stretch;
            if (track.AlbumArtReference != null)
            image.Source = new BitmapImage(new Uri(track.AlbumArtReference[0].Url));
            else
            image.Source = new BitmapImage(new Uri("ms-appx:///Assets/no_image.png", UriKind.Absolute));

            //Set trackName properties
            trackName.Width = double.NaN;
            trackName.Height = 15;
            trackName.Margin = new Thickness(0, 0, 15, 0);
            trackName.HorizontalAlignment = HorizontalAlignment.Right;
            trackName.VerticalAlignment = VerticalAlignment.Top;
            trackName.FontSize = 12; //15 pc
            if (track.Title.ToString().Length > 18)
            {
                trackName.Text = track.Title.ToString().Substring(0, 17) + "...";
            }
            else
                trackName.Text = track.Title.ToString();

            trackName.Foreground = new SolidColorBrush(Colors.White);

            //Set albumName properties
            albumName.Width = double.NaN;
            albumName.Height = 15;
            albumName.Margin = new Thickness(0, 0, 15, 0);
            albumName.HorizontalAlignment = HorizontalAlignment.Right;
            albumName.VerticalAlignment = VerticalAlignment.Center;
            albumName.FontSize = 10; // 15 pc
            if (track.Album.ToString().Length > 19)
            {
                albumName.Text = track.Album.ToString().Substring(0, 18) + "...";
            }
            else
                albumName.Text = track.Album.ToString();


            albumName.Foreground = new SolidColorBrush(Colors.White);

            //Set artistName properties
            artistName.Width = double.NaN;
            artistName.Height = 15;
            artistName.Margin = new Thickness(0, 0, 15, 0);
            artistName.HorizontalAlignment = HorizontalAlignment.Right;
            artistName.VerticalAlignment = VerticalAlignment.Bottom;
            artistName.FontSize = 10; //15 pc
            if (track.Artist.ToString().Length > 19)
            {
                artistName.Text = track.Artist.ToString().Substring(0, 18) + "...";
            }
            else
                artistName.Text = track.Artist.ToString();

            artistName.Foreground = new SolidColorBrush(Colors.White);


            //Assign elements to grid and row/columns
            grid1.Children.Add(image);
            grid1.Children.Add(trackName);
            grid1.Children.Add(albumName);
            grid1.Children.Add(artistName);

            Grid.SetColumn(image, 0);
            Grid.SetColumn(trackName, 1);
            Grid.SetColumn(albumName, 1);
            Grid.SetColumn(artistName, 1);

            Grid.SetRow(image, 0);
            Grid.SetRow(trackName, 0);
            Grid.SetRow(albumName, 0);
            Grid.SetRow(artistName, 0);

            grid1.BorderBrush = new SolidColorBrush(Colors.Black);
            grid1.BorderThickness = new Thickness(0.2);
            grid1.Margin = new Thickness(0, 1, 0, 0);
            grid1.Name = "grid" + index;

            currentPlaylistGridView.Items.Add(grid1);
        }

        private void setNowPlayingAnimation(int currentIndex)
        {
            try
            {
                var lastItem = currentPlaylistGridView.ContainerFromIndex(np.prevSongIndex) as GridViewItem;
                var lastGrid = lastItem.FindName("grid" + np.prevSongIndex) as Grid;
                lastGrid.Background = null;
            }

            catch(Exception e)
            {
                System.Diagnostics.Debug.Write(e);

            }
            

            var item = currentPlaylistGridView.ContainerFromIndex(currentIndex) as GridViewItem;
            var grid = item.FindName("grid" + currentIndex) as Grid;

            ImageBrush myBrush = new ImageBrush();
            Windows.UI.Xaml.Controls.Image someImage = new Windows.UI.Xaml.Controls.Image();

            someImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/nowPlaying.gif", UriKind.Absolute));
            myBrush.ImageSource = someImage.Source;

            grid.Background = myBrush;


        }

        private void currentPlaylistGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            int index = this.currentPlaylistGridView.Items.IndexOf(e.ClickedItem);

            playSong(np.GetSongFromIndex(index));

        }
    }

}

