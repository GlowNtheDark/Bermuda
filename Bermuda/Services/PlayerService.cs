using Bermuda.DataModels;
using Bermuda.ViewModels;
using GoogleMusicApi.UWP.Common;
using GoogleMusicApi.UWP.Structure;
using GoogleMusicApi.UWP.Structure.Enums;
using System;
using System.Threading.Tasks;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Bermuda.Services
{
    public class PlayerService
    {
        static PlayerService instance;

        public CoreDispatcher dispatcher;

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

        public bool isRadioMode { get; set; }

        public string radioSeed { get; set; }

        public string radioType { get; set; }

        public bool playlistRefreshed { get; set; }

        ColorListViewModel CLViewModel { get; set; }

        SolidColorBrush ThemeColor { get; set; }

        bool canSkipNext { get; set; }

        bool canSkipPrevious { get; set; }

        SystemMediaTransportControls Controls { get; set; }

        SystemMediaTransportControlsDisplayUpdater Updater { get; set; }

        public PlayerService()
        {
            Player = new MediaPlayer();
            CurrentPlaylist = new TrackList();
            CLViewModel = new ColorListViewModel();
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
            ThemeColor = CLViewModel[CLViewModel.index].Color;
        }

        public void ResetService()
        {
            Player.Pause();
            CurrentPlaylist = null;
            CLViewModel = null;
            currentSongIndex = 0;
            previousSongIndex = 0;
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
                await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    //Marshalled from a different thread. Need to update colors in the background.
                    CurrentPlaylist[currentSongIndex].tileColor = ThemeColor;
                    if(previousSongIndex != currentSongIndex)
                        CurrentPlaylist[previousSongIndex].tileColor = new SolidColorBrush(Colors.Transparent);
                });

            }
            catch(Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
        }

        private async void Player_MediaEnded(MediaPlayer sender, object args)
        {
            if(currentSongIndex != CurrentPlaylist.Count - 1)
            {
                previousSongIndex = currentSongIndex;
                currentSongIndex++;
                Player.Source = new MediaPlaybackItem(MediaSource.CreateFromUri(await GetStreamUrl(NewMain.Current.mc, CurrentPlaylist[currentSongIndex])));
                Player.Play();
            }

            else if(isRadioMode)
            {
                await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        getRadioTracks();
                    });

                playlistRefreshed = true;
            }
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
                    if (!canSkipNext || currentSongIndex == CurrentPlaylist.Count - 1)
                        break;
                    canSkipNext = false;
                    previousSongIndex = currentSongIndex;
                    currentSongIndex++;
                    Player.Source = new MediaPlaybackItem(MediaSource.CreateFromUri(await GetStreamUrl(NewMain.Current.mc, CurrentPlaylist[currentSongIndex])));
                    Player.Play();
                    canSkipNext = true;
                    break;
                case SystemMediaTransportControlsButton.Previous:
                    if (((float)Player.PlaybackSession.Position.TotalMilliseconds / CurrentPlaylist[Instance.currentSongIndex].DurationMillis) > .1)
                    {
                        Player.PlaybackSession.Position = new TimeSpan(0);
                    }
                    else
                    { 
                        if (!canSkipPrevious)
                            break;

                        if (Instance.currentSongIndex > 0)
                        {
                        canSkipPrevious = false;
                            Instance.previousSongIndex = Instance.currentSongIndex;

                            Instance.currentSongIndex--;

                            Player.Source = new MediaPlaybackItem(MediaSource.CreateFromUri(await GetStreamUrl(NewMain.Current.mc, CurrentPlaylist[Instance.currentSongIndex])));

                            Player.Play();

                            canSkipPrevious = true;
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        private async void getRadioTracks()
        {
            RadioFeed feed;

            if (radioType == "Track")
            {
                feed = await NewMain.Current.mc.GetStationFeed(ExplicitType.Explicit,
                      new StationFeedStation
                      {
                          LibraryContentOnly = false,
                          NumberOfEntries = -1,
                          RecentlyPlayed = new Track[0],
                          Seed = new StationSeed
                          {
                              SeedType = 1,
                              TrackId = radioSeed
                          }
                      }
                  );

            }

            else if(radioType == "Album")
            {
                feed = await NewMain.Current.mc.GetStationFeed(ExplicitType.Explicit,
                         new StationFeedStation
                         {
                             LibraryContentOnly = false,
                             NumberOfEntries = -1,
                             RecentlyPlayed = new Track[0],
                             Seed = new StationSeed
                             {
                                 SeedType = 4,
                                  AlbumId = radioSeed
                             }
                         }
                     );
            }

            else if (radioType == "Artist")
            {
                feed = await NewMain.Current.mc.GetStationFeed(ExplicitType.Explicit,
                         new StationFeedStation
                         {
                             LibraryContentOnly = false,
                             NumberOfEntries = -1,
                             RecentlyPlayed = new Track[0],
                             Seed = new StationSeed
                             {
                                 SeedType = 3,
                                 ArtistId = radioSeed
                             }
                         }
                     );
            }

            else if(radioType == "Genre")
            {
                feed = await NewMain.Current.mc.GetStationFeed(ExplicitType.Explicit,
                         new StationFeedStation
                         {
                             LibraryContentOnly = false,
                             NumberOfEntries = -1,
                             RecentlyPlayed = new Track[0],
                             Seed = new StationSeed
                             {
                                 SeedType = 3,
                                 ArtistId = radioSeed
                             }
                         }
                     );
            }

            else
            {
                feed = await NewMain.Current.mc.GetStationFeed(ExplicitType.Explicit,
                         new StationFeedStation
                         {
                             LibraryContentOnly = false,
                             NumberOfEntries = -1,
                             RecentlyPlayed = new Track[0],
                             Seed = new StationSeed
                             {
                                 SeedType = 7,
                                 ArtistId = radioSeed
                             }
                         }
                     );
            }

            if (feed.Data.Stations[0].Tracks != null)
            {

                CurrentPlaylist.Clear();
                previousSongIndex = 0;
                currentSongIndex = 0;

                foreach (Track track in feed.Data.Stations[0].Tracks)
                {
                    if ((track != null) && (track.Nid != radioSeed))
                        CurrentPlaylist.Add(track);
                }

                Player.Source = new MediaPlaybackItem(MediaSource.CreateFromUri(await GetStreamUrl(NewMain.Current.mc, CurrentPlaylist[0])));
                Player.Play();
            }
        }

        public void updateTheme()
        {
            CLViewModel = null;
            CLViewModel = new ColorListViewModel();
            ThemeColor = CLViewModel[CLViewModel.index].Color;
            if (CurrentPlaylist.Count > 0)
                CurrentPlaylist[currentSongIndex].tileColor = ThemeColor;
        }

        public static async Task<Uri> GetStreamUrl(MobileClient mc, Track track)
        {
            Uri data;

            data = await mc.GetStreamUrlAsync(track);

            return data;
        }
    }
}
