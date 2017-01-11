using Bermuda.DataModels;
using Bermuda.Services;
using Bermuda.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Playback;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Bermuda
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SearchScene : Page
    {
        public SearchViewModel searchviewmodel {get; set;}

        MediaPlayer Player => PlayerService.Instance.Player;

        MediaPlaybackItem PlaybackItem
        {
            get { return Player.Source as MediaPlaybackItem; }
            set { Player.Source = value; }
        }


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

        public SearchScene()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Disabled;

            searchviewmodel = new SearchViewModel(Player, MediaList, this.Dispatcher, MessageViewModel);

            PlayerService.Instance.dispatcher = this.Dispatcher;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            searchviewmodel = null;
            GC.Collect();
            AppSettings.localSettings.Values["lastPage"] = "Search";
        }
    }
}
