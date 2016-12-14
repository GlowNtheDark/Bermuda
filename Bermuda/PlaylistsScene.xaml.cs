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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Bermuda
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PlaylistsScene : Page
    {
        public PlaylistsScene()
        {
            this.InitializeComponent();
        }

        private async void TempButton_Click(object sender, RoutedEventArgs e)
        {
            ResultList<Playlist> result = new ResultList<Playlist>();
            List<Track> tracks = new List<Track>();

            result = await NewMain.Current.mc.ListPlaylistsAsync();

            tracks = await NewMain.Current.mc.ListTracksFromPlaylist(result.Data.Items[0]);

            foreach(Track track in tracks)
                TempListBox.Items.Add(track.Title);
        }
    }
}
