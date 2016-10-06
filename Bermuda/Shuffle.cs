using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using GoogleMusicApi.UWP.Structure;

namespace Bermuda
{
    class Shuffle
    {
        static Random rng = new Random();

        static public void AllTheThings(List<Track> list)
        {
            RNGCryptoServiceProvider provider = new RNGCryptoServiceProvider();
            int n = list.Count;
            while (n > 1)
            {
                byte[] box = new byte[1];
                do provider.GetBytes(box);
                while (!(box[0] < n * (Byte.MaxValue / n)));
                int k = (box[0] % n);
                n--;
                Track value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
