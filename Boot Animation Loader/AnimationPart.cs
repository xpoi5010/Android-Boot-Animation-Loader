using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Boot_Animation_Loader
{
    public class AnimationInfo
    {
        public int Width;

        public int Height;

        public int fps;

        public AnimationPart[] parts;

        public bool HasAudio;
    }

    public struct AnimationPart
    {
        public enum AniType
        {
            p,
            c,
            f//unknown
        }

        public AniType type ;

        public int count;

        public int pause;

        public string path;

        public Color reghex;

        public Texture2D[] Textures;
    }


}
