using Bermuda.DataModels;
using GoogleMusicApi.UWP.Structure;
using System;
using System.ComponentModel;
using Windows.Media.Playback;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;

namespace Bermuda.ViewModels
{
    public class SearchViewModel : INotifyPropertyChanged, IDisposable
    {
        MediaPlayer Player;
        TrackList SongList;
        CoreDispatcher dispatcher;
        AlbumListViewModel alviewmodel;

        public event PropertyChangedEventHandler PropertyChanged;

        public SearchViewModel(MediaPlayer player, TrackList tracklist, CoreDispatcher dispatcher)
        {
            this.Player = player;
            this.SongList = tracklist;
            this.dispatcher = dispatcher;
        }

        public AlbumListViewModel ALViewModel
        {
            get { return alviewmodel; }

            set
            {
                if (alviewmodel != value)
                {
                    alviewmodel = value;
                    RaisePropertyChanged("ALViewModel");
                }
            }
        }

        public void AlbumItemClick()
        {

        }

        public async void QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            SearchResponse response = await NewMain.Current.mc.SearchAsync(args.QueryText, 3); //3 for Album Search

            ALViewModel = new AlbumListViewModel(response, dispatcher);

        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {

        }
    }
}
