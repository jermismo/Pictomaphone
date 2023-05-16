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

namespace Jermismo.Photo.Imaging.Adjustments
{
    public class ColorFilter : EffectBase
    {

        public const string Name = "ColorFilter";

        private ColorB color;
        private float preserveHighlights;

        public ColorFilter()
        {
            Parameters = new System.Collections.ObjectModel.ReadOnlyCollection<EffectParamBase>(new EffectParamBase[] {
                new RangeParam("Color", 0, 360, 180),
                new RangeParam("Density", 0, 100, 50),
                new RangeParam("Preserve Highlights", 0, 100, 50),
            });
        }

        internal override void GetParameters()
        {
            Hsl hsl = new Hsl();
            hsl.Hue = ((RangeParam)Parameters[0]).Value / 60f;
            hsl.Saturation = 1f;
            hsl.Lightness = 1.5f - (((RangeParam)Parameters[1]).Value / 200f + .5f);

            preserveHighlights = ((RangeParam)Parameters[2]).Value / 100f;

            color = hsl.ToColorB();
        }

        internal override IPixel PixelOperation(IPixel pixelin)
        {
            ColorB pixel = (ColorB)pixelin;
            ColorB temp = pixel * color;

            if (preserveHighlights > 0)
            {
                int amount = pixel.GetIntensity() - 127;
                if (amount < 0) amount = 0;
                else amount = (int)(amount * preserveHighlights);
                temp = temp.Lerp(pixel, amount);
            }

            return temp;
        }
    }
}
