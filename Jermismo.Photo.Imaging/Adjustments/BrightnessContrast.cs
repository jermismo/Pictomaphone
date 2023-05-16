using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media.Imaging;

using Microsoft.Xna.Framework;

namespace Jermismo.Photo.Imaging.Adjustments
{
    public class BrightnessContrast : EffectBase
    {

        public const string Name = "BrightnessContrast";

        private const string brightnessName = "Brightness";
        private const string contrastName = "Contrast";

        private int brightness;
        //private int contrast;
        private double contrastTan;

        public BrightnessContrast()
        {
            Parameters = new ReadOnlyCollection<EffectParamBase>(new EffectParamBase[] {
                new RangeParam(brightnessName, -100, 100, 0),
                new RangeParam(contrastName, -100, 100, 0)
            });
        }

        internal override void GetParameters()
        {
            float b = ((RangeParam)Parameters[0]).Value / 100;
            brightness = (int)(b * 255);

            float c = ((RangeParam)Parameters[1]).Value / 100;
            //contrast = (int)(contrast * 255);

            contrastTan = Math.Tan((c + 1) * Math.PI / 4);
        }

        internal override IPixel PixelOperation(IPixel pixelin)
        {
            ColorB pixel = (ColorB)pixelin;
            // brightness
            if (brightness < 0)
            {
                pixel *= (255 + brightness);
            }
            else
            {
                int r = pixel.Red + (((255 - pixel.Red) * brightness) >> 8);
                int g = pixel.Green + (((255 - pixel.Green) * brightness) >> 8);
                int b = pixel.Blue + (((255 - pixel.Blue) * brightness) >> 8);
                pixel = new ColorB(pixel.Alpha, r, g, b);
            }

            // contrast
            int rc = (int)(((pixel.Red - 128) * contrastTan) + 128);
            int gc = (int)(((pixel.Green - 128) * contrastTan) + 128);
            int bc = (int)(((pixel.Blue - 128) * contrastTan) + 128);

            return new ColorB(pixel.Alpha, rc.ClampToByte(), gc.ClampToByte(), bc.ClampToByte());
        }
    }
}
