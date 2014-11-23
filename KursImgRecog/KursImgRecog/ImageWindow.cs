using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KursImgRecog
{
    public partial class ImageWindow : Form
    {
        public ImageWindow(Bitmap bitmap)
        {
            InitializeComponent();

            pictureBox1.Image = bitmap;
        }
    }
}
