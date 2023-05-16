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
    public class Solarize : EffectBase
    {

        public const string Name = "Solarize";

        float threshold;

        public Solarize()
        {
            Parameters = new System.Collections.ObjectModel.ReadOnlyCollection<EffectParamBase>(new EffectParamBase[] {
                new RangeParam("Threshold", 0, 255, 127)
            });
        }

        internal override void GetParameters()
        {
            threshold = (byte)((RangeParam)Parameters[0]).Value;
        }

        internal override IPixel PixelOperation(IPixel pixelin)
        {
            ColorB pixel = (ColorB)pixelin;
            if (pixel.GetLuma() > threshold)
            {
                pixel.Red = (255 - pixel.Red).ClampToByte();
                pixel.Green = (255 - pixel.Green).ClampToByte();
                pixel.Blue = (255 - pixel.Blue).ClampToByte();
            }
            return pixel;
        }
    }
}
