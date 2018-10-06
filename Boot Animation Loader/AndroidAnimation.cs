using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using System.Drawing;
using System.IO;

namespace Boot_Animation_Loader
{
    public class AndroidAnimation:IDisposable
    {



        /// <summary>
        /// Initialize AndroidAnimation
        /// </summary>
        /// <param name="Path">Path of Bootanimation.zip</param>
        public AndroidAnimation(string Path, Size Platform)
        {
            PlatformSize = Platform;
            Init(Path);
        }

        public void UpdateFrame(FrameEventArgs e)
        {

            if (!stop) renderFrame();
            else DrawingBackground(Color.Black);
        }

        bool disposed = false;

        bool finished = false;

        bool inited = false;

        bool stop = false;

        bool bootfinished = false;

        public Size PlatformSize { get; set; }

        public void BootFinished()
        {
            this.bootfinished = true;
        }

        private int index = 0;

        private FrameInfo frameInfo = new FrameInfo();

        private void DrawingBackground(Color color)
        {
            //
            // Clear
            //
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.ClearColor(Color.FromArgb(color.ToArgb() ^ 0x00FFFFFF));
            //
            // bg
            //
            
            GL.Begin(PrimitiveType.Quads);
            {
                GL.Color3(color);
                GL.Vertex2(-1.0f, 1.0f);
                GL.Color3(color);
                GL.Vertex2(1.0f, 1.0f);
                GL.Color3(color);
                GL.Vertex2(1.0f, -1.0f);
                GL.Color3(color);
                GL.Vertex2(-1.0f, -1.0f);
            }
            GL.End();
            
        }

        public delegate void BootFinishedEvent_();

        public event BootFinishedEvent_ BootFinishedEvent;

        public delegate void ChangeFPSRequire_(int newFPS);

        public event ChangeFPSRequire_ ChangeFPSRequire;

        public AnimationInfo info;
        ZipStreams zs;
        private void Init(string Path)
        {
            info = new AnimationInfo();
            zs = new ZipStreams(Path);
            if (!zs.Exist("desc.txt")) throw new Exception("_notCorrectfile");
            string dest = Encoding.UTF8.GetString(zs["desc.txt"].Data);
            string[] LineSplit = System.Text.RegularExpressions.Regex.Split(dest, "\r\n");
            if(LineSplit.Count()<=1) LineSplit = System.Text.RegularExpressions.Regex.Split(dest, "\n");
            if (LineSplit.Count() <= 1) throw new Exception("_destFormat");
            string header = LineSplit[0];
            string[] header_split = System.Text.RegularExpressions.Regex.Split(header, " ");
            info.HasAudio = zs.Exist("audio.wav");
            info.Width = Convert.ToInt32(header_split[0]);
            info.Height = Convert.ToInt32(header_split[1]);
            info.fps = Convert.ToInt32(header_split[2]);
            Array.Copy(LineSplit, 1, LineSplit, 0, LineSplit.Count() - 1);
            info.parts = ProcessingAnimation(LineSplit);
            
            inited = true;
        }

        private AnimationPart[] ProcessingAnimation(string[] partArray)
        {
            List<AnimationPart> list = new List<AnimationPart>();
            foreach (string part in partArray)
            {
                if (part == "") continue;
                list.Add(ConvertPart(part));
            }
            zs.Dispose();
            zs = null;
            return list.ToArray();
        }

        private AnimationPart ConvertPart(string part)
        {
            AnimationPart ap = new AnimationPart();
            string[] split = System.Text.RegularExpressions.Regex.Split(part, " ");
            if (split.Count() < 4) throw new Exception("_destFormat");
            int s_c = split.Count();
            ap.type = (AnimationPart.AniType)Enum.Parse(typeof(AnimationPart.AniType), split[0]);
            ap.count = Convert.ToInt32(split[1]);
            ap.pause = Convert.ToInt32(split[2]);
            ap.path = split[3];
            if (s_c >= 5) ap.reghex = HEXColor(split[4]);
            //CLOCK1,CLOCK2 is unsupport now.
            ZipFile[] zff = zs.GetFiles(ap.path);
            ap.Textures = getTextures(ref zff, PlatformSize);
            zff = null;
            return ap;
        }

        private Color HEXColor(string color)
        {
            string n_color = color.Replace("#", "");
            int col = Convert.ToInt32(n_color, 16);
            return Color.FromArgb(col + (0xFF << 24));
        }

