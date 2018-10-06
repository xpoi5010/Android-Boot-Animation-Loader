using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Boot_Animation_Loader
{
    public partial class Form1
    {
        [DllImport("User32.dll")]
        private static extern IntPtr SetParent(IntPtr hwc, IntPtr jwnp);

        [DllImport("User32.dll")]
        private static extern IntPtr MoveWindow(IntPtr hWnd, int x, int y, int nw, int wh, bool brepaint);

        public void Init()
        {
            animation_window = new AnimationWindow();
            
            animation_window.WindowBorder = OpenTK.WindowBorder.Hidden;
            animation_window.UpdateFrame += Animation_window_UpdateFrame;
            InitializeComponent();
            SetParent(animation_window.WindowInfo.Handle, this.animation.Handle);
            MoveWindow(animation_window.WindowInfo.Handle, 0, 0, animation.Width, animation.Height, false);
        }

        private void Animation_window_UpdateFrame(object sender, OpenTK.FrameEventArgs e)
        {
            fPS.Text = $"FPS:{Math.Round(1/e.Time,1).ToString()}";
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            MoveWindow(animation_window.WindowInfo.Handle, 0, 0, animation.Width, animation.Height, false);
        }


        private void bootfinishToolStripMenuItem_Click(object sender, EventArgs e)
        {
            animation_window.BootFinished();
        }

        private void animation_Paint(object sender, PaintEventArgs e)
        {
            MoveWindow(animation_window.WindowInfo.Handle, 0, 0, animation.Width, animation.Height, false);
            if (!animation_window.Loaded) animation_window.Run();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }


        private void fileToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Android BootanimationFile|bootanimation.zip";
            if(ofd.ShowDialog() == DialogResult.OK)animation_window.BeginAnimation(ofd.FileName);
        }

        
    }
}
