using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace KursImgRecog
{

    public class CustomImage
    {
        delegate void ImageChangeHandler();

            event ImageChangeHandler OnImageChange;

        private Image _originalImage;

        public Image OriginalImage
        {
            get { return _originalImage; }
            set { _originalImage = value; }
        }

        Image img;
        public Image Image
        {
            set { img = value;
                OnImageChange();}
            get { return img; }
        }

        PictureBox pb;


        public CustomImage(PictureBox box)
        {
            OnImageChange += CustomImage_OnImageChange;
            pb = box;
            if(box.Image == null)
            {
            OriginalImage = new Bitmap(Properties.Resources.CIMG0266);
            Image = new Bitmap(Properties.Resources.CIMG0266);

            }
            else
            {
                OriginalImage = new Bitmap(box.Image);
                Image = new Bitmap(box.Image);
            }
        }

        private void CustomImage_OnImageChange()
        {
            pb.Image = Image;
        }

        public void RestoreImage()
        {
            Image = OriginalImage;
            
        }

        public void Rotate(RotateFlipType type)
        {
            img.RotateFlip(type);
            Image = img;
        }

        internal int GetPixelsCount()
        {
            return Image.Width * Image.Height;
        }

        public void GreyScale()
        {
            unsafe
            {
                Stopwatch s = new Stopwatch();
                s.Start();
                LockBitmap bmp = new LockBitmap(new Bitmap(Image));
                bmp.LockBits();
                for (int x = 0; x < Image.Width; x++)
                {
                    for (int y = 0; y < Image.Height; y++)
                    {
                        Color c = bmp.GetPixel(x, y);
                        int g = (int)(0.222 * c.R + 0.707 * c.G + 0.071 * c.B);
                        Color newColor = Color.FromArgb(g, g, g);
                        bmp.SetPixel(x, y, newColor);

                    }
                }

                bmp.UnlockBits();
                Image = bmp.GetImage();
                s.Stop();
                Console.WriteLine(s.Elapsed);
            }
        }

      

        public void AdiitiveNoise(int SKO)
        {
            Bitmap bmp = new Bitmap(Image);

            var random = new Random();
            for (int i = 0; i < bmp.Width; i++)
                for (int j = 0; j < bmp.Height; j++)
                {
                    int rnd = random.Next(SKO * 2) - SKO;
                    int r = bmp.GetPixel(i, j).R + rnd;
                    r = (r < 0 ? 0 : r);
                    r = (r > 255 ? 255 : r);
                    int g = bmp.GetPixel(i, j).G + rnd;
                    g = (g < 0 ? 0 : g);
                    g = (g > 255 ? 255 : g);
                    int b = bmp.GetPixel(i, j).B + rnd;
                    b = (b < 0 ? 0 : b);
                    b = (b > 255 ? 255 : b);
                    bmp.SetPixel(i, j, Color.FromArgb(r, g, b));
                }
            Image = bmp;
        }

        internal void ImpulseNoise(int count, int brightness)
        {
            var r = new Random();
            var bitmap = new Bitmap(Image);

            for (int i = 0; i < count; i++)
            {
                bitmap.SetPixel(r.Next() % bitmap.Width, r.Next() % bitmap.Height, Color.FromArgb(brightness, brightness, brightness));
            }

            Image = bitmap;
        }

        private int GetMaskSumm(int[,] mask)
        {
            int summ = 0;

            for (int i = 0; i < mask.GetLength(0); i++)
            {
                for (int j = 0; j < mask.GetLength(1); j++)
                {
                    summ += mask[i, j];
                }
            }
            return summ;
        }

        internal void SvertkawithMask()
        {
            var bitmap = new Bitmap(Image);
            int[,] mask =
                {
                    {2, 4, 2},
                    {4, 8, 4},
                    {2, 4, 2}
                };
            int masksumm = GetMaskSumm(mask);

            for (int i = 1; i < bitmap.Width - 1; i++)
            {
                for (int j = 1; j < bitmap.Height - 1; j++)
                {
                    int I = 0;
                    int m = 0;
                    for (int k = i - 1; k < i + mask.GetLength(0) - 1; k++)
                    {
                        int n = 0;
                        for (int l = j - 1; l < j + mask.GetLength(1) - 1; l++)
                        {
                            I += (int)(bitmap.GetPixel(k, l).GetBrightness() * 255) * mask[m, n];
                            n++;
                        }
                        m++;
                    }


                    I /= masksumm;
                    bitmap.SetPixel(i, j, Color.FromArgb(I, I, I));
                }
            }

            Image = bitmap;
        }

        internal void Previtt()
        {
            int[,] mask1 = new int[3, 3]
                {
                    { 1, 0, -1 },
                    { 1, 0, -1 },
                    { 1, 0, -1 }
                };
            int[,] mask2 = new int[3, 3]
                {
                    { 1, 1, 1 },
                    { 0, 0, 0 },
                    { -1, -1, -1 }
                };
            Image = Gradient(mask1, mask2);
        }

        internal void Laplass()
        {
            int[,] mask1 = new int[3, 3]
                {
                    { 0, 1, 0 },
                    { 1, -4, 1 },
                    { 0, 1, 0 }
                };
            int[,] mask2 = new int[3, 3]
                {
                    { 1, 1, 1 },
                    { 1, -8, 1 },
                    { 1, 1, 1 }
                };
            Image = Gradient(mask1, mask2);
        }

        internal void Kirsha()
        {
            int[,] mask1 = new int[3, 3]
                {
                    { 5,5,5 },
                    { -3, 0, -3 },
                    { -3, -3, -3 }
                };
            int[,] mask2 = new int[3, 3]
                {
                    { 5, -3, -3 },
                    { 5, 0, -3 },
                    { 5, -3, -3 }
                };
            Image = Gradient(mask1, mask2);

        }

        private Bitmap Gradient(int[,] mask1, int[,] mask2)
        {
            Bitmap b = new Bitmap(Image);
            Bitmap bb = (Bitmap)b.Clone();
            double xgrad, ygrad;
            int value;


            for (int i = 1; i < b.Width - 1; i++)
            {
                for (int j = 1; j < b.Height - 1; j++)
                {
                    xgrad = ygrad = 0.0;
                    for (int k = 0; k < 3; k++)
                    {
                        for (int l = 0; l < 3; l++)
                        {
                            int x = mask1[k, l];
                            int y = mask2[k, l];

                            double olbr = b.GetPixel(i - 1 + k, j - 1 + l).GetBrightness();
                            // int br = (int)(olbr * 255);


                            xgrad += x * olbr;
                            ygrad += y * olbr;
                        }
                    }

                    value = (int)Math.Sqrt(xgrad * xgrad + ygrad * ygrad);
                    //double sd = Math.Sqrt(xgrad * xgrad + ygrad * ygrad);
                    if (value > 255)
                        value = 255;
                    else if (value < 0)
                        value = 0;
                    //byte sdf = (byte)(sd * 255);
                    b.SetPixel(i, j, Color.FromArgb(value, value, value));
                }
            }
            return b;
        }

        internal void VzveshennyyMedianny()
        {

            Filter f = new Filter();
            Bitmap b = new Bitmap(Image);
            f.FilterMatrix = new int[,] { { 1, 2, 1 }, { 2, 3, 2 }, { 1, 2, 1 } };
            Image = f.W_MedianFilter(b, 3);
        }


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






        internal void KBlizghaishyh()
        {
            Filter f = new Filter();
            Bitmap b = new Bitmap(Image);
            Image = f.k_nearest(b, 5);

        }

        internal void Vodorazdel()
        {
            WatershedGrayscale w = new WatershedGrayscale();
            Image = w.Apply(new Bitmap(Image));
        }

        internal void Porogovaya()
        {
            Bitmap b = new Bitmap(Image);

           
            List<int> arr = CreateHistogramm();

            List<int> arr2 = new List<int>(arr);

            int firstpeak = arr2.IndexOf(arr2.Max());
            arr2.RemoveAt(firstpeak);
            int secondPeak = arr.IndexOf(arr2.Max());

            int min = 999999;
            int porog = 0;
            if (firstpeak > secondPeak)
            {
                for (int i = secondPeak; i < firstpeak; i++)
                {
                    if (arr[i] < min)
                    {
                        min = arr[i];
                        porog = i;
                    }
                }
            }
            else
            {
                for (int i = firstpeak; i < secondPeak; i++)
                {
                    if (arr[i] < min)
                    {
                        min = arr[i];
                        porog = i;
                    }
                }
            }


            for (int i = 0; i < b.Width; i++)
            {
                for (int j = 0; j < b.Height; j++)
                {
                    double br = (int)(b.GetPixel(i, j).GetBrightness() * 255);
                    if (br > porog)
                        b.SetPixel(i, j, Color.FromArgb(128, 220, 220, 220));
                    else
                    {
                        b.SetPixel(i, j, Color.FromArgb(128, 36, 36, 36));
                    }

                }
            }

            Image = b;
        }

        private List<int> CreateHistogramm()
        {
            int summ = 0;

            var arr = new int[256];
            var bitmap = new Bitmap(Image);

            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    Color c = bitmap.GetPixel(i, j);
                    var l = (int)(c.GetBrightness() * 255);

                    summ += l;
                    arr[l]++;
                }
            }
            return new List<int>(arr);
        }

        internal void Medianny()
        {
            var bitmap = new Bitmap(Image);
            for (int i = 1; i < bitmap.Width - 1; i++)
            {
                for (int j = 1; j < bitmap.Height - 1; j++)
                {
                    var mediana = new List<int>();
                    for (int k = i - 1; k < i + 2; k++)
                    {
                        for (int l = j - 1; l < j + 2; l++)
                        {
                            mediana.Add((int)(bitmap.GetPixel(k, l).GetBrightness() * 255));
                        }
                    }
                    mediana.Sort();
                    int I = mediana[mediana.Count / 2];
                    bitmap.SetPixel(i, j, Color.FromArgb(I, I, I));
                }
            }

            Image = bitmap;
        }
    }

    public class LockBitmap
    {
        Bitmap source = null;
        IntPtr Iptr = IntPtr.Zero;
        BitmapData bitmapData = null;

        public byte[] Pixels { get; set; }
        public int Depth { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public LockBitmap(Bitmap source)
        {
            this.source = source;
        }

        /// <summary>
        /// Lock bitmap data
        /// </summary>
        public void LockBits()
        {
            try
            {
                // Get width and height of bitmap
                Width = source.Width;
                Height = source.Height;

                // get total locked pixels count
                int PixelCount = Width * Height;

                // Create rectangle to lock
                Rectangle rect = new Rectangle(0, 0, Width, Height);

                // get source bitmap pixel format size
                Depth = System.Drawing.Bitmap.GetPixelFormatSize(source.PixelFormat);

                // Check if bpp (Bits Per Pixel) is 8, 24, or 32
                if (Depth != 8 && Depth != 24 && Depth != 32)
                {
                    throw new ArgumentException("Only 8, 24 and 32 bpp images are supported.");
                }

                // Lock bitmap and return bitmap data
                bitmapData = source.LockBits(rect, ImageLockMode.ReadWrite,
                                             source.PixelFormat);

                // create byte array to copy pixel values
                int step = Depth / 8;
                Pixels = new byte[PixelCount * step];
                Iptr = bitmapData.Scan0;

                // Copy data from pointer to array
                Marshal.Copy(Iptr, Pixels, 0, Pixels.Length);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Unlock bitmap data
        /// </summary>
        public void UnlockBits()
        {
            try
            {
                // Copy data from byte array to pointer
                Marshal.Copy(Pixels, 0, Iptr, Pixels.Length);

                // Unlock bitmap data
                source.UnlockBits(bitmapData);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Get the color of the specified pixel
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Color GetPixel(int x, int y)
        {
            Color clr = Color.Empty;

            // Get color components count
            int cCount = Depth / 8;

            // Get start index of the specified pixel
            int i = ((y * Width) + x) * cCount;

            if (i > Pixels.Length - cCount)
                throw new IndexOutOfRangeException();

            if (Depth == 32) // For 32 bpp get Red, Green, Blue and Alpha
            {
                byte b = Pixels[i];
                byte g = Pixels[i + 1];
                byte r = Pixels[i + 2];
                byte a = Pixels[i + 3]; // a
                clr = Color.FromArgb(a, r, g, b);
            }
            if (Depth == 24) // For 24 bpp get Red, Green and Blue
            {
                byte b = Pixels[i];
                byte g = Pixels[i + 1];
                byte r = Pixels[i + 2];
                clr = Color.FromArgb(r, g, b);
            }
            if (Depth == 8)
            // For 8 bpp get color value (Red, Green and Blue values are the same)
            {
                byte c = Pixels[i];
                clr = Color.FromArgb(c, c, c);
            }
            return clr;
        }

        /// <summary>
        /// Set the color of the specified pixel
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="color"></param>
        public void SetPixel(int x, int y, Color color)
        {
            // Get color components count
            int cCount = Depth / 8;

            // Get start index of the specified pixel
            int i = ((y * Width) + x) * cCount;

            if (Depth == 32) // For 32 bpp set Red, Green, Blue and Alpha
            {
                Pixels[i] = color.B;
                Pixels[i + 1] = color.G;
                Pixels[i + 2] = color.R;
                Pixels[i + 3] = color.A;
            }
            if (Depth == 24) // For 24 bpp set Red, Green and Blue
            {
                Pixels[i] = color.B;
                Pixels[i + 1] = color.G;
                Pixels[i + 2] = color.R;
            }
            if (Depth == 8)
            // For 8 bpp set color value (Red, Green and Blue values are the same)
            {
                Pixels[i] = color.B;
            }
        }

        internal Image GetImage()
        {
            return source;
        }
    }
}