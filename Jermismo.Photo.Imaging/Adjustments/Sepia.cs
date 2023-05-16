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
    public class Sepia : EffectBase
    {

        public const string Name = "Sepia";

        public Sepia()
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
            float r = pixel.Red;
            float g = pixel.Green;
            float b = pixel.Blue;

            float r2 = (r * 0.393f) + (g * 0.769f) + (b * 0.189f);
            float g2 = (r * 0.349f) + (g * 0.686f) + (b * 0.168f);
            float b2 = (r * 0.272f) + (g * 0.534f) + (b * 0.131f);
            return new ColorB(pixel.Alpha, r2.ClampToByte(), g2.ClampToByte(), b2.ClampToByte());
        }
    }
}