        private Texture2D[] getTextures(ref ZipFile[] zf, Size size)
        {
            ZipFile trim = Array.Find(zf, x => x.Header.Location.FileName == "trim.txt");
            bool HasTrim = !(trim is null);
            string[] split_;
            List<Texture2D> list = new List<Texture2D>();
            Trim[] trims = new Trim[] { };
            if (HasTrim)
            {
                byte[] Data = trim.Data;
                string data = Encoding.UTF8.GetString(Data);
                split_ = System.Text.RegularExpressions.Regex.Split(data, "\n");
                if(split_.Count()<=1) split_ = System.Text.RegularExpressions.Regex.Split(data, "\r\n");
                trims = ConvertToTrim(split_);
                trim.Dispose();
            }
            for (int i = 0; i < zf.Count(); i++)
            {
                ZipFile zff = zf[i];
                if (zff.Header.Location.FileName == "trim.txt") continue;
                byte[] Data = zff.Data;
                using (MemoryStream ms = new MemoryStream(Data))
                {
                    Texture2D texture = new Texture2D(ms);
                    if (HasTrim)
                    {
                        texture.Point = trims[i].point;
                        texture.Size = trims[i].size;
                    }
                    else
                    {
                        texture.Size = new Size(info.Width, info.Height);
                        texture.Point = new Point
                            ((size.Width - texture.Width) / 2,
                            (size.Height - texture.Height) / 2);
                    }
                    list.Add(texture);
                    zff.Dispose();
                }
            }
            return list.ToArray();
        }

        private struct Trim
        {
            public Point point;
            public Size size;
        }

        private Trim[] ConvertToTrim(string[] trim)
        {
            List<Trim> list = new List<Trim>();
            foreach (string trr in trim)
            {
                if (trr == "") continue;
                string[] split_ = System.Text.RegularExpressions.Regex.Split(trr, @"\+");
                string[] poin_spli = System.Text.RegularExpressions.Regex.Split(split_[0], "x");
                int w = Convert.ToInt32(poin_spli[0]);
                int h = Convert.ToInt32(poin_spli[1]);
                int x = Convert.ToInt32(split_[1]);
                int y = Convert.ToInt32(split_[2]);
                list.Add(new Trim() { size = new Size(w, h), point = new Point(x, y) });
            }
            return list.ToArray();
        }

        private void renderFrame()
        {
            //info.parts[index].Textures[frameInfo.texture_index]
            if (stop) return;
            if (disposed) return;
            Texture2D texture = info.parts[index].Textures[frameInfo.texture_index];
            int x = texture.Point.X;
            int y = texture.Point.Y;
            int h = texture.Size.Height;
            int w = texture.Size.Width;
            DrawingBackground(info.parts[index].reghex);
            GL.BindTexture(TextureTarget.Texture2D, texture.Id);
            GL.Begin(PrimitiveType.Quads);
            {
                GL.TexCoord2(0, 0);
                GL.Color3(Color.White);
                GL.Vertex2(cx(x), cy(y));//temp,testing   cx(x),cy(y)
                GL.TexCoord2(1, 0);
                GL.Color3(Color.White);
                GL.Vertex2(cx(x + w), cy(y));//cx(x+w),cy(y)
                GL.TexCoord2(1, 1);
                GL.Color3(Color.White);
                GL.Vertex2(cx(x + w), cy(y + h));//cx(x+w),cy(y+h)
                GL.TexCoord2(0, 1);
                GL.Color3(Color.White);
                GL.Vertex2(cx(x), cy(y + h));//cx(x),cy(y+h)
            }
            GL.End();
            if (frameInfo.texture_index >= info.parts[index].Textures.Count() - 1)
            {
                frameInfo.paused = true;
            }
            if (frameInfo.paused)
            {
                if (frameInfo.pause >= info.parts[index].pause)
                {
                    frameInfo.paused = false;
                    frameInfo.playcount++;
                    frameInfo.partEnd = true;
                }
                else
                {
                    frameInfo.pause++;
                }
            }
            else frameInfo.texture_index++;
            frameInfo.playcount = info.parts[index].count == 0 ? -1 : frameInfo.playcount;
            if (frameInfo.playcount >= info.parts[index].count)
            {
                frameInfo = new FrameInfo();
                index++;
            }
            else if (frameInfo.partEnd)
            {
                frameInfo.texture_index = 0;
                frameInfo.partEnd = false;
                if (info.parts[index].count == 0 && bootfinished && info.parts[index].type == AnimationPart.AniType.c)
                {
                    while(index < info.parts.Count() && info.parts[index].count == 0) index++;
                    frameInfo = new FrameInfo();
                }
            }
            if (index >= info.parts.Count())
            {
                BootFinished();
                stop = true;
                return;
            }
            if (info.parts[index].type == AnimationPart.AniType.p && bootfinished)
            {
                BootFinished();
                stop = true;
            }
        }
        private float cx(int x)
        {
            int width = PlatformSize.Width;
            float a = ((float)x / width) * 2 - 1;
            return a;
        }

        private float cy(int y)
        {
            int height = PlatformSize.Height;
            float a = 1 - (((float)y / height) * 2);
            return a;
        }

        private int[] Texture2DToIntarr(Texture2D[] textures)
        {
            return Array.ConvertAll(textures, x => x.Id);
        }

        public void Dispose()
        {
            foreach(AnimationPart ap in info.parts)
            {
                int[] a = Array.ConvertAll(ap.Textures, x => x.Id);
                GL.DeleteTextures(a.Count(), a);
            }
            info.parts = null;
            info = null;
        }
    }
}
