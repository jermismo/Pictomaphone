/**************************
 * Stack Blur Algorithm by Mario Klingemann <mario@quasimondo.com>
 * ************************/

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
    public class StackBlur : EffectBase
    {

        public const string Name = "Blur";

        protected int radius;

        public StackBlur()
        {
            Parameters = new System.Collections.ObjectModel.ReadOnlyCollection<EffectParamBase>(new EffectParamBase[] {
                new RangeParam("Radius", 1, 100, 10)
            });
        }

        internal override void GetParameters()
        {
            float radiusPercent = ((RangeParam)Parameters[0]).Value * .01f;
            float quarter = Math.Max(maxX, maxY) * .25f;
            radius = (int)(quarter * radiusPercent);
        }

        public override void Apply(IPixel[] pixels, System.Windows.Media.Imaging.WriteableBitmap modified, System.ComponentModel.BackgroundWorker worker)
        {
            maxX = modified.PixelWidth;
            maxY = modified.PixelHeight;
            // initialize parameters
            GetParameters();
            // testing method
            ApplyInternal(pixels, modified, worker);
        }

        public override void Apply(int[] pixels, System.Windows.Media.Imaging.WriteableBitmap modified, System.ComponentModel.BackgroundWorker worker)
        {
            maxX = modified.PixelWidth;
            maxY = modified.PixelHeight;
            // initialize parameters
            GetParameters();
            // testing method
            ApplyInternal(pixels, modified, worker);
        }

        protected void ApplyInternal(IPixel[] pixels, System.Windows.Media.Imaging.WriteableBitmap modified, System.ComponentModel.BackgroundWorker worker)
        {

            int w = maxX;
            int h = maxY;
            int wm = w - 1;
            int hm = h - 1;
            int wh = w * h;
            int div = radius + radius + 1;

            int[] r = new int[wh];
            int[] g = new int[wh];
            int[] b = new int[wh];

            int rsum, gsum, bsum, x, y, i, yp, yi, yw;
            ColorB p;
            int[] vmin = new int[Math.Max(w,h)];

            int divsum = (div + 1) >> 1;
            divsum *= divsum;
            int[] dv = new int[256 * divsum];
            for (i = 0; i < 256 * divsum; i++)
            {
                dv[i]=(i/divsum);
            }

            yw = yi = 0;

            int[][] stack = new int[div][];
            for (i = 0; i < div; i++) stack[i] = new int[3];
            int stackpointer;
            int stackstart;
            int[] sir;
            int rbs;
            int r1 = radius + 1;
            int routsum, goutsum, boutsum;
            int rinsum, ginsum, binsum;

            for (y = 0; y < h; y++)
            {
                if (worker != null && worker.CancellationPending) break;
                rinsum = ginsum = binsum = routsum = goutsum = boutsum = rsum = gsum = bsum = 0;

                for(i = -radius; i <= radius; i++)
                {
                    p = (ColorB)pixels[yi+ Math.Min(wm, Math.Max(i, 0))];
                    sir = stack[i + radius];
                    sir[0] = p.Red;
                    sir[1] = p.Green;
                    sir[2] = p.Blue;
                    rbs = r1 - Math.Abs(i);
                    rsum += sir[0] * rbs;
                    gsum += sir[1] * rbs;
                    bsum += sir[2] * rbs;
                    if (i > 0)
                    {
                        rinsum += sir[0];
                        ginsum += sir[1];
                        binsum += sir[2];
                    } 
                    else 
                    {
                        routsum += sir[0];
                        goutsum += sir[1];
                        boutsum += sir[2];
                    }
                }
                stackpointer = radius;

                for (x = 0; x < w; x++)
                {
                    r[yi] = dv[rsum];
                    g[yi] = dv[gsum];
                    b[yi] = dv[bsum];
      
                    rsum -= routsum;
                    gsum -= goutsum;
                    bsum -= boutsum;

                    stackstart = stackpointer - radius + div;
                    sir = stack[stackstart % div];
      
                    routsum -= sir[0];
                    goutsum -= sir[1];
                    boutsum -= sir[2];
      
                    if(y==0)
                    {
                        vmin[x] = Math.Min(x + radius + 1, wm);
                    }

                    p = (ColorB)pixels[yw + vmin[x]];
      
                    sir[0]= p.Red;
                    sir[1]= p.Green;
                    sir[2]= p.Blue;

                    rinsum += sir[0];
                    ginsum += sir[1];
                    binsum += sir[2];

                    rsum += rinsum;
                    gsum += ginsum;
                    bsum += binsum;
      
                    stackpointer = (stackpointer + 1) % div;
                    sir = stack[(stackpointer) % div];
     
                    routsum += sir[0];
                    goutsum += sir[1];
                    boutsum += sir[2];
     
                    rinsum -= sir[0];
                    ginsum -= sir[1];
                    binsum -= sir[2];
     
                    yi++;
                }

                yw+=w;
            }

            for (x = 0; x < w; x++)
            {
                if (worker != null && worker.CancellationPending) break;
                rinsum = ginsum = binsum = routsum = goutsum = boutsum = rsum = gsum = bsum = 0;
                yp =- radius * w;
                for(i = -radius; i <= radius; i++)
                {
                    yi = Math.Max(0, yp) + x;
     
                    sir = stack[i + radius];
      
                    sir[0] = r[yi];
                    sir[1] = g[yi];
                    sir[2] = b[yi];
     
                    rbs = r1 - Math.Abs(i);
      
                    rsum += r[yi] * rbs;
                    gsum += g[yi] * rbs;
                    bsum += b[yi] * rbs;
     
                    if (i > 0)
                    {
                        rinsum += sir[0];
                        ginsum += sir[1];
                        binsum += sir[2];
                    } 
                    else 
                    {
                        routsum += sir[0];
                        goutsum += sir[1];
                        boutsum += sir[2];
                    }
      
                    if(i < hm)
                    {
                        yp += w;
                    }
                }

                yi = x;
                stackpointer = radius;
                for (y = 0; y < h; y++)
                {
                    if (worker != null && worker.CancellationPending) break;
                    modified.Pixels[yi] = new ColorB(255, dv[rsum], dv[gsum], dv[bsum]);

                    rsum -= routsum;
                    gsum -= goutsum;
                    bsum -= boutsum;

                    stackstart = stackpointer - radius + div;
                    sir = stack[stackstart % div];
     
                    routsum -= sir[0];
                    goutsum -= sir[1];
                    boutsum -= sir[2];
     
                    if(x == 0)
                    {
                        vmin[y] = Math.Min(y + r1, hm) * w;
                    }
                    p= x + vmin[y];
      
                    sir[0] = r[p];
                    sir[1] = g[p];
                    sir[2] = b[p];
      
                    rinsum += sir[0];
                    ginsum += sir[1];
                    binsum += sir[2];

                    rsum += rinsum;
                    gsum += ginsum;
                    bsum += binsum;

                    stackpointer = (stackpointer + 1) % div;
                    sir = stack[stackpointer];
     
                    routsum += sir[0];
                    goutsum += sir[1];
                    boutsum += sir[2];
      
                    rinsum -= sir[0];
                    ginsum -= sir[1];
                    binsum -= sir[2];

                    yi += w;
                }
            }
 
        }

        protected void ApplyInternal(int[] pixels, System.Windows.Media.Imaging.WriteableBitmap modified, System.ComponentModel.BackgroundWorker worker)
        {

            int w = maxX;
            int h = maxY;
            int wm = w - 1;
            int hm = h - 1;
            int wh = w * h;
            int div = radius + radius + 1;

            int[] r = new int[wh];
            int[] g = new int[wh];
            int[] b = new int[wh];

            int rsum, gsum, bsum, x, y, i, yp, yi, yw;
            ColorB p;
            int[] vmin = new int[Math.Max(w, h)];

            int divsum = (div + 1) >> 1;
            divsum *= divsum;
            int[] dv = new int[256 * divsum];
            for (i = 0; i < 256 * divsum; i++)
            {
                dv[i] = (i / divsum);
            }

            yw = yi = 0;

            int[][] stack = new int[div][];
            for (i = 0; i < div; i++) stack[i] = new int[3];
            int stackpointer;
            int stackstart;
            int[] sir;
            int rbs;
            int r1 = radius + 1;
            int routsum, goutsum, boutsum;
            int rinsum, ginsum, binsum;

            for (y = 0; y < h; y++)
            {
                if (worker != null && worker.CancellationPending) break;
                rinsum = ginsum = binsum = routsum = goutsum = boutsum = rsum = gsum = bsum = 0;

                for (i = -radius; i <= radius; i++)
                {
                    p = (ColorB)pixels[yi + Math.Min(wm, Math.Max(i, 0))];
                    sir = stack[i + radius];
                    sir[0] = p.Red;
                    sir[1] = p.Green;
                    sir[2] = p.Blue;
                    rbs = r1 - Math.Abs(i);
                    rsum += sir[0] * rbs;
                    gsum += sir[1] * rbs;
                    bsum += sir[2] * rbs;
                    if (i > 0)
                    {
                        rinsum += sir[0];
                        ginsum += sir[1];
                        binsum += sir[2];
                    }
                    else
                    {
                        routsum += sir[0];
                        goutsum += sir[1];
                        boutsum += sir[2];
                    }
                }
                stackpointer = radius;

                for (x = 0; x < w; x++)
                {
                    r[yi] = dv[rsum];
                    g[yi] = dv[gsum];
                    b[yi] = dv[bsum];

                    rsum -= routsum;
                    gsum -= goutsum;
                    bsum -= boutsum;

                    stackstart = stackpointer - radius + div;
                    sir = stack[stackstart % div];

                    routsum -= sir[0];
                    goutsum -= sir[1];
                    boutsum -= sir[2];

                    if (y == 0)
                    {
                        vmin[x] = Math.Min(x + radius + 1, wm);
                    }

                    p = (ColorB)pixels[yw + vmin[x]];

                    sir[0] = p.Red;
                    sir[1] = p.Green;
                    sir[2] = p.Blue;

                    rinsum += sir[0];
                    ginsum += sir[1];
                    binsum += sir[2];

                    rsum += rinsum;
                    gsum += ginsum;
                    bsum += binsum;

                    stackpointer = (stackpointer + 1) % div;
                    sir = stack[(stackpointer) % div];

                    routsum += sir[0];
                    goutsum += sir[1];
                    boutsum += sir[2];

                    rinsum -= sir[0];
                    ginsum -= sir[1];
                    binsum -= sir[2];

                    yi++;
                }

                yw += w;
            }

            for (x = 0; x < w; x++)
            {
                if (worker != null && worker.CancellationPending) break;
                rinsum = ginsum = binsum = routsum = goutsum = boutsum = rsum = gsum = bsum = 0;
                yp = -radius * w;
                for (i = -radius; i <= radius; i++)
                {
                    yi = Math.Max(0, yp) + x;

                    sir = stack[i + radius];

                    sir[0] = r[yi];
                    sir[1] = g[yi];
                    sir[2] = b[yi];

                    rbs = r1 - Math.Abs(i);

                    rsum += r[yi] * rbs;
                    gsum += g[yi] * rbs;
                    bsum += b[yi] * rbs;

                    if (i > 0)
                    {
                        rinsum += sir[0];
                        ginsum += sir[1];
                        binsum += sir[2];
                    }
                    else
                    {
                        routsum += sir[0];
                        goutsum += sir[1];
                        boutsum += sir[2];
                    }

                    if (i < hm)
                    {
                        yp += w;
                    }
                }

                yi = x;
                stackpointer = radius;
                for (y = 0; y < h; y++)
                {
                    if (worker != null && worker.CancellationPending) break;
                    modified.Pixels[yi] = new ColorB(255, dv[rsum], dv[gsum], dv[bsum]);

                    rsum -= routsum;
                    gsum -= goutsum;
                    bsum -= boutsum;

                    stackstart = stackpointer - radius + div;
                    sir = stack[stackstart % div];

                    routsum -= sir[0];
                    goutsum -= sir[1];
                    boutsum -= sir[2];

                    if (x == 0)
                    {
                        vmin[y] = Math.Min(y + r1, hm) * w;
                    }
                    p = x + vmin[y];

                    sir[0] = r[p];
                    sir[1] = g[p];
                    sir[2] = b[p];

                    rinsum += sir[0];
                    ginsum += sir[1];
                    binsum += sir[2];

                    rsum += rinsum;
                    gsum += ginsum;
                    bsum += binsum;

                    stackpointer = (stackpointer + 1) % div;
                    sir = stack[stackpointer];

                    routsum += sir[0];
                    goutsum += sir[1];
                    boutsum += sir[2];

                    rinsum -= sir[0];
                    ginsum -= sir[1];
                    binsum -= sir[2];

                    yi += w;
                }
            }

        }

    }
}
