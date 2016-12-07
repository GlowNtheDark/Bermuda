using Bermuda.DataModels;
using Bermuda.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;

namespace Bermuda.ViewModels
{
    public class TrackListViewModel : ObservableCollection<TrackViewModel>, IDisposable
    {
        public CoreDispatcher dispatcher;
        int currentItemIndex = -1;
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
                    currentItemIndex = value;

                    OnPropertyChanged(new PropertyChangedEventArgs("CurrentItemIndex"));
                    OnPropertyChanged(new PropertyChangedEventArgs("CurrentItem"));
                }
            }
        }

        public TrackListViewModel(TrackList trackList, CoreDispatcher dispatcher)
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

        public void Dispose()
        {

        }
    }
}
