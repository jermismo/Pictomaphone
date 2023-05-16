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
    public class OldNewYork : EffectBase
    {

        public const string Name = "OldNewYork";

        private Adjustments.Curves curves;
        private float hue, sat, light;

        public OldNewYork()
        {
            Parameters = new System.Collections.ObjectModel.ReadOnlyCollection<EffectParamBase>(new EffectParamBase[]{
                new RangeParam("Lightness", -50, 50, 0)
            });
            colorspace = EffectColorSpace.HSL;
        }

        internal override void GetParameters()
        {
            hue = 231f / 60f;
            sat = 4f / 100f;

            light = ((RangeParam)Parameters[0]).Value;

            curves = new Adjustments.Curves();

            CurvesParam param = curves.Parameters[0] as CurvesParam;
            param.Lightness = new Point[] { new Point(80 - light, 10), new Point(198, 205), new Point(255, 255) };
            param.Red = new Point[] { new Point(9, 0), new Point(240, 255) };
            param.Green = new Point[] { new Point(0, 0), new Point(62, 70), new Point(127, 127), new Point(232, 255) };
            param.Blue = new Point[] { new Point(0, 5), new Point(72, 78), new Point(130, 122), new Point(255, 245) };
            curves.GetParameters();
        }

        internal override IPixel PixelOperation(IPixel pixel)
        {
            Hsl hsl = (Hsl)pixel;
            hsl.Hue = hue;
            hsl.Saturation = sat;

            IPixel temp = hsl.ToColorB();
            temp = curves.PixelOperation(temp);

            return temp;
        }
    }
}
