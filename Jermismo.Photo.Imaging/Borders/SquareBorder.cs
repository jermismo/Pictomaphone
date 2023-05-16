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

namespace Jermismo.Photo.Imaging.Borders
{
    public class SquareBorder : EffectBase
    {

        public const string Name = "SquareBorder";

        private ColorB color;
        private int size;

        public SquareBorder()
        {
            Parameters = new System.Collections.ObjectModel.ReadOnlyCollection<EffectParamBase>(new EffectParamBase[] {
                new RangeParam("Red", 0, 255, 255),
                new RangeParam("Green", 0, 255, 255),
                new RangeParam("Blue", 0, 255, 255),
                new RangeParam("Size", 0, 100, 10)
            });
            Passes.Add(new EffectPass() { RequireXY = true });
        }

        internal override void GetParameters()
        {
            int red = (int)((RangeParam)Parameters[0]).Value;
            int green = (int)((RangeParam)Parameters[1]).Value;
            int blue = (int)((RangeParam)Parameters[2]).Value;
            color = new ColorB(255, red, green, blue);
            size = (int)(((RangeParam)Parameters[3]).Value / 400f * maxX);
        }

        internal override IPixel PixelOperation(IPixel pixel)
        {
            throw new NotImplementedException();
        }

        internal override IPixel PixelOperation(IPixel pixel, int x, int y)
        {
            if (x < size || maxX - x < size) return color;
            if (y < size || maxY - y < size) return color;
            return pixel;
        }
    }
}
