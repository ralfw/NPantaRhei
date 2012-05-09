using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;

namespace Alarm_clock
{
    class Soundplayer
    {
        public void Start_playing()
        {
            var simpleSound = new SoundPlayer(@"tada.wav");
            simpleSound.Play();
        }

        public void Stop_playing() {}
    }
}
