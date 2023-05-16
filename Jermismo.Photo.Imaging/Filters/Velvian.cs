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
using System.Collections.ObjectModel;

namespace Jermismo.Photo.Imaging.Filters
{
    public class Velvian : EffectBase
    {
        public const string Name = "Velvian";

        //private byte amount = 255;

        private double outHigh, outLow, inHigh, inLow, outdiff, indiff;

        public Velvian()
        {
            Parameters = new ReadOnlyCollection<EffectParamBase>(new EffectParamBase[] {
                //new RangeParam("Intensity", 0, 100, 100),
            });
            this.colorspace = EffectColorSpace.LAB;
        }

        internal override void GetParameters()
        {
            inHigh = 175;
            inLow = 25;
            outHigh = 200;
            outLow = 0;

            outdiff = outHigh - outLow;
            indiff = 1f / (inHigh - inLow);
        }

        internal override IPixel PixelOperation(IPixel pixelin)
        {
            CieLab lab = (CieLab)pixelin;

            //double a = lab.A + 100 - inLow;
            double b = lab.B + 100 - inLow;

            //if (a > 0)
            //{
            //    if (a + inLow >= inHigh) a = outHigh;
            //    else a = outLow + outdiff * (a * indiff);
            //}
            if (b > 0)
            {
                if (b + inLow >= inHigh) b = outHigh;
                else b = outLow + outdiff * (b * indiff);
            }

            //lab.A = a - 100;
            lab.B = b - 100;

            return lab;
        }
    }
}
