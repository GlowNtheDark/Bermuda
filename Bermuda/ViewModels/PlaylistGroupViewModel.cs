using Bermuda.DataModels;
using GoogleMusicApi.UWP.Structure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace Bermuda.ViewModels
{
    public class PlaylistGroupViewModel : ObservableCollection<PlaylistItemViewModel>, IDisposable
    {
        public PlaylistGroup PlGroup { get; private set; }
        public CoreDispatcher dispatcher;
        bool disposed;
        bool initializing;

        public PlaylistGroupViewModel(CoreDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;

            getPlaylists();
        }

        public async void getPlaylists()
        {
            ResultList<Playlist> result = new ResultList<Playlist>();

            result = await NewMain.Current.mc.ListPlaylistsAsync();

            foreach (var playlistitem in result.Data.Items)
                Add(new PlaylistItemViewModel(playlistitem));
        }

        public void Dispose()
        {

        }
    }
}
