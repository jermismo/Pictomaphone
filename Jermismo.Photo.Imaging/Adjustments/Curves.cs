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
    public class Curves : EffectBase
    {

        public const string Name = "Curves";

        private byte[] lCurve = new byte[256];
        private byte[] rCurve = new byte[256];
        private byte[] gCurve = new byte[256];
        private byte[] bCurve = new byte[256];

        public Curves()
        {
            Parameters = new System.Collections.ObjectModel.ReadOnlyCollection<EffectParamBase>(new EffectParamBase[]{
                new CurvesParam()
            });
        }

        internal override void GetParameters()
        {
            CurvesParam param = Parameters[0] as CurvesParam;

            Spline ls = new Spline();
            Spline rs = new Spline();
            Spline gs = new Spline();
            Spline bs = new Spline(); // lol

            ls.AddRange(param.Lightness);
            rs.AddRange(param.Red);
            gs.AddRange(param.Green);
            bs.AddRange(param.Blue);

            for (int i = 0; i < 256; i++)
            {
                lCurve[i] = (ls.Interpolate((float)i) + 0.5f).ClampToByte();

                rCurve[i] = (rs.Interpolate((float)i) + 0.5f).ClampToByte();
                gCurve[i] = (gs.Interpolate((float)i) + 0.5f).ClampToByte();
                bCurve[i] = (bs.Interpolate((float)i) + 0.5f).ClampToByte();
            }
        }

        internal override IPixel PixelOperation(IPixel pixelin)
        {
            ColorB pixel = (ColorB)pixelin;
            byte r = lCurve[pixel.Red];
            byte g = lCurve[pixel.Green];
            byte b = lCurve[pixel.Blue];

            r = rCurve[r];
            g = gCurve[g];
            b = bCurve[b];

            return new ColorB(r, g, b);
        }
    }
}
