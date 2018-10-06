using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Boot_Animation_Loader
{
    public class VirtualDeviceInfo
    {
        private Size size = new Size();

        public Size Size
        {
            get
            {
                return size;
            }
            set
            {
                size = value;
                if (!(ChangeEvent is null)) ChangeEvent();
            }
        }

        public delegate void change_();

        public event change_ ChangeEvent;
    }
}
