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

        public ColorListViewModel CLViewModel { get; set; }

        MediaPlayer Player => PlayerService.Instance.Player;

        public MessagingViewModel MessageViewModel
        {
            get { return MessagingService.Instance.MessageViewModel; }
            set { MessagingService.Instance.MessageViewModel = value; }
        }

        TrackList MediaList
        {
            get { return PlayerService.Instance.CurrentPlaylist; }
            set { PlayerService.Instance.CurrentPlaylist = value; }
        }

        public PlaylistsScene()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Disabled;

            CLViewModel = new ColorListViewModel();

            PLViewModel = new PlaylistViewModel(Player, MediaList, this.Dispatcher, MessageViewModel, CLViewModel);

            PlayerService.Instance.dispatcher = this.Dispatcher;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            PLViewModel.GroupViewModel = new PlaylistGroupViewModel(this.Dispatcher, PLViewModel, MessageViewModel, CLViewModel);
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            PLViewModel = null;
            CLViewModel = null;
            GC.Collect();
            AppSettings.localSettings.Values["lastPage"] = "Playlists";
        }

    }
}
