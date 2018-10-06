using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK;
using System.ComponentModel;
using OpenTK.Input;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Drawing;

namespace Boot_Animation_Loader
{
    public class AnimationWindow : GameWindow
    {

        public AnimationWindow()
        {
            this.info = new VirtualDeviceInfo();
            info.Size = new Size(1080, 1920);
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.Blend);
            info.ChangeEvent += Info_ChangeEvent;
        }

        private void Info_ChangeEvent()
        {
            ChangeViewPlatform(info.Size.Width, info.Size.Height);
        }

        protected override void Dispose(bool manual)
        {
            base.Dispose(manual);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
        }

        protected override void OnDisposed(EventArgs e)
        {
            base.OnDisposed(e);
        }

        protected override void OnFileDrop(FileDropEventArgs e)
        {
            base.OnFileDrop(e);
        }

        protected override void OnFocusedChanged(EventArgs e)
        {
            base.OnFocusedChanged(e);
        }

        protected override void OnIconChanged(EventArgs e)
        {
            base.OnIconChanged(e);
        }

        protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);
        }

        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            base.OnKeyUp(e);
        }

        public bool Loaded = false;

        private double angle = 120;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Loaded = true;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            //DrawingBackground(Color.White);
            if (enabled) baseAnimation.UpdateFrame(e);
            this.SwapBuffers();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            ChangeViewPlatform(info.Size.Width, info.Size.Height);


        }
        VirtualDeviceInfo info = new VirtualDeviceInfo();
        public VirtualDeviceInfo Info
        {
            get
            {
                return info;
            }
            set
            {
                info = value;
                ChangeViewPlatform(info.Size.Width, info.Size.Height);
            }
        }

        private void ChangeViewPlatform(int w, int h)
        {
            double rate_source = ((double)w / (double)h);
            double rate_platform = (double)Width / (double)Height;
            bool scale_w = rate_platform <= rate_source;
            double scale = scale_w ? ((double)Width / w) : ((double)Height / h);
            h = (int)(h * scale);
            w = (int)(w * scale);
            Point startPoint = scale_w ? new Point(0, (Height - h) / 2) : new Point((Width - w) / 2, 0);
            GL.Viewport(startPoint, new Size(w, h));
        }


        public void ChangeFPS(double FPS)
        {
            this.Run(FPS, FPS);
        }
        AndroidAnimation baseAnimation;

        bool enabled = false;

        public void BeginAnimation(string Path)
        {
            if (!(baseAnimation is null)) baseAnimation.Dispose();
            baseAnimation = new AndroidAnimation(Path,new Size(info.Size.Width,info.Size.Height));
            enabled = true;
            baseAnimation.BootFinishedEvent += BaseAnimation_BootFinishedEvent;
            this.ChangeFPS(baseAnimation.info.fps);
        }
        public void BootFinished()
        {
            baseAnimation.BootFinished();
        }

        private void BaseAnimation_BootFinishedEvent()
        {
            
        }
    }
}
