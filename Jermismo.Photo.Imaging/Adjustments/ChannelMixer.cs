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
    public class ChannelMixer : EffectBase
    {

        public const string Name = "ChannelMixer";

        private int redAdjust, greenAdjust, blueAdjust;

        public ChannelMixer()
        {
            Parameters = new System.Collections.ObjectModel.ReadOnlyCollection<EffectParamBase>(new EffectParamBase[]{
                new RangeParam("Red", -100, 100, 0f),
                new RangeParam("Green", -100, 100, 0f),
                new RangeParam("Blue", -100, 100, 0f)
            });
        }

        internal override void GetParameters()
        {
            // TODO: Make this use percentages
            float r = ((RangeParam)Parameters[0]).Value / 100f;
            float g = ((RangeParam)Parameters[1]).Value / 100f;
            float b = ((RangeParam)Parameters[2]).Value / 100f;

            redAdjust = (int)(r * 255);
            greenAdjust = (int)(g * 255);
            blueAdjust = (int)(b * 255);
        }

        internal override IPixel PixelOperation(IPixel pixelin)
        {
            ColorB pixel = (ColorB)pixelin;
            // get new color
            pixel.Red = (pixel.Red + redAdjust).ClampToByte();
            pixel.Green = (pixel.Green + greenAdjust).ClampToByte();
            pixel.Blue = (pixel.Blue + blueAdjust).ClampToByte();

            return pixel;
        }
    }
}
