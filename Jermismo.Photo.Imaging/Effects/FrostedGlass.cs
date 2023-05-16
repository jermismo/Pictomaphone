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

namespace Jermismo.Photo.Imaging.Effects
{
    public class FrostedGlass : EffectBase
    {
        public const string Name = "FrostedGlass";

        private float spread;
        private int intensity;
        private Random rand;

        public FrostedGlass()
        {
            Parameters = new System.Collections.ObjectModel.ReadOnlyCollection<EffectParamBase>(new EffectParamBase[] {
                new RangeParam("Spread", 1, 50, 8),
                new RangeParam("Intensity", 0, 100, 80)
            });
            Passes.Add(new EffectPass() { RequireXY = true });
        }

        internal override void GetParameters()
        {
            spread = ((RangeParam)Parameters[0]).Value;
            intensity = (int)(((RangeParam)Parameters[1]).Value * 2.55);
            rand = new Random();
        }

        internal override IPixel PixelOperation(IPixel pixel, int x, int y)
        {
            int nx = x + (int)((rand.NextDouble() * spread * 2) - spread);
            int ny = y + (int)((rand.NextDouble() * spread * 2) - spread);

            if (nx < 0) nx = 0;
            else if (nx >= maxX) nx = maxX - 1;

            if (ny < 0) ny = 0;
            if (ny >= maxY) ny = maxY - 1;

            ColorB temp = (ColorB)GetPixelXy(nx, ny);
            return ((ColorB)pixel).BlendAlpha(temp, intensity);
        }
    }
}
