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
    public class Levels : EffectBase
    {

        public const string Name = "Levels";

        private byte[] rVals, gVals, bVals;
        internal ColorB inLow, inHigh;
        internal ColorB outLow, outHigh;
        internal float rGamma, gGamma, bGamma;

        public Levels()
        {
            Parameters = new System.Collections.ObjectModel.ReadOnlyCollection<EffectParamBase>(new EffectParamBase[] {
                new RangeParam("Highs", 0, 100, 100),
                new RangeParam("Mids", 1, 100, 50),
                new RangeParam("Lows", 0, 100, 0),
            });
        }

        internal override void GetParameters()
        {
            outHigh = new ColorB(255, 255);
            outLow = new ColorB(255, 0);

            float inh = ((RangeParam)Parameters[0]).Value / 100f;
            float inl = ((RangeParam)Parameters[2]).Value / 100f;

            inHigh = new ColorB(255, (byte)(inh * 255));
            inLow = new ColorB(255, (byte)(inl * 255));

            float tempGamma = ((RangeParam)Parameters[1]).Value / 50f;
            tempGamma = (float)Math.Pow(tempGamma, 2);

            rGamma = gGamma = bGamma = tempGamma;

            BuildLookup();
        }

        internal override IPixel PixelOperation(IPixel pixelin)
        {
            ColorB pixel = (ColorB)pixelin;
            pixel.Red = rVals[pixel.Red];
            pixel.Green = gVals[pixel.Green];
            pixel.Blue = bVals[pixel.Blue];
            return pixel;
        }

        internal void BuildLookup()
        {
            rVals = new byte[256];
            gVals = new byte[256];
            bVals = new byte[256];

            for (int i = 0; i < 256; i++)
            {
                ColorB val = CalcValue(i);
                rVals[i] = val.Red;
                gVals[i] = val.Green;
                bVals[i] = val.Blue;
            }
        }

        private ColorB CalcValue(int val)
        {
            byte r = CalcValChannel(val, outHigh.Red, outLow.Red, inHigh.Red, inLow.Red, rGamma);
            byte g = CalcValChannel(val, outHigh.Green, outLow.Green, inHigh.Green, inLow.Green, gGamma);
            byte b = CalcValChannel(val, outHigh.Blue, outLow.Blue, inHigh.Blue, inLow.Blue, bGamma);
            return new ColorB(255, r, g, b);
        }

        private byte CalcValChannel(int val, byte outHighVal, byte outLowVal, byte inHighVal, byte inLowVal, float gamma)
        {
            byte ret;
            int tval = val - inLowVal;
            if (tval < 0)
            {
                ret = 0;
            }
            else if (tval + inLowVal >= inHighVal)
            {
                ret = outHighVal;
            }
            else
            {
                int outdiff = outHighVal - outLowVal;
                int indiff = inHighVal - inLowVal;
                gamma = (float)Math.Pow((float)tval / (float)indiff, gamma);
                float nval = outLowVal + outdiff * gamma;
                ret = nval.ClampToByte();
            }
            return ret;
        }

    }
}
