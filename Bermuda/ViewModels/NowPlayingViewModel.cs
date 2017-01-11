using Bermuda.Services;
using GoogleMusicApi.UWP.Common;
using GoogleMusicApi.UWP.Structure;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
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
        bool disposed;
        MessagingViewModel MessageViewModel;
        public MediaPlayer player;
        CoreDispatcher dispatcher;
        TrackListViewModel songList;
        bool canSkipNext;
        bool canSkipPrevious;
        public PlaybackSessionViewModel PlaybackSession { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool CanSkipNext
        {
            get { return canSkipNext; }
            set
            {
                if (canSkipNext != value)
                {
                    canSkipNext = value;
                    RaisePropertyChanged("CanSkipNext");
                }
            }
        }

        public bool CanSkipPrevious
        {
            get { return canSkipPrevious; }
            set
            {
                if (canSkipPrevious != value)
                {
                    canSkipPrevious = value;
                    RaisePropertyChanged("CanSkipPrevious");
                }
            }
        }

        public NowPlayingViewModel(MediaPlayer player, CoreDispatcher dispatcher, MessagingViewModel MessageViewModel)
        {
            this.player = player;
            this.dispatcher = dispatcher;
            this.MessageViewModel = MessageViewModel;
            PlaybackSession = new PlaybackSessionViewModel(player.PlaybackSession, dispatcher);
            player.SourceChanged += Player_SourceChanged;

            if (PlayerService.Instance.CurrentPlaylist.Count == 0)
            {
                CanSkipNext = false;
                CanSkipPrevious = false;
            }
            else
            {
                CanSkipNext = true;
                CanSkipPrevious = true;
            }
        }

        private void Player_SourceChanged(MediaPlayer sender, object args)
        {
            SongList.Update();
        }

        public async void skipPrevious()
        {
            if (!CanSkipPrevious)
                return;

            if (((float)player.PlaybackSession.Position.TotalMilliseconds / SongList[PlayerService.Instance.currentSongIndex].song.DurationMillis) > .1)
            {
                player.PlaybackSession.Position = new TimeSpan(0);
            }
            else
            {
                if (PlayerService.Instance.currentSongIndex > 0)
                {
                    CanSkipPrevious = false;

                    PlayerService.Instance.previousSongIndex = PlayerService.Instance.currentSongIndex;

                    PlayerService.Instance.currentSongIndex--;

                    player.Source = new MediaPlaybackItem(MediaSource.CreateFromUri(await GetStreamUrl(NewMain.Current.mc, SongList[PlayerService.Instance.currentSongIndex].song)));

                    player.Play();

                    CanSkipPrevious = true;
                }
            }

        }

        public async void skipNext()
        {
            if (!CanSkipNext || SongList.CurrentItemIndex == SongList.Count - 1)
                return;

            CanSkipNext = false;

            PlayerService.Instance.previousSongIndex = PlayerService.Instance.currentSongIndex;

            PlayerService.Instance.currentSongIndex++;

            player.Source = new MediaPlaybackItem(MediaSource.CreateFromUri(await GetStreamUrl(NewMain.Current.mc, SongList[PlayerService.Instance.currentSongIndex].song)));

            player.Play();
            CanSkipNext = true;
        }

        public async void Shuffle()
        {
            SongList.setCurrentTileDefault();

            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            int n = SongList.Count;
            while (n > 1)
            {
                byte[] box = new byte[1];
                do provider.GetBytes(box);
                while (!(box[0] < n * (Byte.MaxValue / n)));
                int k = (box[0] % n);
                n--;
                Track value = PlayerService.Instance.CurrentPlaylist[k];
                PlayerService.Instance.CurrentPlaylist[k] = PlayerService.Instance.CurrentPlaylist[n];
                PlayerService.Instance.CurrentPlaylist[n] = value;
            }

            SongList.Dispose();
            SongList = null;
            SongList = new TrackListViewModel(PlayerService.Instance.CurrentPlaylist, dispatcher, MessageViewModel);

            PlayerService.Instance.currentSongIndex = 0;
            PlayerService.Instance.previousSongIndex = 0;
            player.Source = new MediaPlaybackItem(MediaSource.CreateFromUri(await GetStreamUrl(NewMain.Current.mc, SongList[PlayerService.Instance.currentSongIndex].song)));
            player.Play();
        }

        public async void ItemClicked(object sender, ItemClickEventArgs e)
        {
            GridView gv = sender as GridView;
            int index = gv.Items.IndexOf(e.ClickedItem);

            if (index != PlayerService.Instance.currentSongIndex)
            {

                PlayerService.Instance.previousSongIndex = PlayerService.Instance.currentSongIndex;

                PlayerService.Instance.currentSongIndex = index;

                player.Source = new MediaPlaybackItem(MediaSource.CreateFromUri(await GetStreamUrl(NewMain.Current.mc, SongList[PlayerService.Instance.currentSongIndex].song)));

                player.Play();

                RaisePropertyChanged("CanSkipNext");
                RaisePropertyChanged("CanSkipPrevious");
            }
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
