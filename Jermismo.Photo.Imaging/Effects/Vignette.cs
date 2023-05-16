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
using System.Windows.Media.Imaging;

namespace Jermismo.Photo.Imaging.Effects
{
    public class Vignette : EffectBase
    {

        #region * Static Initialization *
 
        private static byte[] gradient = new byte[256];

        static Vignette()
        {
            Spline s = new Spline();
            s.Add(0, 0);
            s.Add(127, 175);
            s.Add(255, 255);

            for (int i = 0; i < 256; i++)
            {
                gradient[i] = s.Interpolate(i).ClampToByte();
            }
        }

        #endregion

        private enum VignetteType { Circle, Square }

        public const string Name = "Vignette";

        private VignetteType type = VignetteType.Circle;
        private float size, spread, rimDist, spreadDist, ratio;
        int cx, cy, max;
        Rect rectRim, rectSpread;
        
        public Vignette()
        {
            Parameters = new System.Collections.ObjectModel.ReadOnlyCollection<EffectParamBase>(new EffectParamBase[] {
                //new ListParam("Type", new string[] { "Circle", "Square" }, "Circle"),
                new RangeParam("Size", 0, 100, 0),
                new RangeParam("Spread", 0, 100, 20)
            });
            Passes.Add(new EffectPass() { RequireXY = true });
        }

        internal override void GetParameters()
        {
            //string stype = ((ListParam)Parameters[0]).Selected;
            size = ((RangeParam)Parameters[0]).Value / 100f;
            spread = ((RangeParam)Parameters[1]).Value / 100f;

            //switch (stype)
            //{
            //    case "Circle": type = VignetteType.Circle; break;
            //    case "Square": type = VignetteType.Square; break;
            //}
            
            CalcDistances();
        }

        internal override IPixel PixelOperation(IPixel pixelin, int x, int y)
        {
            return PixelOpCircle(pixelin, x, y);
            //switch (type)
            //{
            //    case VignetteType.Square: return PixelOpSquare(pixelin, x, y);
            //    default: return PixelOpCircle(pixelin, x, y);
            //}
        }

        private IPixel PixelOpCircle(IPixel pixelin, int x, int y)
        {

            float dist = MathHelper.Distance(cx, cy, x, y);
            byte m;

            if (dist > rimDist) m = 0;
            else if (dist < spreadDist) m = 255;
            else
            {
                m = (255 - ((dist - spreadDist) / ratio * 255)).ClampToByte();
                m = gradient[m];
            }

            ColorB pixel = (ColorB)pixelin;
            return pixel.BlendLinearBurn(new ColorB(255, m), 255);
        }

        private IPixel PixelOpSquare(IPixel pixelin, int x, int y)
        { 
            ColorB vig;
            Point p = new Point(x, y);

            if (!rectRim.Contains(p))
            {
                vig = new ColorB(255, 0);
            }
            else if (!rectSpread.Contains(p))
            {
                if (x < rectSpread.Left)
                { 
                     
                }
                vig = new ColorB(255, 127);
            }
            else
            {
                vig = new ColorB(255, 255);
            }

            ColorB pixel = (ColorB)pixelin;
            pixel *= vig;

            return pixel;
        }

        private void CalcDistances()
        {
            if (type == VignetteType.Circle)
            {
                cx = maxX / 2; // center x
                cy = maxY / 2; // center y
                max = (int)(Math.Sqrt((cx * cx) + (cy * cy))) + 20;
                rimDist = max * (1 - size); // invert size so slider reversed
                spreadDist = rimDist * (1 - spread); // invert spread so slider reversed
                ratio = rimDist - spreadDist;
            }
            else
            {
                max = (int)(Math.Min(maxX, maxY) / 2);
                rimDist = (float)Math.Floor(max * size);
                spreadDist = (float)Math.Floor(max * spread);
                
                rectRim = new Rect(rimDist, rimDist, maxX - rimDist - rimDist, maxY - rimDist - rimDist);
                rectSpread = new Rect(spreadDist, spreadDist, maxX - spreadDist - spreadDist, maxY - spreadDist - spreadDist);
            }
        }

    }
}
