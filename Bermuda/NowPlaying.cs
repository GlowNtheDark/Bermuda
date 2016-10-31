using System;
using System.Collections.Generic;
using GoogleMusicApi.UWP.Structure;
using System.Security.Cryptography;
using Windows.Media;
using Windows.UI.Xaml;
using Windows.Media.Playback;
using Windows.UI.Core;
using Windows.Media.Core;
using System.Threading.Tasks;
using GoogleMusicApi.UWP.Common;
using Windows.UI.Xaml.Media.Imaging;

namespace Bermuda
{
    public static class NowPlaying
    {
        public static List<Track> Songs = new List<Track>();
        public static MediaPlaybackList playlist = new MediaPlaybackList();
        public static bool isLoadingSong { get; set; }
        public static bool isSongEnded { get; set; }
        public static int currentSongIndex { get; set; }
        public static int prevSongIndex { get; set; }
        public static bool startPlaying { get; set; }
        public static SystemMediaTransportControls systemMediaTransportControls;
        public static MediaPlayer player = new MediaPlayer();
        public static bool isFirstPlaySinceOpen = true;
        public static CoreDispatcher dispatcher;
        public static BitmapImage CurrentSongImg { get; set; }

        static Random rng = new Random();

        public async static void PopulateSongs(Track[] sentTracks)
        {
            foreach (Track track in sentTracks)
            {
                try
                {
                    Songs.Add(track);
                    Uri uri = await GetStreamUrl(NewMain.Current.mc, track);
                    playlist.Items.Add(new MediaPlaybackItem ( MediaSource.CreateFromUri(uri)));
                }
                catch(Exception ex)
                {
                    System.Diagnostics.Debug.Write(ex);
                    Songs.RemoveAt(Songs.Count - 1);
                }
            }

            player.Source = playlist;
        }

        public static async Task<Uri> GetStreamUrl(MobileClient mc, Track track)
        {
            Uri data;

            data = await mc.GetStreamUrlAsync(track);

            return data;
        }

        public static Track GetSongFromIndex(int songIndex)
        {
            prevSongIndex = currentSongIndex;
            currentSongIndex = songIndex;
            return Songs[currentSongIndex];
        }

        public static Track GetNextSong()
        {
            prevSongIndex = currentSongIndex;
            currentSongIndex++;

            return Songs[currentSongIndex];
        }
        
        public static Track GetPreviousSong()
        {
            prevSongIndex = currentSongIndex;
            currentSongIndex--;

            return Songs[currentSongIndex];
        }

        public static Track GetCurrentSong()
        {
            return Songs[currentSongIndex];
        }

        public static void ShuffleSongs()
        {
            currentSongIndex = 0;

            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            int n = Songs.Count;
            while (n > 1)
            {
                byte[] box = new byte[1];
                do provider.GetBytes(box);
                while (!(box[0] < n * (Byte.MaxValue / n)));
                int k = (box[0] % n);
                n--;
                Track value = Songs[k];
                Songs[k] = Songs[n];
                Songs[n] = value;
            }
        }

        public async static void Playlist_CurrentItemChanged(MediaPlaybackList sender, CurrentMediaPlaybackItemChangedEventArgs args)
        {
            currentSongIndex = (int)playlist.CurrentItemIndex;
            
           await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
           {
               CurrentSongImg = new BitmapImage(new Uri(Songs[currentSongIndex].AlbumArtReference[0].Url));
               //NowPlayingScene.trackPlayProgressBar.Maximum = (double)track.DurationMillis / 1000;
               //trackPlayProgressBar.Value = 0;

               //if (NowPlaying.GetCurrentSong().AlbumArtReference != null)
               // albumArtImage.Source = new BitmapImage(new Uri(NowPlaying.GetCurrentSong().AlbumArtReference[0].Url));
               //else
               // albumArtImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/logo2480x1200.png", UriKind.Absolute));

               //playButton.Style = (Style)this.Resources["customPauseButton"];
           });
        }
    }
}
