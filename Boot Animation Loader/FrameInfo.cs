using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boot_Animation_Loader
{
    public class FrameInfo
    {
        public FrameInfo()
        {
            playcount = 0;
            pause = 0;
            texture_index = 0;
            play_end = false;
            paused = false;
            partEnd = false;
        }
        public int playcount;

        public int pause;

        public int texture_index;

        public bool play_end;

        public bool paused;

        public bool partEnd;
    }
}
