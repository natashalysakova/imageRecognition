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
    public partial class Form1 : Form
    {
        CustomImage image;
        string fileName;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            image.Rotate(RotateFlipType.Rotate180FlipNone);
        }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            image.GreyScale();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            image = new CustomImage(pictureBox1);

        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void additivNoiseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AdditiveNoise window = new AdditiveNoise();
            if (window.ShowDialog() == DialogResult.OK)
            {
                image.AdiitiveNoise(window.GetSKO());
            }
        }

        private void toolStripButton11_Click(object sender, EventArgs e)
        {
            image.RestoreImage();
        }

        private void rotate90ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            image.Rotate(RotateFlipType.Rotate90FlipNone);
        }

        private void rotate180ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            image.Rotate(RotateFlipType.Rotate180FlipNone);
        }

        private void rotate270ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            image.Rotate(RotateFlipType.Rotate270FlipNone);
        }

        private void impulseNoiseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImpulseNoise window = new ImpulseNoise(image);
            if (window.ShowDialog() == DialogResult.OK)
            {
                image.ImpulseNoise(window.GetCount(), window.GetBrightness());
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                fileName = openFileDialog1.FileName;
                pictureBox1.Image = Bitmap.FromFile(fileName);
                image = new CustomImage(pictureBox1);
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if (fileName != String.Empty && pictureBox1.Image != null)
            {
                Save(this.fileName);
            }
            else if (fileName == String.Empty && pictureBox1.Image != null)
            {
                Save(string.Format("image_{0}.jpg", DateTime.Now.ToString()));
            }
            else
            {
                MessageBox.Show("Нечего сохранять");
            }

        }

        private void Save(string filename)
        {
            Bitmap b = new Bitmap(pictureBox1.Image);
            b.Save(fileName);
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    Save(saveFileDialog1.FileName);
                }
            }
            else
            {
                MessageBox.Show("Нечего сохранять");
            }
        }

        private void svertkaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            image.SvertkawithMask();
        }

        private void medianaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            image.Medianny();
        }

        private void bordersegmentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            image.Porogovaya();
        }

        private void vodorazdelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            image.Vodorazdel();
        }

        private void knearestNeiboursToolStripMenuItem_Click(object sender, EventArgs e)
        {
            image.KBlizghaishyh();
        }

        private void vzvMediannToolStripMenuItem_Click(object sender, EventArgs e)
        {
            image.VzveshennyyMedianny();
        }

        private void previttOperToolStripMenuItem_Click(object sender, EventArgs e)
        {
            image.Previtt();
        }

        private void kirshaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            image.Kirsha();
        }

        private void laplassToolStripMenuItem_Click(object sender, EventArgs e)
        {
            image.Laplass();
        }
    }
}
