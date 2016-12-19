using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using GoogleMusicApi.UWP.Structure;
using Bermuda.ViewModels;
using Bermuda.Services;
using Bermuda.DataModels;
using Windows.Media.Playback;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Bermuda
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PlaylistsScene : Page
    {
        public PlaylistViewModel PLViewModel { get; set; }

        MediaPlayer Player => PlayerService.Instance.Player;

        TrackList MediaList
        {
            get { return PlayerService.Instance.CurrentPlaylist; }
            set { PlayerService.Instance.CurrentPlaylist = value; }
        }

        public PlaylistsScene()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Disabled;

            PLViewModel = new PlaylistViewModel(Player, MediaList, this.Dispatcher);

            PlayerService.Instance.dispatcher = this.Dispatcher;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            PLViewModel.GroupViewModel = new PlaylistGroupViewModel(this.Dispatcher, PLViewModel);
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {

        }

    }
}
