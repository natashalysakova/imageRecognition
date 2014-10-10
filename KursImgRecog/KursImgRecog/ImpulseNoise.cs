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
    public partial class ImpulseNoise : Form
    {
        public ImpulseNoise(CustomImage  img)
        {
            InitializeComponent();

            numericUpDown1.Maximum = img.GetPixelsCount();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        internal int GetBrightness()
        {
            return (int)numericUpDown2.Value;
        }
    }
}
