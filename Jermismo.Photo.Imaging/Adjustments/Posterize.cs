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
    public class Posterize : EffectBase
    {

        private enum PosterizeType { Rgb, Lightness }

        public const string Name = "Posterize";

        private byte[] posLevels;
        private PosterizeType type;

        public Posterize()
        {
            Parameters = new System.Collections.ObjectModel.ReadOnlyCollection<EffectParamBase>(new EffectParamBase[] {
                new ListParam("Type", new string[] { "RGB", "Lightness" }, "RGB"),
                new RangeParam("Range Size", 2, 32, 4)
            });
        }

        internal override void GetParameters()
        {
            switch (((ListParam)Parameters[0]).Selected)
            {
                case "Lightness": 
                    type = PosterizeType.Lightness; 
                    break;
                default:
                    type = PosterizeType.Rgb;
                    break;
            }

            GetLevels((int)((RangeParam)Parameters[1]).Value);
        }

        internal override IPixel PixelOperation(IPixel pixelin)
        {
            if (type == PosterizeType.Rgb)
            {
                ColorB pixel = (ColorB)pixelin;
                pixel.Red = posLevels[pixel.Red];
                pixel.Green = posLevels[pixel.Green];
                pixel.Blue = posLevels[pixel.Blue];
                return pixel;
            }
            else
            {
                Hsl hsl = new Hsl((ColorB)pixelin);
                int lightness = (int)(hsl.Lightness * 100);
                lightness = posLevels[lightness];
                hsl.Lightness = lightness * Hsl.SatScale;
                return hsl.ToColorB();
            }
        }

        private void GetLevels(int range)
        {
            int max = 0;
            switch (type)
            { 
                case PosterizeType.Lightness:
                    max = 101;
                    break;
                default:
                    max = 256;
                    break;
            }

            posLevels = new byte[max];
            byte[] levels = new byte[range];
            for (int i = 0; i < range; i++)
            {
                levels[i] = ((i * max) / (range - 1)).ClampToByte();
            }
            int j = 0, k = 0;
            for (int i = 0; i < max; i++)
            {
                posLevels[i] = levels[j];
                k += range;
                if (k > max - 1)
                {
                    k -= (max - 1); 
                    j++;
                }
            }
        }

    }
}
