using Bermuda.DataModels;
using Bermuda.ViewModels;
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
using Windows.UI.Xaml;

namespace Bermuda.Services
{
    public class PlayerService
    {
        static PlayerService instance;

        public static PlayerService Instance
        {
            get
            {
                if (instance == null)
                    instance = new PlayerService();

                return instance;
            }
        }

        public MediaPlayer Player { get; private set; }

        public TrackList CurrentPlaylist { get; set; }

        public int currentSongIndex { get; set; }

        public int previousSongIndex { get; set; }

        SystemMediaTransportControls Controls { get; set; }

        public PlayerService()
        {
            Player = new MediaPlayer();
            CurrentPlaylist = new TrackList();
            Player.AutoPlay = false;
            Player.Volume = .5;
            Player.CommandManager.IsEnabled = false;
            Controls = Player.SystemMediaTransportControls;
            Controls.IsEnabled = true;
            Controls.IsPlayEnabled = true;
            Controls.IsPauseEnabled = true;
            Controls.IsNextEnabled = true;
            Controls.IsPreviousEnabled = true;
            Controls.ButtonPressed += SystemMediaTransportControls_ButtonPressed;
            Player.MediaEnded += Player_MediaEnded;
            Player.PlaybackSession.PlaybackStateChanged += PlaybackSession_PlaybackStateChanged;
        }

        private void PlaybackSession_PlaybackStateChanged(MediaPlaybackSession sender, object args)
        {
            switch (sender.PlaybackState)
            {
                case MediaPlaybackState.Playing:
                    Controls.PlaybackStatus = MediaPlaybackStatus.Playing;
                    break;
                case MediaPlaybackState.Paused:
                    Controls.PlaybackStatus = MediaPlaybackStatus.Paused;
                    break;
                default:
                    break;
            }
        }

        private async void Player_MediaEnded(MediaPlayer sender, object args)
        {
            previousSongIndex = currentSongIndex;
            currentSongIndex++;

            Player.Source = new MediaPlaybackItem(MediaSource.CreateFromUri(await GetStreamUrl(NewMain.Current.mc, CurrentPlaylist[currentSongIndex])));
            Player.Play();
        }

        private async void SystemMediaTransportControls_ButtonPressed(SystemMediaTransportControls sender, SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            switch (args.Button)
            {
                case SystemMediaTransportControlsButton.Play:
                    Player.Play();
                    break;
                case SystemMediaTransportControlsButton.Pause:
                    Player.Pause();
                    break;
                case SystemMediaTransportControlsButton.Next:
                    currentSongIndex++;
                    Player.Source = new MediaPlaybackItem(MediaSource.CreateFromUri(await GetStreamUrl(NewMain.Current.mc, CurrentPlaylist[currentSongIndex])));
                    Player.Play();
                    break;
                case SystemMediaTransportControlsButton.Previous:

                    break;
                default:
                    break;
            }
        }

        public static async Task<Uri> GetStreamUrl(MobileClient mc, Track track)
        {
            Uri data;

            data = await mc.GetStreamUrlAsync(track);

            return data;
        }

    }
}
