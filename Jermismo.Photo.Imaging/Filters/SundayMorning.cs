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
    public class SundayMorning : EffectBase
    {

        public const string Name = "SundayMorning";

        private Adjustments.Levels levels;
        private Adjustments.ColorFilter filter;
        private int intensity;

        public SundayMorning()
        {
            Parameters = new System.Collections.ObjectModel.ReadOnlyCollection<EffectParamBase>(new EffectParamBase[] {
                new RangeParam("Intensity", 0, 100, 100)
            });
        }

        internal override void GetParameters()
        {
            float intenF = ((RangeParam)Parameters[0]).Value / 100;
            intensity = (int)(255f * intenF);

            levels = new Adjustments.Levels();
            filter = new Adjustments.ColorFilter();

            BuildFilter();
            BuildLevels();
        }

        internal override IPixel PixelOperation(IPixel pixelIn)
        {
            ColorB pixel = (ColorB)pixelIn;
            ColorB orig = pixel;
            Hsl hsl = new Hsl(pixel);
            hsl.Saturation -= 0.15f;
            pixel = hsl.ToColorB();

            pixel = (ColorB)levels.PixelOperation(pixel);

            pixel = (ColorB)filter.PixelOperation(pixel);

            return orig.BlendAlpha(pixel, intensity);
        }

        private void BuildFilter()
        {
            RangeParam color = filter.Parameters[0] as RangeParam;
            color.Value = 55;
            RangeParam density = filter.Parameters[1] as RangeParam;
            density.Value = 25;
            RangeParam highlight = filter.Parameters[2] as RangeParam;
            highlight.Value = 95;
            filter.GetParameters();
        }

        private void BuildLevels()
        {
            // input
            levels.inLow = new ColorB(5, 5, 5);
            levels.inHigh = new ColorB(235, 235, 235);
            // output
            levels.outLow = new ColorB(45, 0, 0);
            levels.outHigh = new ColorB(255, 255, 255);
            //gamma
            levels.rGamma = levels.gGamma = levels.bGamma = 0.85f;

            levels.BuildLookup();
        }

    }
}
