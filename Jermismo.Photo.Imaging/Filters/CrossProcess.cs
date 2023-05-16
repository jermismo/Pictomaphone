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
    public class CrossProcess : EffectBase
    {

        public const string Name = "CrossProcess";

        private float highlight, shadow;
        private int tint, tintAmount;
        private Curves curves;
        private ColorB[] tintTable = new ColorB[256];

        public CrossProcess()
        {
            Parameters = new System.Collections.ObjectModel.ReadOnlyCollection<EffectParamBase>(new EffectParamBase[] {
                new RangeParam("Color Shift", 1, 100, 50),
                new RangeParam("Tint", 0, 100, 50),
                new RangeParam("Tint Amount", 0, 100, 50)
            });
        }

        internal override void GetParameters()
        {
            highlight = shadow = (int)((RangeParam)Parameters[0]).Value * .01f; ;
            tint = (int)((RangeParam)Parameters[1]).Value;
            tintAmount = (int)((RangeParam)Parameters[2]).Value / 2;
            BuildCurves();
            BuildTint();
        }

        internal override IPixel PixelOperation(IPixel pixelin)
        {
            ColorB pixel = (ColorB)curves.PixelOperation(pixelin);
            int intensity = pixel.GetIntensity();

            pixel = pixel.Lerp(tintTable[intensity], tintAmount);

            return pixel;
        }

        private void BuildCurves()
        {
            curves = new Curves();
            CurvesParam param = curves.Parameters[0] as CurvesParam;
            param.Lightness = new Point[] {
                new Point(0, 0), 
                new Point(255,255) };
            param.Red = new Point[] {
                new Point(0, 0),
                new Point(255, 255),
                new Point(96, 96 - shadow * 96),
                new Point(192 - highlight * 32, 192),
                new Point(255 - highlight * 64, 255) };
            param.Green = new Point[] {
                new Point(0, 0),
                new Point(64, 64),
                new Point(255, 255),
                new Point(192, 192 + highlight * 40) };
            param.Blue = new Point[] {
                new Point(0, 64 * shadow),
                new Point(255, 255 - 64 * highlight) };
            curves.GetParameters();
        }

        private void BuildTint()
        {
            int tintColor = (int)tint;
            if (tintColor >= 50)
            {
                tintColor = 67 + (tintColor - 50);
            }
            else
            {
                tintColor = (int)(67 - (50 - tintColor) * 2.4f);
                if (tintColor < 0) tintColor += 360;
            }
            Hsl hslTint = new Hsl(tintColor, 100, 50);
            ColorB temp = hslTint.ToColorB();
            for (int i = 0; i < 256; i++)
            {
                this.tintTable[i] = GetLumaAdjust(temp, i);
            }
        }

        private ColorB GetLumaAdjust(ColorB tint, int target)
        {
            Argb tinta = Argb.FromColorB(tint);
            float intensity = tinta.Intensity();
            float targetF = target * Argb.FloatScale;
            float targetDiff = targetF - intensity;

            tinta += targetDiff;

            if (targetDiff < 0)
            {
                float min = tinta.GetMin();
                if (min < 0)
                {
                    float luma = tinta.Luma();
                    float diff = luma - min;
                    if (diff > 0)
                    {
                        float div = luma / diff;
                        tinta.Red = luma + (tinta.Red - luma) * div;
                        tinta.Green = luma + (tinta.Green - luma) * div;
                        tinta.Blue = luma + (tinta.Blue - luma) * div;
                    }
                    else
                    {
                        tinta = new Argb(1, 0);
                    }
                }
            }
            else
            {
                if (targetDiff > 0)
                {
                    float max = tinta.GetMax();
                    if (max > 1)
                    {
                        float luma = tinta.Luma();
                        float div = (1f - luma) / (max - luma);
                        tinta.Red = luma + (tinta.Red - luma) * div;
                        tinta.Green = luma + (tinta.Green - luma) * div;
                        tinta.Blue = luma + (tinta.Blue - luma) * div;
                    }
                }
            }

            return Argb.ToColorB(tinta);
        }

    }
}
