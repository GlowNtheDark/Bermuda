using GoogleMusicApi.UWP.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Playback;

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

        public List<Track> songList { get; set; }

        public PlayerService()
        {
            Player = new MediaPlayer();
            Player.AutoPlay = true;
        }
    }
}
