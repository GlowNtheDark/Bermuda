﻿using Bermuda.DataModels;
using Bermuda.Services;
using GoogleMusicApi.UWP.Structure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Bermuda.ViewModels
{
    public class TrackListViewModel : ObservableCollection<TrackViewModel>, IDisposable
    {
        public CoreDispatcher dispatcher;
        int currentItemIndex => PlayerService.Instance.currentSongIndex;
        int previousItemIndex => PlayerService.Instance.previousSongIndex;
        bool disposed;
        bool initializing;

        public TrackList SongList { get; private set; }

        public TrackViewModel CurrentItem
        {
            get { return currentItemIndex == -1 ? null : this[currentItemIndex]; }
            set
            {
                if (value == null)
                {
                    CurrentItemIndex = -1;
                    return;
                }

                if (currentItemIndex == -1 || this[currentItemIndex] != value)
                {
                    CurrentItemIndex = IndexOf(value);
                }
            }
        }

        public TrackViewModel PreviousItem
        {
            get { return previousItemIndex == -1 ? null : this[previousItemIndex]; }
            set
            {
                if (value == null)
                {
                    PreviousItemIndex = -1;
                    return;
                }

                if (previousItemIndex == -1 || this[previousItemIndex] != value)
                {
                    PreviousItemIndex = IndexOf(value);
                }
            }
        }

        public int CurrentItemIndex
        {
            get
            {
                return currentItemIndex;
            }
            set
            {
                if (currentItemIndex != value)
                {
                    //currentItemIndex = value;

                    OnPropertyChanged(new PropertyChangedEventArgs("CurrentItemIndex"));
                    OnPropertyChanged(new PropertyChangedEventArgs("CurrentItem"));
                }
            }
        }

        public int PreviousItemIndex
        {
            get
            {
                return previousItemIndex;
            }
            set
            {
                if (previousItemIndex != value)
                {
                    //currentItemIndex = value;

                    OnPropertyChanged(new PropertyChangedEventArgs("PreviousItemIndex"));
                    OnPropertyChanged(new PropertyChangedEventArgs("PreviousItem"));
                }
            }
        }

        public void setCurrentTileDefault()
        {
            CurrentItem.setTileColorDefault();
        }

        public async void Update()
        {
            await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                CurrentItem.Update();
                PreviousItem.Update();
                OnPropertyChanged(new PropertyChangedEventArgs("CurrentItem"));
                OnPropertyChanged(new PropertyChangedEventArgs("PreviousItem"));
            });
        }

        public TrackListViewModel(TrackList trackList, CoreDispatcher dispatcher, Playlist playlist)
        {
            try
            {
                SongList = trackList;
                this.dispatcher = dispatcher;

                // Initialize the view model items
                initializing = true;

                foreach (var mediaItem in trackList)
                    Add(new TrackViewModel(this, mediaItem, playlist));

                initializing = false;

                // The view model supports TwoWay binding so update when the playback list item changes
                //PlaybackList.CurrentItemChanged += PlaybackList_CurrentItemChanged;

                // Start where the playback list is currently at
                CurrentItemIndex = (int)PlayerService.Instance.currentSongIndex;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }
        }

        public TrackListViewModel(TrackList trackList, CoreDispatcher dispatcher)
        {
            try
            {
                SongList = trackList;
                this.dispatcher = dispatcher;

                // Initialize the view model items
                initializing = true;

                foreach (var mediaItem in trackList)
                    Add(new TrackViewModel(this, mediaItem));

                initializing = false;

                // The view model supports TwoWay binding so update when the playback list item changes
                //PlaybackList.CurrentItemChanged += PlaybackList_CurrentItemChanged;

                // Start where the playback list is currently at
                CurrentItemIndex = (int)PlayerService.Instance.currentSongIndex;
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }
        }

        public TrackListViewModel(SearchResponse response, CoreDispatcher dispatcher)
        {
            try
            {
                this.dispatcher = dispatcher;

                // Initialize the view model items
                initializing = true;

                foreach (SearchResult result in response.Entries)
                {
                    if(result.Track != null)
                        Add(new TrackViewModel(this, result.Track));
                }

                initializing = false;

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write(ex);
            }
        }

        public TrackListViewModel()
        {

        }

        public void Dispose()
        {

        }
    }
}
