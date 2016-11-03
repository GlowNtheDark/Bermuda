using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Playback;
using Windows.UI.Core;

namespace Bermuda.ViewModels
{
    public class NowPlayingViewModel : INotifyPropertyChanged, IDisposable
    {
        bool disposed;

        public MediaPlayer player;
        CoreDispatcher dispatcher;
        MediaPlaybackList playbackList;

        public PlaylistViewModel playList;

        public PlaybackSessionViewModel PlaybackSession { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public NowPlayingViewModel(MediaPlayer player, CoreDispatcher dispatcher)
        {
            this.player = player;
            this.dispatcher = dispatcher;
            PlaybackSession = new PlaybackSessionViewModel(player.PlaybackSession, dispatcher);
        }

        public void skipPrevious()
        {
          
            //RaisePropertyChanged("playList");
            //RaisePropertyChanged("PlaybackSession");
        }

        public void skipNext()
        {
            var list = player.Source as MediaPlaybackList;
            if (list == null)
                return;

            list.MoveNext();
        }

        public PlaylistViewModel PlayList
        {
            get { return playList; }
            set
            {
                if (playList != value)
                {
                    if (playbackList != null)
                    {
                        playbackList.CurrentItemChanged -= playbackList_CurrentItemChanged;
                        playbackList = null;
                    }

                    playList = value;

                    if (playList != null)
                    {
                        if (player.Source != playList.playbackList)
                            player.Source = playList.playbackList;

                        playbackList = playList.playbackList;
                        playbackList.CurrentItemChanged += playbackList_CurrentItemChanged;

                    }
                    else
                    {
                        //CanSkipNext = false;
                        //CanSkipPrevious = false;
                    }

                    RaisePropertyChanged("PlayList");
                }
            }
        }

        private void playbackList_CurrentItemChanged(MediaPlaybackList sender, CurrentMediaPlaybackItemChangedEventArgs args)
        {

        }

        private void RaisePropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            if (disposed)
                return;

            disposed = true;
        }
    }
}
