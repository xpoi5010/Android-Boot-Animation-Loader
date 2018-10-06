using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Boot_Animation_Loader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            Init();
        }

        private void fPS_Click(object sender, EventArgs e)
        {
            //Debug
            //animation_window.ChangeFPS(30);
        }

        private void showsettingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Debug
            animation_window.Info.Size = new Size(720, 1280);
        }
    }
}
