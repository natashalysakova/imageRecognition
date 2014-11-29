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
            set
            {
                img = value;
                OnImageChange();
            }
            get { return img; }
        }

        PictureBox pb;


        public CustomImage(PictureBox box)
        {
            OnImageChange += CustomImage_OnImageChange;
            pb = box;

            if (box.Image == null)
            {
                Image = new Bitmap(Properties.Resources.CIMG0266);
                GreyScale();
                OriginalImage = new Bitmap(Properties.Resources.CIMG0266);
            }
            else
            {
                Image = new Bitmap(box.Image);
                GreyScale();
                OriginalImage = new Bitmap(box.Image);
            }

        }

        int black, white;

        private void CustomImage_OnImageChange()
        {
            pb.Image = Image;
        }

        public Bitmap RestoreImage()
        {
            return new Bitmap(OriginalImage);

        }

        public Bitmap Rotate(RotateFlipType type)
        {
            int width = img.Width;
            int height = img.Height;

            Bitmap tmp = new Bitmap(1, 1);
            switch (type)
            {
                case RotateFlipType.Rotate90FlipNone:
                    tmp = new Bitmap(img.Height, img.Width);

                    for (int i = 0; i < width; i++)
                    {
                        for (int j = 0; j < height; j++)
                        {
                            Color old = ((Bitmap)img).GetPixel(i, j);
                            tmp.SetPixel(height - 1 - j, i, old);
                        }
                    }

                    break;
                case RotateFlipType.Rotate270FlipNone:
                    tmp = new Bitmap(img.Height, img.Width);

                    for (int i = 0; i < width; i++)
                    {
                        for (int j = 0; j < height; j++)
                        {
                            Color old = ((Bitmap)img).GetPixel(i, j);

                            tmp.SetPixel(j, width - 1 - i, old);
                        }
                    }

                    break;
                case RotateFlipType.Rotate180FlipNone:
                    tmp = new Bitmap(img.Width, img.Height);

                    for (int i = 0; i < width; i++)
                    {
                        for (int j = 0; j < height; j++)
                        {
                            Color old = ((Bitmap)img).GetPixel(i, j);

                            tmp.SetPixel(width - 1 - i, height - 1 - j, old);
                        }
                    }

                    break;
                default:
                    tmp = new Bitmap(OriginalImage);

                    break;
            }
            return tmp;
        }

        internal int GetPixelsCount()
        {
            return Image.Width * Image.Height;
        }

        public void GreyScale()
        {
            unsafe
            {
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
            }
        }

        public Bitmap GreyScale(Image b)
        {
            unsafe
            {
                LockBitmap bmp = new LockBitmap(new Bitmap(b));
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
                return new Bitmap(bmp.GetImage());
            }
        }



        public Bitmap AdiitiveNoise(int SKO)
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
            return bmp;
        }

        internal int GetWhite()
        {
            return white;
        }

        internal int GetBlack()
        {
            return black;
        }

        internal Bitmap ImpulseNoise(int count, int brightness)
        {
            var r = new Random();
            var bitmap = new Bitmap(Image);

            for (int i = 0; i < count; i++)
            {
                bitmap.SetPixel(r.Next() % bitmap.Width, r.Next() % bitmap.Height, Color.FromArgb(brightness, brightness, brightness));
            }

            return bitmap;
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

        internal Bitmap SvertkawithMask()
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

            return bitmap;
        }

        internal Bitmap Previtt()
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
            return Gradient(mask1, mask2);
        }

        internal Bitmap Laplass()
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
            return Gradient(mask1, mask2);
        }

        internal Bitmap Kirsha()
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
            return Gradient(mask1, mask2);

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
                            int br = (int)(olbr * 255);


                            xgrad += x * br;
                            ygrad += y * br;
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

        internal Bitmap VzveshennyyMedianny()
        {

            Filter f = new Filter();
            Bitmap b = new Bitmap(Image);
            f.FilterMatrix = new int[,] { { 1, 2, 1 }, { 2, 3, 2 }, { 1, 2, 1 } };
            return f.W_MedianFilter(b, 3);
        }


        enum Direction
        {
            North, West, East, South
        }



        public Bitmap AlgorithmBeatle()
        {

            GreyScale();

            //Bitmap neweBitmap = new Bitmap(Image.Width, Image.Height);
            //Bitmap old = new Bitmap(Image);
            //int X = 0, Y = 0; // Координаты первой встречи с объектом
            //int cX, cY; // Текущие координаты маркера
            //Color B; // Значение текущего пиксела
            //Direction Direct; // Направление движения жука
            //// Идем до тех пор, пока не встретим черную область
            //for (Y = 1; Y < old.Height; Y++)
            //{
            //    for (X = 1; X < old.Width; X++)
            //    {
            //        B = old.GetPixel(X, Y);
            //        if (B != Color.White)
            //            break;
            //    }
            //    // Если встречен объект, отличающийся от цвета фона (255 - белый)
            //    // прервать поиск
            //    if (X != old.Width)
            //        break;
            //}
            //// Если не нашли ни одного черного пиксела, то выходим из процедуры
            //if ((X == old.Width) && (Y == old.Height))
            //    return null;
            //// Если все нормально, начинаем обход по алгоритму жука
            //neweBitmap.SetPixel(X, Y, Color.Black);
            //// Поворачиваем налево (новое направление - север)
            //cX = X;
            //cY = Y - 1;
            //Direct = Direction.North;
            //// Пока не придем в исходную точку, выделяем контур объекта
            //while ((cX != X) || (cY != Y))
            //{
            //    // В зависимости от текущего направления движения жука
            //    switch (Direct)
            //    {
            //        // Север
            //        case Direction.North:
            //            {
            //                B = old.GetPixel(cX, cY);
            //                // Если элемент "черный", поворачиваем снова "налево"
            //                if (B != Color.White)
            //                {
            //                    neweBitmap.SetPixel(cX, cY, Color.Black);
            //                    Direct = Direction.West;
            //                    cX--;
            //                }
            //                // Иначе поворачиваем "направо"
            //                else
            //                {
            //                    Direct = Direction.East;
            //                    cX++;
            //                }
            //            }
            //            break;
            //        // Восток
            //        case Direction.East:
            //            {
            //                B = old.GetPixel(cX, cY);
            //                // Если элемент "черный", поворачиваем снова "налево"
            //                if (B != Color.White)
            //                {
            //                    neweBitmap.SetPixel(cX, cY, Color.Black);
            //                    Direct = Direction.North;
            //                    cY--;
            //                }
            //                 // Иначе поворачиваем "направо"
            //                else
            //                {
            //                    Direct = Direction.South;
            //                    cY++;
            //                }
            //            }
            //            break;
            //        // Юг
            //        case Direction.South:
            //            {
            //                B = old.GetPixel(cX, cY);
            //                // Если элемент "черный", поворачиваем снова "налево"
            //                if (B != Color.White)
            //                {
            //                    neweBitmap.SetPixel(cX, cY, Color.Black);
            //                    Direct = Direction.East;
            //                    cX++;
            //                }
            //                    // Иначе поворачиваем "направо"
            //                else
            //                {
            //                    Direct = Direction.West;
            //                    cX--;
            //                }
            //            }
            //            break;
            //        // Запад
            //        case Direction.West:
            //            {
            //                B = old.GetPixel(cX, cY);
            //                // Если элемент "черный", поворачиваем снова "налево"
            //                if (B != Color.White)
            //                {
            //                    neweBitmap.SetPixel(cX, cY, Color.Black);
            //                    Direct = Direction.South;
            //                    cY++;
            //                }
            //                    // Иначе поворачиваем "направо"
            //                else
            //                {
            //                    Direct = Direction.North;
            //                    cY--;
            //                }
            //            }
            //            break;
            //    }
            //}
            //return neweBitmap;



            ////start pixel for bug
            //Bitmap workBmp = new Bitmap(Image);
            ////default direction
            //Direction direction = Direction.North;
            //int currentX = 0;
            //int currentY = 0;
            //for (int i = 0; i < workBmp.Height; i++)
            //{
            //    for (int j = 0; j < workBmp.Width; j++)
            //    {
            //        //founding first black pixel
            //        Color tmp = workBmp.GetPixel(j, i);
            //        if (NearBlack(tmp))
            //        {
            //            if ((j != 0) && (i != 0))
            //            {
            //                currentX = j;//end X coordinate
            //                currentY = i - 1;//end Y coordinate
            //            }

            //            direction = Direction.North;

            //            while ((currentX != j) || (currentY != i))
            //            {
            //                switch (direction)
            //                {
            //                    case Direction.North:
            //                        {
            //                            if (NearBlack(workBmp.GetPixel(currentX, currentY)))
            //                            {
            //                                direction = Direction.West;
            //                                currentX--;
            //                                workBmp.SetPixel(currentX, currentY, Color.Tomato);
            //                            }
            //                            else
            //                            {
            //                                direction = Direction.East;
            //                                currentX++;
            //                            }
            //                            break;
            //                        }
            //                    case Direction.East:
            //                        {
            //                            if (NearBlack(workBmp.GetPixel(currentX, currentY)))
            //                            {
            //                                direction = Direction.North;
            //                                currentY--;
            //                                workBmp.SetPixel(currentX, currentY, Color.Tomato);
            //                            }
            //                            else
            //                            {
            //                                direction = Direction.South;
            //                                currentY++;
            //                            }
            //                            break;
            //                        }
            //                    case Direction.South:
            //                        {
            //                            if (NearBlack(workBmp.GetPixel(currentX, currentY)))
            //                            {
            //                                direction = Direction.East;
            //                                currentX++;
            //                                workBmp.SetPixel(currentX, currentY, Color.Tomato);
            //                            }
            //                            else
            //                            {
            //                                direction = Direction.West;
            //                                currentX--;
            //                            }
            //                            break;
            //                        }
            //                    case Direction.West:
            //                        {

            //                            if (NearBlack(workBmp.GetPixel(currentX, currentY)))
            //                            {
            //                                direction = Direction.South;
            //                                currentY++;
            //                                workBmp.SetPixel(currentX, currentY, Color.Tomato);
            //                            }
            //                            else
            //                            {
            //                                direction = Direction.North;
            //                                currentY--;
            //                            }
            //                            break;
            //                        }

            //                }
            //            }

            //        }
            //    }
            //}
            //return workBmp;


            Bitmap tmp = new Bitmap(Image);

            

            for (int i = 0; i < tmp.Width; i++)
            {
                for (int j = 0; j < tmp.Height; j++)
                {
                    Console.WriteLine("{0}, {1}", i, j);

                    Color tmpColor = tmp.GetPixel(i, j);
                    if (NearBlack(tmpColor))
                    {
                        Point startPoint = new Point(i, j);
                        Direction direction = Direction.North;

                        if (i == 0)
                        {
                            direction = Direction.East;
                        }

                        int currentX = i, currentY = j;
                        if (j > 0)
                            currentY = j - 1;

                        int c = 0;
                        do
                        {
                            c++;
                            if (currentX < 0)
                            {
                                currentX = 0;
                                direction = Direction.North;
                            }
                            if (currentX >= tmp.Width)
                            {
                                currentX = tmp.Width - 1;
                                direction = Direction.South;
                            }
                            if (currentY < 0)
                            {
                                currentY = 0;
                                direction = Direction.East;
                            }
                            if (currentY >= tmp.Height)
                            {
                                currentY = tmp.Height - 1;
                                direction = Direction.West;
                            }

                            Color t = tmp.GetPixel(currentX, currentY);

                            if (NearBlack(t))
                            {
                                tmp.SetPixel(currentX, currentY, Color.Red);
                                switch (direction)
                                {
                                    case Direction.North:
                                        direction = Direction.West;
                                        currentX--;
                                        break;
                                    case Direction.West:
                                        direction = Direction.South;
                                        currentY++;
                                        break;
                                    case Direction.East:
                                        direction = Direction.North;
                                        currentX++;
                                        break;
                                    case Direction.South:
                                        direction = Direction.East;
                                        currentY--;
                                        break;
                                }

                            }
                            else
                            {
                                switch (direction)
                                {
                                    case Direction.North:
                                        direction = Direction.East;
                                        currentY--;
                                        break;
                                    case Direction.West:
                                        direction = Direction.North;
                                        currentX++;
                                        break;
                                    case Direction.East:
                                        direction = Direction.South;
                                        break;
                                    case Direction.South:
                                        direction = Direction.West;
                                        break;
                                }
                            }



                        } while (/*!NearStartPoint(currentX, currentY, startPoint)*/c<1000);

                    }
                }
            }


            return tmp;

        }

        private bool NearStartPoint(int currentX, int currentY, Point start)
        {
            int valX = Math.Abs(start.X - currentX);
            int valY = Math.Abs(start.Y - currentY);
            //Console.WriteLine("{0}", valX+valY);

            if (valX + valY == 1)
            {
                return true;
            }
            return false;

        }

        private bool NearBlack(Color tmp)
        {
            if (tmp.R < 10)
                return true;
            return false;
        }


        internal Bitmap KBlizghaishyh()
        {
            Filter f = new Filter();
            Bitmap b = new Bitmap(Image);
            return f.k_nearest(b, 5);

        }

        internal Bitmap Vodorazdel()
        {
            WatershedGrayscale w = new WatershedGrayscale();
            return w.Apply(new Bitmap(Image));
        }

        internal Bitmap Porogovaya(int porog)
        {
            Bitmap b = new Bitmap(Image);

            black = white = 0;
            List<int> arr = CreateHistogramm();

            List<int> arr2 = new List<int>(arr);

            int firstpeak = arr2.IndexOf(arr2.Max());
            arr2.RemoveAt(firstpeak);
            int secondPeak = arr2.IndexOf(arr2.Max());

            int min = 999999;


            for (int i = 0; i < b.Width; i++)
            {
                for (int j = 0; j < b.Height; j++)
                {
                    double br = (int)(b.GetPixel(i, j).GetBrightness() * 255);
                    if (br > porog)
                    {
                        b.SetPixel(i, j, Color.FromArgb(128, 220, 220, 220));
                        black++;
                    }
                    else
                    {
                        b.SetPixel(i, j, Color.FromArgb(128, 36, 36, 36));
                        white++;
                    }

                }
            }

            return b;
        }

        public List<int> CreateHistogramm()
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

        internal Bitmap Medianny()
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

            return bitmap;
        }
    }

}