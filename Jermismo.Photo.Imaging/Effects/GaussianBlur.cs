using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Jermismo.Photo.Imaging.Effects
{
    public class GaussianBlur : EffectBase
    {

        public const string Name = "GaussianBlur";

        protected int radius, radiusMinusOne;
        private int[] xSum, ySum;
        private int xCount, yCount;

        public GaussianBlur()
        {
            this.Parameters = new System.Collections.ObjectModel.ReadOnlyCollection<EffectParamBase>(new EffectParamBase[] {
                new RangeParam("Amount", 1, 25, 10)
            });
            Passes.Add(new EffectPass() { RequireXY = true, YCoordFirst = true, WriteToTemp = true });
            Passes.Add(new EffectPass() { RequireXY = true, YCoordFirst = false, WriteToTemp = true });
            Passes.Add(new EffectPass() { RequireXY = true, YCoordFirst = true, WriteToTemp = true });
            Passes.Add(new EffectPass() { RequireXY = true, YCoordFirst = false, WriteToTemp = true });
            Passes.Add(new EffectPass() { RequireXY = true, YCoordFirst = true, WriteToTemp = true });
            Passes.Add(new EffectPass() { RequireXY = true, YCoordFirst = false });
        }

        internal override void GetParameters()
        {
            radius = (int)((RangeParam)Parameters[0]).Value;
            radiusMinusOne = radius - 1;
            xSum = new int[3];
            ySum = new int[3];
        }

        internal override IPixel PixelOperation(IPixel pixel, int x, int y)
        {
            switch (currentPass)
            {
                case 0: pixel = HorizontalPassOrig((ColorB)pixel, x, y); break;
                case 1: pixel = VerticalPass((ColorB)pixel, x, y); break;
                case 2: pixel = HorizontalPass((ColorB)pixel, x, y); break;
                case 3: pixel = VerticalPass((ColorB)pixel, x, y); break;
                case 4: pixel = HorizontalPass((ColorB)pixel, x, y); break;
                case 5: pixel = VerticalPass((ColorB)pixel, x, y); break;
            }
            return pixel;
        }

        protected ColorB HorizontalPassOrig(ColorB pixel, int x, int y)
        {
            // get start of row
            if (x == 0)
            {
                xSum[0] = pixel.Red;
                xSum[1] = pixel.Green;
                xSum[2] = pixel.Blue;
                xCount = 1;
                for (int i = 1; i < radius; i++)
                {
                    ColorB temp = (ColorB)GetPixelXy(x + i, y);
                    xSum[0] += temp.Red;
                    xSum[1] += temp.Green;
                    xSum[2] += temp.Blue;
                    xCount++;
                }
            }

            int xPlus = x + radius;
            int xMinus = x - radiusMinusOne;

            if (xMinus >= 0)
            {
                ColorB temp = (ColorB)GetPixelXy(xMinus, y);
                xSum[0] -= temp.Red;
                xSum[1] -= temp.Green;
                xSum[2] -= temp.Blue;
                xCount--;
            }
            if (xPlus < maxX)
            {
                ColorB temp = (ColorB)GetPixelXy(xPlus, y);
                xSum[0] += temp.Red;
                xSum[1] += temp.Green;
                xSum[2] += temp.Blue;
                xCount++;
            }

            if (xCount > 0)
            {
                byte red = (xSum[0] / xCount).ClampToByte();
                byte green = (xSum[1] / xCount).ClampToByte();
                byte blue = (xSum[2] / xCount).ClampToByte();

                return new ColorB(pixel.Alpha, red, green, blue);
            }
            else
            {
                return pixel;
            }
        }

        protected ColorB HorizontalPass(ColorB pixel, int x, int y)
        {
            // get start of row
            if (x == 0)
            {
                ColorB temp = (ColorB)GetTempXy(x, y);
                xSum[0] = temp.Red;
                xSum[1] = temp.Green;
                xSum[2] = temp.Blue;
                xCount = 1;
                for (int i = 1; i < radius; i++)
                {
                    temp = (ColorB)GetTempXy(x + i, y);
                    xSum[0] += temp.Red;
                    xSum[1] += temp.Green;
                    xSum[2] += temp.Blue;
                    xCount++;
                }
            }

            int xPlus = x + radius;
            int xMinus = x - radiusMinusOne;

            if (xMinus >= 0)
            {
                ColorB temp = (ColorB)GetTempXy(xMinus, y);
                xSum[0] -= temp.Red;
                xSum[1] -= temp.Green;
                xSum[2] -= temp.Blue;
                xCount--;
            }
            if (xPlus < maxX)
            {
                ColorB temp = (ColorB)GetTempXy(xPlus, y);
                xSum[0] += temp.Red;
                xSum[1] += temp.Green;
                xSum[2] += temp.Blue;
                xCount++;
            }

            if (xCount > 0)
            {
                byte red = (xSum[0] / xCount).ClampToByte();
                byte green = (xSum[1] / xCount).ClampToByte();
                byte blue = (xSum[2] / xCount).ClampToByte();

                return new ColorB(pixel.Alpha, red, green, blue);
            }
            else
            {
                return pixel;
            }
        }

        protected ColorB VerticalPass(ColorB pixel, int x, int y)
        {
            if (y == 0)
            {
                ColorB temp = (ColorB)GetTempXy(x, y);
                ySum[0] = temp.Red;
                ySum[1] = temp.Green;
                ySum[2] = temp.Blue;
                yCount = 1;
                for (int i = 1; i < radius; i++)
                {
                    temp = (ColorB)GetTempXy(x, y + i);
                    ySum[0] += temp.Red;
                    ySum[1] += temp.Green;
                    ySum[2] += temp.Blue;
                    yCount++;
                }
            }

            int yPlus = y + radius;
            int yMinus = y - radiusMinusOne;

            if (yMinus >= 0)
            {
                ColorB temp = (ColorB)GetTempXy(x, yMinus);
                ySum[0] -= temp.Red;
                ySum[1] -= temp.Green;
                ySum[2] -= temp.Blue;
                yCount--;
            }
            if (yPlus < maxY)
            {
                ColorB temp = (ColorB)GetTempXy(x, yPlus);
                ySum[0] += temp.Red;
                ySum[1] += temp.Green;
                ySum[2] += temp.Blue;
                yCount++;
            }

            if (yCount > 0)
            {
                int red = (ySum[0] / yCount).ClampToByte();
                int green = (ySum[1] / yCount).ClampToByte();
                int blue = (ySum[2] / yCount).ClampToByte();

                return new ColorB(pixel.Alpha, red, green, blue);
            }
            else
            {
                return pixel;
            }
        }

    }
}
