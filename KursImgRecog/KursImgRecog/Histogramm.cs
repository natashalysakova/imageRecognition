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
    public partial class Histogramm : Form
    {
        public Histogramm(List<int> brightness, Bitmap image)
        {
            InitializeComponent();

            chart1.Series[0].Points.Clear();
            int summ = 0;

            var arr = new int[256];
            var bitmap = new Bitmap(image);

            for (int i = 0; i < image.Width; i++)
            {
                for (int j = 0; j < image.Height; j++)
                {
                    Color c = bitmap.GetPixel(i, j);
                    //int l = (c.R + c.G + c.B)/3;
                    var l = (int)(c.GetBrightness() * 255);

                    summ += l;
                    arr[l]++;
                }
            }
            for (int i = 0; i < 256; i++)
            {
                chart1.Series[0].Points.AddXY(i, arr[i]);
            }


            //mathwait
            double math = 1.0 / (image.Width * image.Height) * summ;
            label3.Text = Math.Round(math).ToString();

            //disp  
            int s = arr.Sum();
            int deltx = s / arr.Length;
            double sum = 0.0;
            for (int i = 0; i < arr.Length; i++)
            {
                sum += Math.Pow(arr[i] - deltx, 2);
            }
            sum /= arr.Length;
            double dispers = 1.0 / arr.Length * sum;
            label8.Text = Math.Round(dispers, 3).ToString();

            //SKO
            double sko = Math.Sqrt(dispers);
            label5.Text = Math.Round(sko).ToString();

            int moda = Array.IndexOf(arr, arr.Max());
            label7.Text = @"[" + moda + @"] - " + arr.Max();
        }

        private void Histogramm_Load(object sender, EventArgs e)
        {

        }
    }
}
