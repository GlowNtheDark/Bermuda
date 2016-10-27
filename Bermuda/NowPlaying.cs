using System;
using System.Collections.Generic;
using GoogleMusicApi.UWP.Structure;
using System.Security.Cryptography;
using Windows.Media;
using Windows.UI.Xaml;
using Windows.Media.Playback;

namespace Bermuda
{
    public static class NowPlaying
    {
        public static List<Track> Songs = new List<Track>();
        public static bool isLoadingSong { get; set; }
        public static bool isSongEnded { get; set; }
        public static int currentSongIndex { get; set; }
        public static int prevSongIndex { get; set; }
        public static bool startPlaying { get; set; }
        public static SystemMediaTransportControls systemMediaTransportControls;
        public static MediaPlayer player = new MediaPlayer();
        public static bool isFirstPlaySinceOpen = true;

        static Random rng = new Random();


        public static void PopulateSongs(Track[] sentTracks)
        {
            foreach (Track track in sentTracks)
            {
                try
                {
                    Songs.Add(track);
                }
                catch(Exception ex)
                {
                    System.Diagnostics.Debug.Write(ex);
                    Songs.RemoveAt(Songs.Count - 1);
                }
            }
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
    }
}
