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

        MediaPlaybackItem PlaybackItem
        {
            get { return Player.Source as MediaPlaybackItem; }
            set { Player.Source = value; }
        }

        Track[] MediaList
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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if(MediaList != null && PlaybackItem != null)
                PlayerViewModel.PlayList = new PlaylistViewModel(MediaList, PlaybackItem, Dispatcher);
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            PlayerViewModel.Dispose();
            PlayerViewModel = null;
            GC.Collect();
            AppSettings.localSettings.Values["lastPage"] = "NowPlaying";
        }

        private void currentPlaylistGridView_ItemClick(object sender, ItemClickEventArgs e)
        {

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
