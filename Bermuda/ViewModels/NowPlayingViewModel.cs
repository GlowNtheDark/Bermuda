using GoogleMusicApi.UWP.Common;
using GoogleMusicApi.UWP.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.UI.Core;

namespace Bermuda.ViewModels
{
    public class NowPlayingViewModel : INotifyPropertyChanged, IDisposable
    {
        bool disposed;

        public MediaPlayer player;
        CoreDispatcher dispatcher;
        MediaPlaybackItem playbackItem;

        public PlaylistViewModel playList;

        public PlaybackSessionViewModel PlaybackSession { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public NowPlayingViewModel(MediaPlayer player, CoreDispatcher dispatcher)
        {
            this.player = player;
            this.dispatcher = dispatcher;
            PlaybackSession = new PlaybackSessionViewModel(player.PlaybackSession, dispatcher);
            player.MediaEnded += Player_MediaEnded;
        }

        private async void Player_MediaEnded(MediaPlayer sender, object args)
        {
            playList.currentItemIndex++;
            player.Source = new MediaPlaybackItem(MediaSource.CreateFromUri(await GetStreamUrl(NewMain.Current.mc, playList.songList[playList.currentItemIndex])));
            playList.setCurrentMediaItem();
            player.Play();
        }

        public void skipPrevious()
        {
          
            //RaisePropertyChanged("playList");
            //RaisePropertyChanged("PlaybackSession");
        }

        public async void skipNext()
        {
            playList.currentItemIndex++;
            player.Source = new MediaPlaybackItem(MediaSource.CreateFromUri(await GetStreamUrl(NewMain.Current.mc, playList.songList[playList.currentItemIndex])));
            playList.setCurrentMediaItem();
            player.Play();
        }

        public void togglePlayPause()
        {
            switch (player.PlaybackSession.PlaybackState)
            {
                case MediaPlaybackState.Playing:
                    player.Pause();
                    break;
                case MediaPlaybackState.Paused:
                    player.Play();
                    break;
            }
        }

        public PlaylistViewModel PlayList
        {
            get { return playList; }
            set
            {
                if (playList != value)
                {
                    if (playbackItem != null)
                    {
                        playbackItem = null;
                    }

                    playList = value;

                    if (playList != null)
                    {
                        if (player.Source != playList.playbackItem)
                            player.Source = playList.playbackItem;

                        playbackItem = playList.playbackItem;

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
            if (disposed)
                return;

            disposed = true;
        }
    }
}
