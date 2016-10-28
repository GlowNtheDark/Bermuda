using System;
using System.Linq;
using Windows.Media;
using Windows.Media.Playback;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using GoogleMusicApi.UWP.Common;
using GoogleMusicApi.UWP.Structure;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using System.Threading.Tasks;
using Windows.Media.Core;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Bermuda
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NowPlayingScene : Page
    {
        public NowPlayingScene()
        {
            this.InitializeComponent();

            //this.NavigationCacheMode = NavigationCacheMode.Required;

        }

        public DispatcherTimer songTimer;

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if(NowPlaying.Songs.Any())
                loadUI();

            if (NowPlaying.startPlaying)
            {
                NowPlaying.startPlaying = false;
                playSong(NowPlaying.GetCurrentSong());
            }

            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => { setNowPlayingAnimation(); });

        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            if (songTimer != null)
            {
                songTimer.Stop();
                songTimer.Tick -= SongTimer_Tick;
                songTimer = null;
            }
            GC.Collect();
            AppSettings.localSettings.Values["lastPage"] = "NowPlaying";
        }

        private void currentPlaylistGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            int index = this.currentPlaylistGridView.Items.IndexOf(e.ClickedItem);

            playSong(NowPlaying.GetSongFromIndex(index));

        }

        private void volumeSlider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            NowPlaying.player.Volume = volumeSlider.Value / 100;
        }

        private void shuffleButton_Click(object sender, RoutedEventArgs e)
        {
            if (NowPlaying.Songs.Count > 1)
            {

                NowPlaying.ShuffleSongs();
                loadUI();
                playSong(NowPlaying.GetCurrentSong());
            }

            else
            {

            }
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            if (!NowPlaying.isLoadingSong)
            {
                if (NowPlaying.currentSongIndex < NowPlaying.Songs.Count() - 1)
                {
                    playSong(NowPlaying.GetNextSong());
                }
            }
        }

        private void previousButton_Click(object sender, RoutedEventArgs e)
        {
            if (!NowPlaying.isLoadingSong)
            {
                if (NowPlaying.currentSongIndex > 0)
                {
                    playSong(NowPlaying.GetPreviousSong());
                }
                else
                    NowPlaying.player.PlaybackSession.Position = TimeSpan.Zero;
            }
        }

        private void playPause_Click(object sender, RoutedEventArgs e)
        {
            if (NowPlaying.player.PlaybackSession.PlaybackState == MediaPlaybackState.Playing)
            {
                NowPlaying.player.Pause();
                //pauseButton.Visibility = Visibility.Collapsed;
                //playButton.Visibility = Visibility.Visible;
                playButton.Style = (Style)this.Resources["customPlayButton"];
                playButton.Focus(FocusState.Programmatic);
            }

            else if (NowPlaying.player.PlaybackSession.PlaybackState == MediaPlaybackState.Paused)
            {
                //playButton.Visibility = Visibility.Collapsed;
                //pauseButton.Visibility = Visibility.Visible;
                playButton.Style = (Style)this.Resources["customPauseButton"];
                playButton.Focus(FocusState.Programmatic);

                if (NowPlaying.isSongEnded)
                    playSong(NowPlaying.GetNextSong());
                else
                    NowPlaying.player.Play();
            }
        }

        public async void playSong(Track track)
        {
            if (NewMain.Current.lastNetworkState != "No Internet Access")
            {
                NowPlaying.isLoadingSong = true;
                Uri uri;

                if (track != null)
                {

                    if (NowPlaying.isFirstPlaySinceOpen)
                        setupPlayer();

                    try
                    {
                        uri = await GetStreamUrl(NewMain.Current.mc, track);
                    }

                    catch (Exception e)
                    {
                        System.Diagnostics.Debug.Write(e);
                        uri = null;
                    }

                    if (uri != null)
                    {
                            try
                            {
                                setNowPlayingAnimation();

                                NowPlaying.player.Source = MediaSource.CreateFromUri(uri);

                            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                            {
                                trackPlayProgressBar.Maximum = (double)track.DurationMillis / 1000;
                                trackPlayProgressBar.Value = 0;

                                if (NowPlaying.GetCurrentSong().AlbumArtReference != null)
                                    albumArtImage.Source = new BitmapImage(new Uri(NowPlaying.GetCurrentSong().AlbumArtReference[0].Url));
                                else
                                    albumArtImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/logo2480x1200.png", UriKind.Absolute));
                            });

                            /*if (track.Artist.ToString().Length > 20)
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
                                clsongnametextBlock.Text = track.Title.ToString(); */


                            NowPlaying.isSongEnded = false;

                            NowPlaying.player.Play();

                                /*if (stackPanel1.Visibility == Visibility.Collapsed)
                                    stackPanel1.Visibility = Visibility.Visible;
                                if (playButton.Visibility == Visibility.Collapsed)
                                    playButton.Visibility = Visibility.Visible;
                                //if (pauseButton.Visibility == Visibility.Collapsed)
                                    //pauseButton.Visibility = Visibility.Visible;
                                //if (clCanvas.Visibility == Visibility.Collapsed)
                                //    clCanvas.Visibility = Visibility.Visible;
                                if (shuffleButton.Visibility == Visibility.Collapsed)
                                    shuffleButton.Visibility = Visibility.Visible;
                                if (volumeSlider.Visibility == Visibility.Collapsed)
                                    volumeSlider.Visibility = Visibility.Visible;*/

                                playButton.Style = (Style)this.Resources["customPauseButton"];
                            }

                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.Write(ex);
                            }

                        //});
                    }

                    else
                    {
                        playSong(NowPlaying.GetNextSong());
                        await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {

                        });
                    }
                }

                else
                {
                    playSong(NowPlaying.GetNextSong());
                    await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {

                    });
                }

                NowPlaying.isLoadingSong = false;
            }
        }

        private void setupPlayer()
        {

            NowPlaying.systemMediaTransportControls = NowPlaying.player.SystemMediaTransportControls;
            NowPlaying.player.CommandManager.IsEnabled = false;
            NowPlaying.systemMediaTransportControls.IsEnabled = true;
            NowPlaying.systemMediaTransportControls.IsPlayEnabled = true;
            NowPlaying.systemMediaTransportControls.IsPauseEnabled = true;
            NowPlaying.systemMediaTransportControls.IsNextEnabled = true;
            NowPlaying.systemMediaTransportControls.IsPreviousEnabled = true;
            NowPlaying.player.Volume = .5;


            NowPlaying.systemMediaTransportControls.ButtonPressed += SystemMediaTransportControls_ButtonPressed;
            NowPlaying.player.MediaOpened += Player_MediaOpened;
            NowPlaying.player.MediaEnded += Player_MediaEnded;
            NowPlaying.player.PlaybackSession.PlaybackStateChanged += PlaybackSession_PlaybackStateChanged;

            NowPlaying.isFirstPlaySinceOpen = false;
        }

        private async void SongTimer_Tick(object sender, object e)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => { trackPlayProgressBar.Value = NowPlaying.player.PlaybackSession.Position.TotalSeconds; });
        }

        public void loadUI()
        {
            int index = 0;

            if (currentPlaylistGridView.Items.Any())
                currentPlaylistGridView.Items.Clear();

            foreach (Track song in NowPlaying.Songs)
            {
                createNowPlayingListItem(song, index);
                index++;
            }

            if (NowPlaying.GetCurrentSong().AlbumArtReference != null)
                albumArtImage.Source = new BitmapImage(new Uri(NowPlaying.GetCurrentSong().AlbumArtReference[0].Url));
            else
                albumArtImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/logo2480x1200.png", UriKind.Absolute));

            songTimer = new DispatcherTimer();
            songTimer.Tick += SongTimer_Tick;
            songTimer.Interval = new TimeSpan(0, 0, 0, 0, 17);
            songTimer.Start();

            if (NowPlaying.player.PlaybackSession.PlaybackState == MediaPlaybackState.Playing)
                playButton.Style = (Style)this.Resources["customPauseButton"];
            else if(NowPlaying.player.PlaybackSession.PlaybackState == MediaPlaybackState.Paused)
                playButton.Style = (Style)this.Resources["customPlayButton"];


            trackPlayProgressBar.Maximum = (double)NowPlaying.GetCurrentSong().DurationMillis / 1000;
            //Task.Delay(1000);

        }

        private void createNowPlayingListItem(Track track, int index)
        {
            Grid grid1 = new Grid();

            RowDefinition rowDefinition1 = new RowDefinition();
            rowDefinition1.Height = new GridLength(100, GridUnitType.Pixel);

            // Create column definitions for first grid
            ColumnDefinition columnDefinition1 = new ColumnDefinition();
            ColumnDefinition columnDefinition2 = new ColumnDefinition();
            columnDefinition1.Width = new GridLength(100, GridUnitType.Pixel); //xbox
            columnDefinition2.Width = new GridLength(175, GridUnitType.Pixel);


            // Attached definitions to grids
            grid1.ColumnDefinitions.Add(columnDefinition1);
            grid1.ColumnDefinitions.Add(columnDefinition2);

            //Create elements
            StackPanel stack = new StackPanel();
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
            trackName.Height = 25;
            trackName.Margin = new Thickness(0, 0, 15, 0);
            trackName.HorizontalAlignment = HorizontalAlignment.Right;
            trackName.VerticalAlignment = VerticalAlignment.Top;
            trackName.FontSize = 18; //15 pc
            if (track.Title.ToString().Length > 17)
            {
                trackName.Text = track.Title.ToString().Substring(0, 16) + "...";
            }
            else
                trackName.Text = track.Title.ToString();

            trackName.Foreground = new SolidColorBrush(Colors.White);

            //Set albumName properties
            albumName.Width = double.NaN;
            albumName.Height = 20;
            albumName.Margin = new Thickness(0, 0, 15, 0);
            albumName.HorizontalAlignment = HorizontalAlignment.Right;
            albumName.VerticalAlignment = VerticalAlignment.Center;
            albumName.FontSize = 15; // 15 pc
            if (track.Album.ToString().Length > 18)
            {
                albumName.Text = track.Album.ToString().Substring(0, 17) + "...";
            }
            else
                albumName.Text = track.Album.ToString();


            albumName.Foreground = new SolidColorBrush(Colors.White);

            //Set artistName properties
            artistName.Width = double.NaN;
            artistName.Height = 20;
            artistName.Margin = new Thickness(0, 0, 15, 0);
            artistName.HorizontalAlignment = HorizontalAlignment.Right;
            artistName.VerticalAlignment = VerticalAlignment.Bottom;
            artistName.FontSize = 15; //15 pc
            if (track.Artist.ToString().Length > 18)
            {
                artistName.Text = track.Artist.ToString().Substring(0, 17) + "...";
            }
            else
                artistName.Text = track.Artist.ToString();

            artistName.Foreground = new SolidColorBrush(Colors.White);

            stack.Margin = new Thickness(0, 0, 10, 0);

            stack.Children.Add(trackName);
            stack.Children.Add(albumName);
            stack.Children.Add(artistName);
            //Assign elements to grid and row/columns
            grid1.Children.Add(image);
            grid1.Children.Add(stack);
            //grid1.Children.Add(trackName);
            //grid1.Children.Add(albumName);
            //grid1.Children.Add(artistName);

            Grid.SetColumn(image, 0);
            Grid.SetColumn(stack, 1);
            //Grid.SetColumn(trackName, 1);
            //Grid.SetColumn(albumName, 1);
            //Grid.SetColumn(artistName, 1);

            Grid.SetRow(image, 0);
            Grid.SetRow(stack, 0);
           // Grid.SetRow(trackName, 0);
            //Grid.SetRow(albumName, 0);
            //Grid.SetRow(artistName, 0);

            grid1.BorderBrush = new SolidColorBrush(Colors.Black);
            grid1.BorderThickness = new Thickness(0.2);
            grid1.Margin = new Thickness(0, 1, 0, 0);
            grid1.Name = "grid" + index;

            currentPlaylistGridView.Items.Add(grid1);
        }

        private async void setNowPlayingAnimation()
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {

                if (NowPlaying.Songs.Any())
                {
                    if (NowPlaying.prevSongIndex != NowPlaying.currentSongIndex)
                    {
                        try
                        {
                            var lastItem = currentPlaylistGridView.ContainerFromIndex(NowPlaying.prevSongIndex) as GridViewItem;
                            var lastGrid = lastItem.FindName("grid" + NowPlaying.prevSongIndex) as Grid;
                                lastGrid.Background = new SolidColorBrush(Colors.Transparent);
                        }

                        catch (Exception e)
                        {
                            System.Diagnostics.Debug.Write(e);

                        }
                    }

                    var item = currentPlaylistGridView.ContainerFromIndex(NowPlaying.currentSongIndex) as GridViewItem;
                    var grid = item.FindName("grid" + NowPlaying.currentSongIndex) as Grid;

                    grid.Background = new SolidColorBrush(Colors.Green);
                    currentPlaylistGridView.ScrollIntoView(currentPlaylistGridView.Items[NowPlaying.currentSongIndex], ScrollIntoViewAlignment.Leading);

                }
            });
        }

        private void PlaybackSession_PlaybackStateChanged(MediaPlaybackSession sender, object args)
        {
            switch (NowPlaying.player.PlaybackSession.PlaybackState)
            {
                case MediaPlaybackState.Playing:
                    NowPlaying.systemMediaTransportControls.PlaybackStatus = MediaPlaybackStatus.Playing;
                    break;
                case MediaPlaybackState.Paused:
                    NowPlaying.systemMediaTransportControls.PlaybackStatus = MediaPlaybackStatus.Paused;
                    break;
                case MediaPlaybackState.None:
                    NowPlaying.systemMediaTransportControls.PlaybackStatus = MediaPlaybackStatus.Closed;
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
                        playButton.Style = (Style)this.Resources["customPauseButton"];
                        //playButton.Visibility = Visibility.Collapsed;
                        //pauseButton.Visibility = Visibility.Visible;
                        if (NowPlaying.isSongEnded)
                            playSong(NowPlaying.Songs[NowPlaying.currentSongIndex]);
                        else
                            NowPlaying.player.Play();
                    });
                    break;
                case SystemMediaTransportControlsButton.Pause:
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        playButton.Style = (Style)this.Resources["customPlayButton"];
                        //pauseButton.Visibility = Visibility.Collapsed;
                        //playButton.Visibility = Visibility.Visible;
                        NowPlaying.player.Pause();
                    });
                    break;
                case SystemMediaTransportControlsButton.Next:
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        if (NowPlaying.currentSongIndex < NowPlaying.Songs.Count() - 1)
                        {
                            playSong(NowPlaying.GetNextSong());
                        }
                    });
                    break;
                case SystemMediaTransportControlsButton.Previous:
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        if (NowPlaying.currentSongIndex > 0)
                        {
                            playSong(NowPlaying.GetPreviousSong());
                        }
                        else
                            NowPlaying.player.PlaybackSession.Position = TimeSpan.Zero;
                    });
                    break;
                default:
                    break;
            }
        }

        private void Player_MediaOpened(MediaPlayer sender, object args)
        {
            //playButton.Style = (Style)this.Resources["customPauseButton"];
            // Get the updater.
            SystemMediaTransportControlsDisplayUpdater updater = NowPlaying.systemMediaTransportControls.DisplayUpdater;
            updater.Type = MediaPlaybackType.Music;
            updater.MusicProperties.AlbumArtist = NowPlaying.GetCurrentSong().Artist.ToString();
            updater.MusicProperties.AlbumTitle = NowPlaying.GetCurrentSong().Album.ToString();
            updater.MusicProperties.Title = NowPlaying.GetCurrentSong().Title.ToString();

            // Set the album art thumbnail.
            // RandomAccessStreamReference is defined in Windows.Storage.Streams
            if (NowPlaying.GetCurrentSong().AlbumArtReference[0].Url != null)
                updater.Thumbnail = RandomAccessStreamReference.CreateFromUri(new Uri(NowPlaying.GetCurrentSong().AlbumArtReference[0].Url));
            else
                updater.Thumbnail = RandomAccessStreamReference.CreateFromUri(new Uri("Assets/logo2480x1200.png", UriKind.Relative));

            updater.Update();

        }

        private async void Player_MediaEnded(MediaPlayer sender, object args)
        {
            //await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => { NowPlaying.songTimer.Stop(); });

            NowPlaying.isSongEnded = true;

            if (NowPlaying.currentSongIndex < NowPlaying.Songs.Count() - 1)
            {
                NowPlaying.isSongEnded = false;
                playSong(NowPlaying.GetNextSong());
            }

            else
            {
                await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    playButton.Style = (Style)this.Resources["customPlayButton"];
                    //pauseButton.Visibility = Visibility.Collapsed;
                    //playButton.Visibility = Visibility.Visible;
                    //clCanvas.Visibility = Visibility.Collapsed;

                    if (NowPlaying.currentSongIndex == NowPlaying.Songs.Count() - 1)
                    {

                    }
                });
            }
        }

        public async Task<Uri> GetStreamUrl(MobileClient mc, Track track)
        {
            Uri data;

            data = await mc.GetStreamUrlAsync(track);

            return data;
        }
    }
}
