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
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

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

        bool canSkipNext { get; set; }

        bool canSkipPrevious { get; set; }

        SystemMediaTransportControls Controls { get; set; }

        SystemMediaTransportControlsDisplayUpdater Updater { get; set; }

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
            Updater = Controls.DisplayUpdater;
            Player.MediaOpened += Player_MediaOpened;
            Player.MediaEnded += Player_MediaEnded;
            Player.PlaybackSession.PlaybackStateChanged += PlaybackSession_PlaybackStateChanged;
            Player.SourceChanged += Player_SourceChanged;
        }

        private void Player_MediaOpened(MediaPlayer sender, object args)
        {
            canSkipNext = true;
            canSkipPrevious = true;

            Updater.Type = MediaPlaybackType.Music;
            Updater.MusicProperties.AlbumArtist = CurrentPlaylist[currentSongIndex].Artist;
            Updater.MusicProperties.AlbumTitle = CurrentPlaylist[currentSongIndex].Album;
            Updater.MusicProperties.Title = CurrentPlaylist[currentSongIndex].Title;

            Updater.Thumbnail = RandomAccessStreamReference.CreateFromUri(new Uri(CurrentPlaylist[currentSongIndex].AlbumArtReference[0].Url));

            Updater.Update();
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

        private async void Player_SourceChanged(MediaPlayer sender, object args)
        {
            try
            {
                //Marshalled from a different thread. Need to update colors in the background.
                CurrentPlaylist[currentSongIndex].tileColor = new SolidColorBrush(Colors.Green);
                CurrentPlaylist[previousSongIndex].tileColor = new SolidColorBrush(Colors.Transparent);
            }
            catch(Exception e)
            {

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
                    if (!canSkipNext)
                        break;
                    canSkipNext = false;
                    previousSongIndex = currentSongIndex;
                    currentSongIndex++;
                    Player.Source = new MediaPlaybackItem(MediaSource.CreateFromUri(await GetStreamUrl(NewMain.Current.mc, CurrentPlaylist[currentSongIndex])));
                    Player.Play();
                    canSkipNext = true;
                    break;
                case SystemMediaTransportControlsButton.Previous:
                    if (((float)Player.PlaybackSession.Position.TotalMilliseconds / CurrentPlaylist[PlayerService.Instance.currentSongIndex].DurationMillis) > .1)
                    {
                        Player.PlaybackSession.Position = new TimeSpan(0);
                    }
                    else
                    { 
                        if (!canSkipPrevious)
                            break;

                        if (PlayerService.Instance.currentSongIndex > 0)
                        {
                        canSkipPrevious = false;
                        PlayerService.Instance.previousSongIndex = PlayerService.Instance.currentSongIndex;

                            PlayerService.Instance.currentSongIndex--;

                            Player.Source = new MediaPlaybackItem(MediaSource.CreateFromUri(await GetStreamUrl(NewMain.Current.mc, CurrentPlaylist[PlayerService.Instance.currentSongIndex])));

                            Player.Play();

                            canSkipPrevious = true;
                        }
                    }
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
