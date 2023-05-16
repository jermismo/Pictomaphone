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

using Jermismo.Photo.Imaging.Adjustments;

namespace Jermismo.Photo.Imaging.Filters
{
    public class Brawny : EffectBase
    {

        public const string Name = "Brawny";

        private static Curves curves;
        private static ColorFilter filter;
        private int intensity;

        public Brawny()
        { 
            Parameters = new System.Collections.ObjectModel.ReadOnlyCollection<EffectParamBase>(new EffectParamBase[] {
                new RangeParam("Intensity", 0, 100, 100)
            });
        }

        internal override void GetParameters()
        {
            float intenF = ((RangeParam)Parameters[0]).Value / 100;
            intensity = (int)(255f * intenF);

            curves = new Curves();
            CurvesParam param = curves.Parameters[0] as CurvesParam;
            param.Lightness = new Point[] { new Point(0, 0), new Point(255, 255) };
            param.Red = new Point[] { new Point(0, 51), new Point(207, 204), new Point(240, 255) };
            param.Green = new Point[] { new Point(0, 0), new Point(128, 125), new Point(187, 205), new Point(255, 255) };
            param.Blue = new Point[] { new Point(0, 33), new Point(194, 202), new Point(255, 255) };
            curves.GetParameters();

            filter = new ColorFilter();
            ((RangeParam)filter.Parameters[0]).Value = 35;
            ((RangeParam)filter.Parameters[1]).Value = 35;
            ((RangeParam)filter.Parameters[2]).Value = 65;
            filter.GetParameters();
        }

        internal override IPixel PixelOperation(IPixel pixelin)
        {
            ColorB pixel = (ColorB)pixelin;
            ColorB orig = pixel;
            
            pixel = (ColorB)curves.PixelOperation(pixel);

            ColorB overlay = new ColorB(1, pixel.GetLuma());
            pixel = pixel.BlendOverlay(overlay, 127);

            pixel = (ColorB)filter.PixelOperation(pixel);

            return orig.BlendAlpha(pixel, intensity);
        }

    }
}
