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

namespace Jermismo.Photo.Imaging.Adjustments
{
    public class Vibrance : EffectBase
    {
        public const string Name = "Vibrance";

        private double amount = 0;

        private double outHigh, outLow, inHigh, inLow, outdiff, indiff;
        
        public Vibrance()
        {
            Parameters = new ReadOnlyCollection<EffectParamBase>(new EffectParamBase[] {
                new RangeParam("Amount", -100, 100, 0),
            });
            this.colorspace = EffectColorSpace.LAB;
        }

        internal override void GetParameters()
        {
            amount = ((RangeParam)Parameters[0]).Value;

            if (amount > 0)
            {
                inHigh = 200 - amount;
                inLow = 0 + amount;
                outHigh = 200;
                outLow = 0;
            }
            else
            {
                inHigh = 200;
                inLow = 0;
                outHigh = 200 + amount;
                outLow = 0 - amount;
            }

            outdiff = outHigh - outLow;
            indiff = 1f / (inHigh - inLow);
        }

        internal override IPixel PixelOperation(IPixel pixelin)
        {
            CieLab lab = (CieLab)pixelin;

            double a = lab.A + 100 - inLow;
            double b = lab.B + 100 - inLow;

            if (a > 0)
            {
                if (a + inLow >= inHigh) a = outHigh;
                else a = outLow + outdiff * (a * indiff);
            }
            if (b > 0)
            {
                if (b + inLow >= inHigh) b = outHigh;
                else b = outLow + outdiff * (b * indiff);
            }

            lab.A = a - 100;
            lab.B = b - 100;

            return lab;
        }

    }
}
