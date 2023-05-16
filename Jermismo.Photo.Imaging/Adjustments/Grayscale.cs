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
    public class Grayscale : EffectBase
    {

        private Func<ColorB, byte> getScale;

        public const string Name = "Grayscale";

        public Grayscale()
        {
            Parameters = new System.Collections.ObjectModel.ReadOnlyCollection<EffectParamBase>(new EffectParamBase[] {
                new ListParam("Type", new string[] {"Lightness", "Value", "Intensity", "Luma"}, "Lightness" )
            });
        }

        internal override void GetParameters()
        {
            string selected = ((ListParam)Parameters[0]).Selected;

            switch (selected)
            {
                case "Lightness": getScale = (c) => c.GetLightness(); break;
                case "Value": getScale = (c) => c.GetMax(); break;
                case "Intensity": getScale = (c) => c.GetIntensity(); break;
                case "Luma": getScale = (c) => c.GetLuma(); break;
            }
        }

        internal override IPixel PixelOperation(IPixel pixelin)
        {
            ColorB pixel = (ColorB)pixelin;
            byte m = getScale(pixel);
            return new ColorB(pixel.Alpha, m);
        }
    }
}
