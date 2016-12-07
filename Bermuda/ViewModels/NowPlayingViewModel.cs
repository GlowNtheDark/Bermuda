using Bermuda.Services;
using GoogleMusicApi.UWP.Common;
using GoogleMusicApi.UWP.Structure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Bermuda.ViewModels
{
    public class NowPlayingViewModel : INotifyPropertyChanged, IDisposable
    {
        //bool disposed;

        public MediaPlayer player;
        CoreDispatcher dispatcher;
        MediaPlaybackItem playbackItem;

        //public PlaylistViewModel playList;
        TrackListViewModel songList;
        public PlaybackSessionViewModel PlaybackSession { get; private set; }

        //public ObservableCollection<Track> songList;
        
        public event PropertyChangedEventHandler PropertyChanged;

        public NowPlayingViewModel(MediaPlayer player, CoreDispatcher dispatcher)
        {
            this.player = player;
            this.dispatcher = dispatcher;
            PlaybackSession = new PlaybackSessionViewModel(player.PlaybackSession, dispatcher);
            player.SourceChanged += Player_SourceChanged;
        }

        private async void Player_SourceChanged(MediaPlayer sender, object args)
        {
            /*await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                SongList[PlayerService.Instance.previousSongIndex].tileColor = new Windows.UI.Xaml.Media.SolidColorBrush(Colors.Transparent);
            });

            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                SongList[PlayerService.Instance.currentSongIndex].tileColor = new Windows.UI.Xaml.Media.SolidColorBrush(Colors.Green);
            });

            await Task.Delay(1000);

            RaisePropertyChanged("SongList");*/

            SongList.CurrentItem.setTileColor();
        }

        public void skipPrevious()
        {
          
        }

        public async void skipNext()
        {
            PlayerService.Instance.previousSongIndex = PlayerService.Instance.currentSongIndex;

            PlayerService.Instance.currentSongIndex++;

            player.Source = new MediaPlaybackItem(MediaSource.CreateFromUri(await GetStreamUrl(NewMain.Current.mc, SongList[PlayerService.Instance.currentSongIndex].song)));

            player.Play();
        }

        public void volumeChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            player.Volume = e.NewValue;
        }

        public void togglePlayPause()
        {
            switch (PlaybackSession.PlaybackState)
            {
                case MediaPlaybackState.Playing:
                    player.Pause();
                    break;
                case MediaPlaybackState.Paused:
                    player.Play();
                    break;
            }
        }

       /* public PlaylistViewModel PlayList
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
        }*/

        public TrackListViewModel SongList
        {
            get { return songList; }

            set
            {
                if (songList != value)
                {
                    songList = value;
                    RaisePropertyChanged("SongList");
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
            if (SongList != null)
            {
                SongList.Dispose();
                SongList = null; // Setter triggers vector unsubscribe logic
            }

            player.SourceChanged -= Player_SourceChanged;

            PlaybackSession.Dispose();
        }
    }
}
