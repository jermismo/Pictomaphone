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
    public class Colorize : EffectBase
    {

        public const string Name = "Colorize";

        private float hue, saturation, lightness;

        public Colorize()
        {
            Parameters = new System.Collections.ObjectModel.ReadOnlyCollection<EffectParamBase>(new EffectParamBase[] {
                new RangeParam("Hue", 0, 360, 180),
                new RangeParam("Saturation", -100, 100, 0),
                new RangeParam("Lightness", -100, 100, 0)
            });
            colorspace = EffectColorSpace.HSL;
        }

        internal override void GetParameters()
        {
            hue = ((RangeParam)Parameters[0]).Value / 60f;
            saturation = ((RangeParam)Parameters[1]).Value / 100f;
            lightness = ((RangeParam)Parameters[2]).Value / 100f;
        }

        internal override IPixel PixelOperation(IPixel pixel)
        {
            Hsl hsl = (Hsl)pixel;
            hsl.Hue = hue;
            hsl.Saturation += saturation;
            hsl.Lightness += lightness;

            return hsl;
        }

        
    }
}
