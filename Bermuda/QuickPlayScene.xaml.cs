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
using System.ComponentModel;
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
    public partial class QuickPlayScene : Page
    {
        public QuickPlayViewModel QPViewModel { get; set; }


        public MessagingViewModel MessageViewModel
        {
            get { return MessagingService.Instance.MessageViewModel; }
            set { MessagingService.Instance.MessageViewModel = value; }
        }

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

        public QuickPlayScene()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Disabled;

            QPViewModel = new QuickPlayViewModel(Player, MediaList, this.Dispatcher, MessageViewModel);

            PlayerService.Instance.dispatcher = this.Dispatcher;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            QPViewModel.QPAlbumViewModel = new QuickPlayAlbumViewModel(this.Dispatcher, QPViewModel);
            QPViewModel.QPRadioViewModel = new QuickPlayRadioViewModel(this.Dispatcher, QPViewModel);
            QPViewModel.getListenNow();       
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            QPViewModel = null;
            GC.Collect();
            AppSettings.localSettings.Values["lastPage"] = "QuickPlay";
        }

    }
}
