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
    public class ColorBalance : EffectBase
    {

        public const string Name = "ColorBalance";

        int cyanRed, magentaGreen, yellowBlue;
        int cyanRed2, magentaGreen2, yellowBlue2;

        public ColorBalance()
        {
            Parameters = new System.Collections.ObjectModel.ReadOnlyCollection<EffectParamBase>(new EffectParamBase[] {
                new RangeParam("Cyan / Red", -100, 100, 0),
                new RangeParam("Magenta / Green", -100, 100, 0),
                new RangeParam("Yellow / Blue", -100, 100, 0),
            });
        }

        internal override void GetParameters()
        {
            cyanRed = (int)((RangeParam)Parameters[0]).Value;
            magentaGreen = (int)((RangeParam)Parameters[1]).Value;
            yellowBlue = (int)((RangeParam)Parameters[2]).Value;

            cyanRed2 = cyanRed / 2;
            magentaGreen2 = magentaGreen / 2;
            yellowBlue2 = yellowBlue / 2;
        }

        internal override IPixel PixelOperation(IPixel pixel)
        {
            ColorB color = (ColorB)pixel;

            color.Red = (color.Red + cyanRed - magentaGreen2 - yellowBlue2).ClampToByte();
            color.Green = (color.Green - cyanRed2 + magentaGreen - yellowBlue2).ClampToByte();
            color.Blue = (color.Blue - cyanRed2 - magentaGreen2 + yellowBlue).ClampToByte();
            
            return color;
        }

    }
}
