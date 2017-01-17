using Bermuda.DataModels;
using Bermuda.Services;
using GoogleMusicApi.UWP.Common;
using GoogleMusicApi.UWP.Requests.Data;
using GoogleMusicApi.UWP.Structure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace Bermuda.ViewModels
{
    public class PlaylistViewModel : INotifyPropertyChanged
    {
        CoreDispatcher dispatcher;
        MediaPlayer Player;
        PlaylistGroupViewModel groupviewmodel;
        TrackListViewModel tlviewmodel;
        TrackList MediaList;
        MessagingViewModel MessageViewModel;
        ColorListViewModel colorlistviewmodel;

        bool disposed;
        bool initializing;

        public event PropertyChangedEventHandler PropertyChanged;

        public PlaylistViewModel(MediaPlayer player, TrackList medialist, CoreDispatcher dispatcher, MessagingViewModel MessageViewModel, ColorListViewModel colorlistviewmodel)
        {
            this.Player = player;
            this.dispatcher = dispatcher;
            this.MediaList = medialist;
            this.MessageViewModel = MessageViewModel;
            this.colorlistviewmodel = colorlistviewmodel;
            TLViewModel = new TrackListViewModel();
        }

        public TrackListViewModel TLViewModel
        {
            get { return tlviewmodel; }

            set
            {
                if (tlviewmodel != value)
                {
                    tlviewmodel = value;
                    RaisePropertyChanged("TLViewModel");
                }
            }
        }

        public PlaylistGroupViewModel GroupViewModel
        {
            get { return groupviewmodel; }

            set
            {
                if (groupviewmodel != value)
                {
                    groupviewmodel = value;
                    RaisePropertyChanged("GroupViewModel");
                }
            }
        }

        public void playlistSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ListView lv = sender as ListView;

                if (lv.SelectedItem != null)
                {
                    int index = lv.Items.IndexOf(lv.SelectedItem);

                    if (groupviewmodel[index].playlist != null)
                        getPlaylistTracks(groupviewmodel[index].playlist);
                }
            }

            catch(Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);

                MessageViewModel.MLViewModel.Add(new MessageItemViewModel("Unexpected error -- " + ex));
                MessageViewModel.ShowAlert();
            }
        }

        public void playlistClicked(object sender, ItemClickEventArgs e)
        {
            PlaylistItemViewModel item = e.ClickedItem as PlaylistItemViewModel;
            item.openCloseMenu();
            RaisePropertyChanged("GroupViewModel");
        }

        public void listItemClicked(object sender, ItemClickEventArgs e)
        {
            TrackViewModel item = e.ClickedItem as TrackViewModel;
            item.openCloseMenu();
            RaisePropertyChanged("TLViewModel");
        }

        public async void playlistItemMenuClicked(object sender, ItemClickEventArgs e)
        {
            GridView gv = sender as GridView;
            StackPanel sp = e.ClickedItem as StackPanel;
            var itemviewmodel = gv.DataContext as PlaylistItemViewModel;

            int menuIndex = gv.Items.IndexOf(sp.Parent);

            if (menuIndex == 0) //Add to queue
            {
                foreach (Track track in TLViewModel.SongList)
                {
                    if (track != null)
                        MediaList.Add(track);
                }

                if (Player.Source == null)
                {
                    Player.Source = new MediaPlaybackItem(MediaSource.CreateFromUri(await GetStreamUrl(NewMain.Current.mc, TLViewModel.SongList[0])));
                    Player.Play();
                }

                PlayerService.Instance.isRadioMode = false;
                itemviewmodel.showCheckMark(0);
            }

            else if (menuIndex == 1) //Clear and add to queue
            {
                MediaList.Clear();
                PlayerService.Instance.previousSongIndex = 0;
                PlayerService.Instance.currentSongIndex = 0;

                foreach (Track track in TLViewModel.SongList)
                {
                    if (track != null)
                        MediaList.Add(track);
                }

                PlayerService.Instance.isRadioMode = false;
                Player.Source = new MediaPlaybackItem(MediaSource.CreateFromUri(await GetStreamUrl(NewMain.Current.mc, TLViewModel.SongList[0])));
                Player.Play();

                itemviewmodel.showCheckMark(1);
            }

            else // Delete Playlist
            {
                MutateResponse response = await NewMain.Current.mc.DeletePlaylist(itemviewmodel.playlist);

                if (response.ResponseMutation[0].Deleted)
                {
                    GroupViewModel.Dispose();
                    GroupViewModel = null;
                    GroupViewModel = new PlaylistGroupViewModel(dispatcher, this, MessageViewModel, colorlistviewmodel);
                    await Task.Delay(1000);
                    TLViewModel.Clear();
                }
                else
                {
                    //show some error
                    MessageViewModel.MLViewModel.Add(new MessageItemViewModel("Playlist couldn't be deleted."));
                    MessageViewModel.ShowAlert();
                }
            }
        }

        public async void getPlaylistTracks(Playlist playlist)
        {
            List<Track> templist = new List<Track>();
            TrackList list = new TrackList();
            templist = await NewMain.Current.mc.ListTracksFromPlaylist(playlist);

            if (templist != null)
            {
                foreach (Track track in templist)
                {
                    if (track != null)
                    {
                        Plentry plentry = NewMain.Current.mc.GetTrackPlaylistEntry(playlist, track);

                        if (plentry != null)
                            if (!plentry.Deleted)
                                list.Add(plentry.Track);
                    }
                }

                TLViewModel = new TrackListViewModel(list, dispatcher, playlist, MessageViewModel, colorlistviewmodel);
            }
        }

        public static async Task<Uri> GetStreamUrl(MobileClient mc, Track track)
        {
            Uri data;

            data = await mc.GetStreamUrlAsync(track);

            return data;
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
