﻿using Bermuda.DataModels;
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
        //public PlaylistGroup PlGroup { get; private set; }
        public PlaylistViewModel PLViewModel;
        public CoreDispatcher dispatcher;
        MessagingViewModel MessageViewModel;
        bool disposed;
        bool initializing;

        public PlaylistGroupViewModel(CoreDispatcher dispatcher, PlaylistViewModel plviewmodel, MessagingViewModel MessageViewModel)
        {
            this.dispatcher = dispatcher;
            this.PLViewModel = plviewmodel;
            this.MessageViewModel = MessageViewModel;
            getPlaylists();
        }

        public async void getPlaylists()
        {
            ResultList<Playlist> result = new ResultList<Playlist>();

            if (result != null)
            {

                result = await NewMain.Current.mc.ListPlaylistsAsync();

                foreach (var playlistitem in result.Data.Items)
                {
                    if(playlistitem != null)
                        if (playlistitem.Deleted != true)
                            Add(new PlaylistItemViewModel(playlistitem, PLViewModel));
                }
            }

            else
            {
                MessageViewModel.MLViewModel.Add(new MessageItemViewModel("Error getting playlist tracks."));
                MessageViewModel.ShowAlert();
            }
        }

        public void Dispose()
        {

        }
    }
}
