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
    public class ChannelOverlay : EffectBase
    {

        public const string Name = "ChannelOverlay";

        private Func<ColorB, ColorB> GetChannel;
        private int intensity;

        public ChannelOverlay()
        { 
            Parameters = new System.Collections.ObjectModel.ReadOnlyCollection<EffectParamBase>(new EffectParamBase[] {
                new ListParam("Channel", new string[] { "Lightness", "Luma", "Red", "Green", "Blue" }, "Lightness"),
                new RangeParam("Intensity", 0, 100, 75)
            });
        }

        internal override void GetParameters()
        {
            string selected = ((ListParam)Parameters[0]).Selected;
            intensity = (int)(((RangeParam)Parameters[1]).Value / 100f * 255);

            CreateGetChannelFunc(selected);
        }

        internal override IPixel PixelOperation(IPixel pixelin)
        {
            ColorB pixel = (ColorB)pixelin;
            return pixel.BlendOverlay(GetChannel(pixel), intensity);
        }

        private void CreateGetChannelFunc(string selected)
        {
            switch (selected)
            {
                case "Lightness":
                    GetChannel = c => new ColorB(c.GetLightness());
                    break;
                case "Luma":
                    GetChannel = c => new ColorB(c.GetLuma());
                    break;
                case "Intensity":
                    GetChannel = c => new ColorB(c.GetIntensity());
                    break;
                case "Red":
                    GetChannel = c => new ColorB(c.Red);
                    break;
                case "Green":
                    GetChannel = c => new ColorB(c.Green);
                    break;
                case "Blue":
                    GetChannel = c => new ColorB(c.Blue);
                    break;
            }
        }
    }
}
