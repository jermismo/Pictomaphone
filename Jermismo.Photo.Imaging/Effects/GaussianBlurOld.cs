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
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Jermismo.Photo.Imaging.Effects
{
    public class GaussianBlurOld : EffectBase
    {

        public const string Name = "GaussianBlur";

        protected int radius;
        protected int [] kernel;

        public GaussianBlurOld()
        {
            //numPasses = 2;
            //requiresXY = true;
            Parameters = new System.Collections.ObjectModel.ReadOnlyCollection<EffectParamBase>(new EffectParamBase[] 
            {
                new RangeParam("Amount", 1, 10, 3)
            });
        }

        internal override void GetParameters()
        {
            radius = (int)((RangeParam)Parameters[0]).Value;
            CalculateKernel();
        }

        internal override IPixel PixelOperation(IPixel pixelin, int x, int y)
        {
            ColorB pixel = (ColorB)pixelin;
            if (currentPass == 0) PixelOpPassOne(ref pixel, x, y);
            else PixelOpPassTwo(ref pixel, x, y);
            return pixel;
        }

        protected virtual void PixelOpPassOne(ref ColorB pixel, int x, int y)
        {
            ColorB temp = pixel;
            temp *= kernel[0];
            int sum = kernel[0];

            for (int i = 1; i <= radius; i++)
            {
                int x1 = x + i;
                int x2 = x - i;

                if (x1 > 0 && x1 < maxX) temp += (ColorB)GetPixelXy(x1, y) * kernel[i]; sum += kernel[i];
                if (x2 > 0 && x2 < maxX) temp += (ColorB)GetPixelXy(x2, y) * kernel[i]; sum += kernel[i];
            }

            pixel = temp / sum;
        }

        protected virtual void PixelOpPassTwo(ref ColorB pixel, int x, int y)
        {
            ColorB temp = (ColorB)pixel * kernel[0];
            int sum = kernel[0];

            for (int i = 1; i <= radius; i++)
            {
                int y1 = y + i;
                int y2 = y - i;

                if (y1 > 0 && y1 < maxY) temp += (ColorB)GetTempXy(x, y1) * kernel[i]; sum += kernel[i];
                if (y2 > 0 && y2 < maxY) temp += (ColorB)GetTempXy(x, y2) * kernel[i]; sum += kernel[i];
            }

            pixel = temp / sum;
        }

        protected virtual void CalculateKernel()
        {
            // the kernel is 1/2 of Pascal's Triangle
            kernel = new int[radius + 1];

            for (int i = 0; i <= radius; i++)
            {
                kernel[radius - i] = ((i + 1) * (i + 1));
            }
        }

    }
}
