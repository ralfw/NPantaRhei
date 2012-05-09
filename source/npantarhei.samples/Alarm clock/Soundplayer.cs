using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;

namespace Alarm_clock
{
    class Soundplayer
    {
        private SoundPlayer _player;

        public void Start_playing()
        {
            if (_player != null) return;

            _player = new SoundPlayer(@"tada.wav");
            _player.PlayLooping();
        }

        public void Stop_playing()
        {
            if (_player == null) return;

            _player.Stop();
            _player = null;
        }
    }
}
