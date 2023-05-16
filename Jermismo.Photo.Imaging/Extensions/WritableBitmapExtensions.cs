using System;
using System.Windows.Media.Imaging;

namespace Jermismo.Photo.Imaging
{
    public static class WritableBitmapExtensions
    {

        /// <summary>
        /// Creates a new copy of the bitmap.
        /// </summary>
        public static WriteableBitmap Clone(this WriteableBitmap image)
        {
            WriteableBitmap clone = new WriteableBitmap(image.PixelWidth, image.PixelHeight);
            for (int i = 0; i < image.Pixels.Length; i++)
            {
                clone.Pixels[i] = image.Pixels[i];
            }
            return clone;
        }

        /// <summary>
        /// Gets the pixel at the x and y coordinates.
        /// </summary>
        public static int GetPixel(this WriteableBitmap image, int x, int y)
        {
            if (y < 0 || y > image.PixelHeight) throw new ArgumentOutOfRangeException("y");
            if (x < 0 || x > image.PixelWidth) throw new ArgumentOutOfRangeException("x");

            return image.Pixels[y * image.PixelWidth + x];
        }

        /// <summary>
        /// Sets the pixel value at the x and y coordinates.
        /// </summary>
        public static void SetPixel(this WriteableBitmap image, int x, int y, int value)
        {
            if (y < 0 || y > image.PixelHeight) throw new ArgumentOutOfRangeException("y");
            if (x < 0 || x > image.PixelWidth) throw new ArgumentOutOfRangeException("x");

            image.Pixels[y * image.PixelWidth + x] = value;
        }

        /// <summary>
        /// Gets an IPixel array of ColorB instances.
        /// </summary>
        public static IPixel[] GetIPixelRgb(this WriteableBitmap image)
        {
            IPixel[] pixels = new IPixel[image.Pixels.Length];
            int[] imageP = image.Pixels;
            int len = image.Pixels.Length;
            for (int i = 0; i < len; i++)
            {
                pixels[i] = new ColorB(imageP[i]);
            }
            return pixels;
        }

        /// <summary>
        /// Gets an IPixel array of Hsl instances.
        /// </summary>
        public static IPixel[] GetIPixelHsl(this WriteableBitmap image)
        {
            IPixel[] pixels = new IPixel[image.Pixels.Length];
            int[] imageP = image.Pixels;
            int len = image.Pixels.Length;
            for (int i = 0; i < len; i++)
            {
                pixels[i] = new Hsl(imageP[i]);
            }
            return pixels;
        }

        /// <summary>
        /// Gets an IPixel array of Lab instances.
        /// </summary>
        public static IPixel[] GetIPixelLab(this WriteableBitmap image)
        {
            IPixel[] pixels = new IPixel[image.Pixels.Length];
            int[] imageP = image.Pixels;
            int len = image.Pixels.Length;
            for (int i = 0; i < len; i++)
            {
                pixels[i] = new CieLab(imageP[i]);
            }
            return pixels;
        }

        /// <summary>
        /// Rotates the image 90 degrees clockwise.
        /// </summary>
        public static WriteableBitmap Rotate90(this WriteableBitmap image)
        {
            // swap height and width
            WriteableBitmap temp = new WriteableBitmap(image.PixelHeight, image.PixelWidth);

            // get and set pixels
            for (int x = 0; x < image.PixelWidth; x++)
            {
                for (int y = 0; y < image.PixelHeight; y++)
                {
                    int ny = image.PixelHeight - (y + 1);
                    temp.SetPixel(y, x, image.GetPixel(x, ny));
                }
            }

            return temp;
        }

        /// <summary>
        /// Rotates the image 180 degress.
        /// </summary>
        public static WriteableBitmap Rotate180(this WriteableBitmap image)
        { 
            // swap height and width
            WriteableBitmap temp = new WriteableBitmap(image.PixelWidth, image.PixelHeight);

            // get and set pixels
            for (int x = 0; x < image.PixelWidth; x++)
            {
                for (int y = 0; y < image.PixelHeight; y++)
                {
                    int nx = image.PixelWidth - x - 1;
                    int ny = image.PixelHeight - y - 1;
                    temp.SetPixel(x, y, image.GetPixel(nx, ny));
                }
            }

            return temp;
        }

        /// <summary>
        /// Rotates the image 270 degrees clockwise (90 degrees CCW).
        /// </summary>
        public static WriteableBitmap Rotate270(this WriteableBitmap image)
        {
            // swap height and width
            WriteableBitmap temp = new WriteableBitmap(image.PixelHeight, image.PixelWidth);

            // get and set pixels
            for (int x = 0; x < image.PixelWidth; x++)
            {
                for (int y = 0; y < image.PixelHeight; y++)
                {
                    int nx = image.PixelWidth - (x + 1);
                    temp.SetPixel(y, x, image.GetPixel(nx, y));
                }
            }

            return temp;
        }

        /// <summary>
        /// Flips the image Horizontally.
        /// </summary>
        public static WriteableBitmap FlipHorizontal(this WriteableBitmap image)
        {
            WriteableBitmap temp = new WriteableBitmap(image.PixelWidth, image.PixelHeight);
            for (int x = 0; x < image.PixelWidth; x++)
            {
                for (int y = 0; y < image.PixelHeight; y++)
                {
                    int nx = image.PixelWidth - x - 1;
                    temp.SetPixel(x, y, image.GetPixel(nx, y));
                }
            }
            return temp;
        }

        /// <summary>
        /// Flips the image Vertically.
        /// </summary>
        public static WriteableBitmap FlipVertical(this WriteableBitmap image)
        {
            WriteableBitmap temp = new WriteableBitmap(image.PixelWidth, image.PixelHeight);
            for (int x = 0; x < image.PixelWidth; x++)
            {
                for (int y = 0; y < image.PixelHeight; y++)
                {
                    int ny = image.PixelHeight - y - 1;
                    temp.SetPixel(x, y, image.GetPixel(x, ny));
                }
            }
            return temp;
        }

        /// <summary>
        /// Crops the image to the specified rectangle.
        /// </summary>
        public static WriteableBitmap Crop(this WriteableBitmap image, System.Windows.Rect selection)
        {

            int width = (int)selection.Width;
            int height = (int)selection.Height;

            int sx = (int)selection.X;
            int sy = (int)selection.Y;

            int ex = sx + width;
            int ey = sy + height;

            WriteableBitmap temp = new WriteableBitmap(width, height);

            int index = 0;
            for (int y = sy; y < ey; y++)
            {
                for (int x = sx; x < ex; x++)
                {
                    temp.Pixels[index++] = image.GetPixel(x, y);
                }
            }
           
            return temp;
        }

        /// <summary>
        /// Resizes the image to the specified width and height.
        /// </summary>
        public static WriteableBitmap Resize(this WriteableBitmap image, int width, int height)
        { 
            WriteableBitmap temp = new WriteableBitmap(width, height);

            using (System.IO.MemoryStream mem = new System.IO.MemoryStream())
            {
                image.SaveJpeg(mem, width, height, 0, 100);
                mem.Position = 0;
                BitmapImage bitmap = new BitmapImage();
                bitmap.SetSource(mem);
                temp = new WriteableBitmap(bitmap);
            }

            return temp;
        }

    }
}
