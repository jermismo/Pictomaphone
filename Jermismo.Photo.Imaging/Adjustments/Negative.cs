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
    public class Negative : EffectBase
    {

        public const string Name = "Negative";

        public Negative()
        {
            Parameters = new System.Collections.ObjectModel.ReadOnlyCollection<EffectParamBase>(new EffectParamBase[0]);
        }

        internal override void GetParameters()
        {
            // none
        }

        internal override IPixel PixelOperation(IPixel pixelin)
        {
            ColorB pixel = (ColorB)pixelin;
            pixel.Red = (byte)(255 - pixel.Red);
            pixel.Green = (byte)(255 - pixel.Green);
            pixel.Blue = (byte)(255 - pixel.Blue);
            return pixel;
        }
    }
}
