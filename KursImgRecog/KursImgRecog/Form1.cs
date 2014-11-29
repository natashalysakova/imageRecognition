using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
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
        bool isSelected;

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
                pictureBox1.Image = image.AdiitiveNoise(window.GetSKO());
            }
        }

        private void toolStripButton11_Click(object sender, EventArgs e)
        {
            isSelected = false;
            pictureBox1.Refresh();
        }

        private void rotate90ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = image.Rotate(RotateFlipType.Rotate90FlipNone);
        }

        private void rotate180ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = image.Rotate(RotateFlipType.Rotate180FlipNone);
        }

        private void rotate270ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = image.Rotate(RotateFlipType.Rotate270FlipNone);
        }

        private void impulseNoiseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImpulseNoise window = new ImpulseNoise(image);
            if (window.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = image.ImpulseNoise(window.GetCount(), window.GetBrightness());
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                fileName = openFileDialog1.FileName;
                pictureBox1.Image = Bitmap.FromFile(fileName);
                image = new CustomImage(pictureBox1);
                isSelected = false;
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

        private void OpenWindow(Bitmap img)
        {
            ImageWindow window = new ImageWindow(img);
            window.Show();
        }

        private void svertkaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenWindow(image.SvertkawithMask());
        }

        private void medianaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenWindow(image.Medianny());
        }

        private void bordersegmentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 form = new Form2();
            if (form.ShowDialog() == DialogResult.OK)
            {
                OpenWindow(image.Porogovaya(form.GetPorog()));
                int black = image.GetBlack(), white = image.GetWhite();
                MessageBox.Show("Площадь белого = " + white + " пикселей \nПлощадь черного = " + black + " пикселей");
            }

        }

        private void vodorazdelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenWindow(image.Vodorazdel());
        }

        private void knearestNeiboursToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenWindow(image.KBlizghaishyh());
        }

        private void vzvMediannToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenWindow(image.VzveshennyyMedianny());
        }

        private void previttOperToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenWindow(image.Previtt());
        }

        private void kirshaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenWindow(image.Kirsha());
        }

        private void laplassToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenWindow(image.Laplass());
        }

        Point begin, end;

        Rectangle GetSelRectangle(Point orig, Point location)
        {
            int deltaX = location.X - orig.X, deltaY = location.Y - orig.Y;
            Size s = new Size(Math.Abs(deltaX), Math.Abs(deltaY));
            Rectangle rect = new Rectangle();
            if (deltaX >= 0 & deltaY >= 0)
                rect = new Rectangle(orig, s);
            if (deltaX < 0 & deltaY > 0)
                rect = new Rectangle(location.X, orig.Y, s.Width, s.Height);
            if (deltaX < 0 & deltaY < 0)
                rect = new Rectangle(location, s);
            if (deltaX > 0 & deltaY < 0)
                rect = new Rectangle(orig.X, location.Y, s.Width, s.Height);
            return rect;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                end = e.Location;
                pictureBox1.Refresh();
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                begin = e.Location;
                isSelected = true;
            }
        }
        Rectangle rect;
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (isSelected)
            {
                rect = GetSelRectangle(begin, end);
                e.Graphics.DrawRectangle(Pens.Red, rect);
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(128, 255, 255, 255)), rect);
            }
        }
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            new Histogramm(image.CreateHistogramm(), new Bitmap(pictureBox1.Image)).ShowDialog();
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (rect.Width > 0 && rect.Height > 0)
            {
                Bitmap b = new Bitmap(rect.Width, rect.Height);
                Bitmap source = new Bitmap(pictureBox1.Image);
                int k = rect.X, l = rect.Y;
                for (int i = 0; i < rect.Width - 1; i++)
                {
                    for (int j = 0; j < rect.Height - 1; j++)
                    {
                        Color c = source.GetPixel(i + k, j + l);
                        b.SetPixel(i, j, c);
                    }

                }

                Clipboard.SetImage(b);

            }
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void toolStripButton8_Click_1(object sender, EventArgs e)
        {
            new rasst().ShowDialog();
        }

        private void chainCodingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(pictureBox1.Image!=null)
                OpenWindow(image.AlgorithmBeatle());
        }

        private void градусовToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = image.Rotate(RotateFlipType.RotateNoneFlipNone);
        }

        private static void DrawScaledArImage(Image sourceImage, Graphics context, Rectangle sourceRect)
        {
            int width = sourceImage.Width;
            int height = sourceImage.Height;
            double ratio;
            int destWidth;
            int destHeight;
            context.InterpolationMode = InterpolationMode.HighQualityBicubic;

            if (width > height)
            {
                ratio = height / (double)width;
                destWidth = sourceRect.Width;
                destHeight = Convert.ToInt32(sourceRect.Height * ratio);
                context.DrawImage(sourceImage,
                    new Rectangle(
                        sourceRect.X,
                        sourceRect.Y + ((sourceRect.Height - destHeight) / 2),
                        destWidth, destHeight),
                    new Rectangle(0, 0, sourceImage.Width, sourceImage.Height),
                    GraphicsUnit.Pixel);
            }
            else
            {
                ratio = width / (double)height;
                destWidth = Convert.ToInt32(sourceRect.Width * ratio);
                destHeight = sourceRect.Height;
                context.DrawImage(sourceImage,
                    new Rectangle(
                        sourceRect.X + ((sourceRect.Width - destWidth) / 2),
                        sourceRect.Y,
                        destWidth, destHeight),
                    new Rectangle(0, 0, sourceImage.Width, sourceImage.Height),
                    GraphicsUnit.Pixel);
            }
        }
    }
}
