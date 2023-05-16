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

namespace Jermismo.Photo.Imaging.Filters
{
    public class SummerForest : EffectBase
    {

        public const string Name = "SummerForest";

        private Adjustments.Curves curves;
        private Adjustments.ColorFilter filter;
        private int intensity;

        public SummerForest()
        {
            Parameters = new System.Collections.ObjectModel.ReadOnlyCollection<EffectParamBase>(new EffectParamBase[] {
                new RangeParam("Intensity", 0, 100, 100)
            });
        }

        internal override void GetParameters()
        {
            float intenF = ((RangeParam)Parameters[0]).Value / 100;
            intensity = (int)(255f * intenF);

            curves = new Adjustments.Curves();
            filter = new Adjustments.ColorFilter();

            BuildFilter();
            BuildCurves();
        }

        internal override IPixel PixelOperation(IPixel pixel)
        {
            ColorB orig = (ColorB)pixel;

            pixel = curves.PixelOperation(pixel);
            pixel = filter.PixelOperation(pixel);

            return orig.BlendAlpha((ColorB)pixel, intensity);
        }

        private void BuildFilter()
        {
            RangeParam color = filter.Parameters[0] as RangeParam;
            color.Value = 55;
            RangeParam density = filter.Parameters[1] as RangeParam;
            density.Value = 0;
            RangeParam highlight = filter.Parameters[2] as RangeParam;
            highlight.Value = 85;
            filter.GetParameters();
        }

        private void BuildCurves()
        {
            CurvesParam param = curves.Parameters[0] as CurvesParam;
            param.Lightness = new Point[] {
                new Point(0, 0),
                new Point(255, 255)
            };
            param.Red = new Point[] {
                new Point(0, 0),
                new Point(85, 55),
                new Point(175, 185),
                new Point(255, 255)
            };
            param.Green = new Point[] {
                new Point(0, 0),
                new Point(120, 132),
                new Point(255, 255)
            };
            param.Blue = new Point[] {
                new Point(0, 0),
                new Point(135, 115),
                new Point(255, 255)
            };
            curves.GetParameters();
        }

    }
    
}
