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
using Jermismo.Photo.Imaging.Effects;

namespace Jermismo.Photo.Imaging.Filters
{
    public class Lomo : EffectBase
    {

        public const string Name = "Lomo";

        private static Curves curves;
        private static Vignette vignette;
        private Vibrance vibrance = new Vibrance();
        private int intensity;

        public Lomo()
        { 
            Parameters = new System.Collections.ObjectModel.ReadOnlyCollection<EffectParamBase>(new EffectParamBase[] {
                new RangeParam("Intensity", 0, 100, 100)
            });
            Passes.Add(new EffectPass() { RequireXY = true });
        }

        internal override void GetParameters()
        {
            float intenF = ((RangeParam)Parameters[0]).Value / 100;
            intensity = (int)(255f * intenF);

            curves = new Curves();
            CurvesParam param = curves.Parameters[0] as CurvesParam;
            param.Lightness = new Point[] { new Point(0, 0), new Point(75, 55), new Point(180, 200), new Point(255, 255) };
            param.Red = new Point[] { new Point(0, 0), new Point(255, 255) };
            param.Green = new Point[] { new Point(0, 0), new Point(255, 255) };
            param.Blue = new Point[] { new Point(0, 0), new Point(255, 255) };
            curves.GetParameters();

            vignette = new Vignette();
            vignette.SetSize(maxX, maxY);
            RangeParam size = vignette.Parameters[0] as RangeParam;
            size.Value = 5;
            RangeParam spread = vignette.Parameters[1] as RangeParam;
            spread.Value = 35;
            vignette.GetParameters();

            ((RangeParam)vibrance.Parameters[0]).Value = 15;
            vibrance.GetParameters();
        }

        internal override IPixel PixelOperation(IPixel pixelin, int x, int y)
        {
            ColorB orig = (ColorB)pixelin;
            CieLab lab = CieLab.FromColorB(orig);
            lab = (CieLab)vibrance.PixelOperation(lab);
            
            ColorB pixel = lab.ToColorB();
            pixel = (ColorB)curves.PixelOperation(pixel);
            pixel = pixel.BlendAlpha((ColorB)vignette.PixelOperation(pixel, x, y), 200);

            return orig.BlendAlpha(pixel, intensity);
        }

    }
}
