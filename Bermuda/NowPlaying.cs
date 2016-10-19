using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoogleMusicApi.UWP.Structure;
using System.Security.Cryptography;

namespace Bermuda
{
    public class NowPlaying
    {
        public List<Track> Songs = new List<Track>();
        public bool isLoadingSong { get; set; }
        public bool isSongEnded { get; set; }
        public int currentSongIndex { get; set; }
        public int prevSongIndex { get; set; }

        static Random rng = new Random();

        public void PopulateSongs(Track[] sentTracks)
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

        public Track GetSongFromIndex(int songIndex)
        {
            prevSongIndex = currentSongIndex;
            currentSongIndex = songIndex;
            return Songs[currentSongIndex];
        }

        public Track GetNextSong()
        {
            prevSongIndex = currentSongIndex;
            currentSongIndex++;

            return Songs[currentSongIndex];
        }
        
        public Track GetPreviousSong()
        {
            prevSongIndex = currentSongIndex;
            currentSongIndex--;

            return Songs[currentSongIndex];
        }

        public Track GetCurrentSong()
        {
            return Songs[currentSongIndex];
        }

        public void ShuffleSongs()
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
