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
    public class RedEyeFix : EffectBase
    {

        public const string Name = "RedEyeFix";

        private float threshold, tp39, tp40, tp45, tp50;

        public RedEyeFix()
        {
            Parameters = new System.Collections.ObjectModel.ReadOnlyCollection<EffectParamBase>(new EffectParamBase[] {
                new RangeParam("Threshold", 0, 10, 7)
            });
        }

        internal override void GetParameters()
        {
            threshold = ((RangeParam)Parameters[0]).Value;
            tp39 = threshold + 39;
            tp40 = threshold + 40;
            tp45 = threshold + 45;
            tp50 = threshold + 50;
        }

        internal override IPixel PixelOperation(IPixel pixelin)
        {
            // precompute
            int amount = (int)threshold - 10;

            // get hsl
            Hsl hsl = (Hsl)pixelin;
            int hue = (int)(hsl.Hue * 60f) + 2;
            if (hue > 360) hue -= 360;

            int sat = (int)(hsl.Saturation * 100f);
            
            // find some values
            float blendAmount = 1;
            if (hue > 259 && hue < 270)
            {
                blendAmount = ((float)hue - 259f) / 10f;
            }
            float alpha2 = 1;
            if (hue > 259)
            {
                if (sat < tp40) alpha2 = 0;
                if (sat > tp39 && sat < tp45)
                {
                    alpha2 = (sat - tp39) / 5f;
                }
            }
            if (hue < 260)
            {
                if (sat < hue * 2 + tp40) alpha2 = 0;
                if (sat > hue * 2 + tp39 && sat < hue * 2 + tp50)
                {
                    alpha2 = (sat - (hue * 2f + tp39)) / 10f;
                }
            }

            // calc more stuff
            blendAmount *= alpha2;

            return hsl;
            //if (blendAmount > 0) pixel.BlendAlpha(new ColorB(255, pixel.Green, pixel.Green, pixel.Blue), (int)(blendAmount * 255));
            //return pixel;
        }
    }
}
