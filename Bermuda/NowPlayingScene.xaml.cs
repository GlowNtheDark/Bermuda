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
using System.Collections.ObjectModel;
using Bermuda.DataModels;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Bermuda
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public partial class NowPlayingScene : Page
    {

        public NowPlayingViewModel PlayerViewModel { get; set; }
        
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

        public NowPlayingScene()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Disabled;

            if(MediaList != null)
                PlayerViewModel = new NowPlayingViewModel(Player, this.Dispatcher);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MediaList != null && PlaybackItem != null)
                    PlayerViewModel.SongList = new TrackListViewModel(MediaList, this.Dispatcher);
            }
            catch(Exception ex)
            {

            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            if (PlayerViewModel != null)
            {
                PlayerViewModel.Dispose();
                PlayerViewModel = null;
                GC.Collect();
            }
            AppSettings.localSettings.Values["lastPage"] = "NowPlaying";
        }

    }
}
