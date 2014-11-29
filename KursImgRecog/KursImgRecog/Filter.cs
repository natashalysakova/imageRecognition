using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KursImgRecog
{
    public class Filter
    {
        /// <summary>
        /// Инициализация
        /// </summary>
        public Filter()
        {
            _FilterMatrix = new int[3, 3];
        }
        /// <summary>
        /// Инициализация
        /// </summary>
        /// <param name="Width">Ширина        /// <param name="Height">Высота        
        public Filter(int Width, int Height)
        {
            _FilterMatrix = new int[Width, Height];
            this.Width = Width;
            this.Height = Height;
        }
        private int[,] _FilterMatrix = null;
        private int _Width = 3;
        private int _Height = 3;
        private int _Offset = 0;
        /// <summary>
        /// Текущая матрица
        /// </summary>
        public int[,] FilterMatrix
        {
            get { return _FilterMatrix; }
            set { _FilterMatrix = value; }
        }
        /// <summary>
        /// Ширина матрицы
        /// </summary>
        public int Width
        {
            get { return _Width; }
            set { _Width = value; }
        }
        /// <summary>
        /// Высота матрицы
        /// </summary>
        public int Height
        {
            get { return _Height; }
            set { _Height = value; }
        }
        /// <summary>
        /// Сдвиг цветовых значений
        /// </summary>
        public int Offset
        {
            get { return _Offset; }
            set { _Offset = value; }
        }
        /// <summary>
        /// Возвращает изображение с применением фильтра
        /// </summary>
        ///<param name="Input">Исходное изображение       


        static public void Quicksort(List<Color> ar)
        {
            if (ar.Count > 1) Quicksort(ar, 0, ar.Count - 1);
        }

        static private void Quicksort(List<Color> ar, int left, int right)
        {
            if (left == right) return;
            int i = left + 1;
            int j = right;
            Color pivot = ar[left];

            // Loop invariant i <= j
            while (i < j)
            {
                if (ar[i].GetBrightness() <= pivot.GetBrightness()) i++;
                else if (ar[j].GetBrightness() > pivot.GetBrightness()) j--;
                else
                { // Swap ith and jth elements
                    Color m = ar[i]; ar[i] = ar[j]; ar[j] = m;
                }
            }
            // Now i == j

            if (ar[j].GetBrightness() <= pivot.GetBrightness() /* it also means that i == right, because j was never moved */)
            {
                // Left most element is array's maximum
                Color m = ar[left]; ar[left] = ar[right]; ar[right] = m;
                Quicksort(ar, left, right - 1);
            }
            else
            {
                Quicksort(ar, left, i - 1);
                Quicksort(ar, i, right);
            }
        }

        public int Finder(List<Color> a, Color b)
        {
            for (int i = 0; i < a.Count; i++)
            {
                if (a[i].GetBrightness() == b.GetBrightness())
                    return i;
            }
            return 0;
        }

        public Bitmap k_nearest(Bitmap Image, int Size)
        {
            System.Drawing.Bitmap TempBitmap = Image;
            System.Drawing.Bitmap NewBitmap = new System.Drawing.Bitmap(TempBitmap.Width, TempBitmap.Height);
            System.Drawing.Graphics NewGraphics = System.Drawing.Graphics.FromImage(NewBitmap);
            NewGraphics.DrawImage(TempBitmap, new System.Drawing.Rectangle(0, 0, TempBitmap.Width, TempBitmap.Height), new System.Drawing.Rectangle(0, 0, TempBitmap.Width, TempBitmap.Height), System.Drawing.GraphicsUnit.Pixel);
            NewGraphics.Dispose();
            Random TempRandom = new Random();
            int ApetureMin = -(Size / 2);
            int ApetureMax = (Size / 2);
            for (int x = 1; x < NewBitmap.Width - 1; ++x)
            {
                for (int y = 1; y < NewBitmap.Height - 1; ++y)
                {
                    List<Color> clr = new List<Color>();
                    for (int x2 = -1; x2 < 2; ++x2)
                    {
                        int TempX = x + x2;
                        if (TempX >= 0 && TempX < NewBitmap.Width)
                        {
                            for (int y2 = -1; y2 < 2; ++y2)
                            {
                                int TempY = y + y2;
                                if (TempY >= 0 && TempY < NewBitmap.Height)
                                {
                                    Color TempColor = TempBitmap.GetPixel(TempX, TempY);
                                    clr.Add(TempColor);
                                }
                            }
                        }
                    }

                    Quicksort(clr);
                    int R = TempBitmap.GetPixel(x, y).R;
                    int G = TempBitmap.GetPixel(x, y).G;
                    int B = TempBitmap.GetPixel(x, y).B;

                    int rI = Finder(clr, TempBitmap.GetPixel(x, y));
                    if (rI > 1 && rI < clr.Count - 2)
                    {
                        R = (clr[rI - 2].R + clr[rI - 1].R + clr[rI + 1].R + clr[rI + 2].R) / 4;
                        G = (clr[rI - 2].G + clr[rI - 1].G + clr[rI + 1].G + clr[rI + 2].G) / 4;
                        B = (clr[rI - 2].B + clr[rI - 1].B + clr[rI + 1].B + clr[rI + 2].B) / 4;
                    }

                    if (rI == clr.Count - 1)
                    {
                        R = (clr[rI - 1].R + clr[rI - 2].R + clr[rI - 3].R + clr[rI - 4].R) / 4;
                        G = (clr[rI - 1].G + clr[rI - 2].G + clr[rI - 3].G + clr[rI - 4].G) / 4;
                        B = (clr[rI - 1].B + clr[rI - 2].B + clr[rI - 3].B + clr[rI - 4].B) / 4;
                    }

                    Color MedianPixel = Color.FromArgb(R, G, B);
                    NewBitmap.SetPixel(x, y, MedianPixel);

                }
            }
            return NewBitmap;
        }


        public Bitmap W_MedianFilter(Bitmap Image, int Size)
        {
            System.Drawing.Bitmap TempBitmap = Image;
            System.Drawing.Bitmap NewBitmap = new System.Drawing.Bitmap(TempBitmap.Width, TempBitmap.Height);
            System.Drawing.Graphics NewGraphics = System.Drawing.Graphics.FromImage(NewBitmap);
            NewGraphics.DrawImage(TempBitmap, new System.Drawing.Rectangle(0, 0, TempBitmap.Width, TempBitmap.Height), new System.Drawing.Rectangle(0, 0, TempBitmap.Width, TempBitmap.Height), System.Drawing.GraphicsUnit.Pixel);
            NewGraphics.Dispose();
            Random TempRandom = new Random();
            int ApetureMin = -(Size / 2);
            int ApetureMax = (Size / 2);
            for (int x = 0; x < NewBitmap.Width; ++x)
            {
                for (int y = 0; y < NewBitmap.Height; ++y)
                {
                    List<Color> clr = new List<Color>();
                    for (int x2 = -1; x2 < 2; ++x2)
                    {
                        int TempX = x + x2;
                        if (TempX >= 0 && TempX < NewBitmap.Width)
                        {
                            for (int y2 = -1; y2 < 2; ++y2)
                            {
                                int TempY = y + y2;
                                if (TempY >= 0 && TempY < NewBitmap.Height)
                                {
                                    Color TempColor = TempBitmap.GetPixel(TempX, TempY);
                                    for (int l = 0; l < FilterMatrix[x2 + 1, y2 + 1]; l++) clr.Add(TempColor);
                                }
                            }
                        }
                    }

                    Quicksort(clr);
                    NewBitmap.SetPixel(x, y, clr[(clr.Count - 1) / 2]);
                }
            }
            return NewBitmap;
        }




        public Bitmap ApplyFilter(Bitmap Input)
        {
            Bitmap TempBitmap = Input;
            Bitmap NewBitmap = new Bitmap(TempBitmap.Width, TempBitmap.Height);
            Graphics NewGraphics = Graphics.FromImage(NewBitmap);
            NewGraphics.DrawImage(TempBitmap, new Rectangle(0, 0, TempBitmap.Width, TempBitmap.Height), new Rectangle(0, 0, TempBitmap.Width, TempBitmap.Height), GraphicsUnit.Pixel);
            NewGraphics.Dispose();
            for (int x = 0; x < Input.Width; ++x)
            {
                for (int y = 0; y < Input.Height; ++y)
                {
                    int RValue = 0;
                    int GValue = 0;
                    int BValue = 0;
                    int Weight = 0;
                    int XCurrent = -Width / 2;
                    for (int x2 = 0; x2 < Width; ++x2)
                    {
                        if (XCurrent + x < Input.Width && XCurrent + x >= 0)
                        {
                            int YCurrent = -Height / 2;
                            for (int y2 = 0; y2 < Height; ++y2)
                            {
                                if (YCurrent + y < Input.Height && YCurrent + y >= 0)
                                {
                                    RValue += FilterMatrix[x2, y2] * TempBitmap.GetPixel(XCurrent + x, YCurrent + y).R;
                                    GValue += FilterMatrix[x2, y2] * TempBitmap.GetPixel(XCurrent + x, YCurrent + y).G;
                                    BValue += FilterMatrix[x2, y2] * TempBitmap.GetPixel(XCurrent + x, YCurrent + y).B;
                                    Weight += FilterMatrix[x2, y2];
                                }
                                ++YCurrent;
                            }
                        }
                        ++XCurrent;
                    }
                    Color MeanPixel = TempBitmap.GetPixel(x, y);
                    if (Weight == 0)
                        Weight = 1;
                    if (Weight > 0)
                    {
                        RValue = (RValue / Weight) + Offset;
                        if (RValue < 0)
                            RValue = 0;
                        else if (RValue > 255)
                            RValue = 255;
                        GValue = (GValue / Weight) + Offset;
                        if (GValue < 0)
                            GValue = 0;
                        else if (GValue > 255)
                            GValue = 255;
                        BValue = (BValue / Weight) + Offset;
                        if (BValue < 0)
                            BValue = 0;
                        else if (BValue > 255)
                            BValue = 255;
                        MeanPixel = Color.FromArgb(RValue, GValue, BValue);
                    }
                    NewBitmap.SetPixel(x, y, MeanPixel);
                }
            }
            return NewBitmap;
        }
    }
}
