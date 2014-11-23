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
    public partial class rasst : Form
    {
        public rasst()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int x1 = Int32.Parse(textBox1.Text), x2 = Int32.Parse(textBox3.Text), y1= Int32.Parse(textBox2.Text), y2 =Int32.Parse(textBox4.Text);
            double rasst = Math.Sqrt(Math.Pow(x1-x2, 2) + Math.Pow(y1-y2,2));
            MessageBox.Show("Расстояние между двумя точками " + rasst + " пикселей");
        }
    }
}
