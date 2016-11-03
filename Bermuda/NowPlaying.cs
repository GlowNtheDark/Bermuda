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

        public static void PopulateSongs(Track[] sentTracks)
        {

        }

        public static async Task<Uri> GetStreamUrl(MobileClient mc, Track track)
        {
            Uri data;

            data = await mc.GetStreamUrlAsync(track);

            return data;
        }

        public static void Playlist_CurrentItemChanged(MediaPlaybackList sender, CurrentMediaPlaybackItemChangedEventArgs args)
        {

        }
    }
}
