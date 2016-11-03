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
using Bermuda.ViewModels;
using Bermuda.Services;
using System.Collections.Generic;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Bermuda
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NowPlayingScene : Page
    {

        public NowPlayingViewModel PlayerViewModel { get; set; }

        MediaPlayer Player => PlayerService.Instance.Player;

        MediaPlaybackList PlaybackList
        {
            get { return Player.Source as MediaPlaybackList; }
            set { Player.Source = value; }
        }
        List<Track> MediaList
        {
            get { return PlayerService.Instance.songList; }
            set { PlayerService.Instance.songList = value; }
        }

        public NowPlayingScene()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Disabled;

            PlayerViewModel = new NowPlayingViewModel(Player, Dispatcher);
        }

        public DispatcherTimer songTimer;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if(MediaList != null && PlaybackList != null)
                PlayerViewModel.PlayList = new PlaylistViewModel(MediaList, PlaybackList, Dispatcher);
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
            NowPlaying.player.PlaybackSession.PlaybackStateChanged += PlaybackSession_PlaybackStateChanged;

            NowPlaying.isFirstPlaySinceOpen = false;
        }

        private async void SongTimer_Tick(object sender, object e)
        {
            await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => { trackPlayProgressBar.Value = NowPlaying.player.PlaybackSession.Position.TotalSeconds; });
        }

        public void loadUI()
        {
            songTimer = new DispatcherTimer();
            songTimer.Tick += SongTimer_Tick;
            songTimer.Interval = new TimeSpan(0, 0, 0, 0, 17);
            songTimer.Start();
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

        private  void setNowPlayingAnimation()
        {
            /*await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {

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
            });*/
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
                        //Player Play()

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
                        if (NowPlaying.isLoadingSong)
                        {
                            //Start next song
                        }
                    });
                    break;
                case SystemMediaTransportControlsButton.Previous:
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        if (NowPlaying.currentSongIndex > 0)
                        {
                            //Start last song
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
            // Get the updater.
            SystemMediaTransportControlsDisplayUpdater updater = NowPlaying.systemMediaTransportControls.DisplayUpdater;
            updater.Type = MediaPlaybackType.Music;

            //Need to remodel for playlist.currentitem

            /*updater.MusicProperties.AlbumArtist = NowPlaying.GetCurrentSong().Artist.ToString();
            updater.MusicProperties.AlbumTitle = NowPlaying.GetCurrentSong().Album.ToString();
            updater.MusicProperties.Title = NowPlaying.GetCurrentSong().Title.ToString();

            // Set the album art thumbnail.
            // RandomAccessStreamReference is defined in Windows.Storage.Streams
            if (NowPlaying.GetCurrentSong().AlbumArtReference[0].Url != null)
                updater.Thumbnail = RandomAccessStreamReference.CreateFromUri(new Uri(NowPlaying.GetCurrentSong().AlbumArtReference[0].Url));
            else
                updater.Thumbnail = RandomAccessStreamReference.CreateFromUri(new Uri("Assets/logo2480x1200.png", UriKind.Relative));

            updater.Update();*/

        }
    }
}
