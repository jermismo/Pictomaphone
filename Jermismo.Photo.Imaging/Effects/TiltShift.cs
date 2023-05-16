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

namespace Jermismo.Photo.Imaging.Effects
{
    public class TiltShift : StackBlur
    {

        new public const string Name = "TiltShift";

        private float offset = 0;
        int[] mask;

        private Vibrance vibrance = new Vibrance();

        public TiltShift()
            : base()
        {
            Parameters = new System.Collections.ObjectModel.ReadOnlyCollection<EffectParamBase>(new EffectParamBase[] 
            {
                new RangeParam("Blur Amount", 1, 15, 5),
                new RangeParam("Vibrance", 0, 40, 20),
                new LineSelectParam(0.5f, Orientation.Horizontal),
                new TextParam("Touch the image to move the focus", true)
            });
        }

        internal override void GetParameters()
        {
            base.GetParameters(); // gets radius for blur
            ((RangeParam)vibrance.Parameters[0]).Value = ((RangeParam)Parameters[1]).Value;
            offset = ((LineSelectParam)Parameters[2]).Offset;
            CreateMask();
            vibrance.GetParameters();
        }

        public override void Apply(IPixel[] pixels, System.Windows.Media.Imaging.WriteableBitmap modified, System.ComponentModel.BackgroundWorker worker)
        {
            maxX = modified.PixelWidth;
            maxY = modified.PixelHeight;
            // initialize parameters
            GetParameters();
            // blur method
            ApplyInternal(pixels, modified, worker);

            // tilt shift
            int index = 0;
            for (int y = 0; y < maxY; y++)
            {
                for (int x = 0; x < maxX; x++)
                {
                    if (worker != null && worker.CancellationPending) break;
                    modified.Pixels[index] = PixelOperation((ColorB)pixels[index], (ColorB)modified.Pixels[index], x, y);
                    index++;
                }
            }

        }

        public override void Apply(int[] pixels, System.Windows.Media.Imaging.WriteableBitmap modified, System.ComponentModel.BackgroundWorker worker)
        {
            maxX = modified.PixelWidth;
            maxY = modified.PixelHeight;
            // initialize parameters
            GetParameters();
            // blur method
            ApplyInternal(pixels, modified, worker);

            // tilt shift
            int index = 0;
            for (int y = 0; y < maxY; y++)
            {
                for (int x = 0; x < maxX; x++)
                {
                    if (worker != null && worker.CancellationPending) break;
                    modified.Pixels[index] = PixelOperation((ColorB)pixels[index], (ColorB)modified.Pixels[index], x, y);
                    index++;
                }
            }
        }

        internal ColorB PixelOperation(ColorB orig, ColorB mod, int x, int y)
        {
            ColorB temp = orig.BlendAlpha(mod, mask[y]);
            CieLab lab = CieLab.FromColorB(temp);
            lab = (CieLab)vibrance.PixelOperation(lab);
            return lab.ToColorB();
        }

        private void CreateMask()
        {
            int range = maxY / 6;
            int grad = (int)(range * 1.2f);

            int maskCenter = (int)(maxY * offset);

            int start1 = maskCenter - range - grad;
            int end1 = maskCenter - range + grad;
            int start2 = maskCenter + range - grad;
            int end2 = maskCenter + range + grad;

            mask = new int[maxY];

            for (int y = 0; y < maxY; y++)
            {
                if (y < start1 || y > end2)
                {   // ends
                    mask[y] = 255;
                }
                else if (y > end1 && y < start2)
                {   // middle
                    mask[y] = 0;
                }
                else if (y <= end1)
                {   // top
                    float a = y - start1;
                    float b = end1 - start1;
                    float c = a / b;
                    mask[y] = 255 - (int)(255 * c);
                }
                else 
                {   // bottom
                    float a = y - start2;
                    float b = end2 - start2;
                    float c = a / b;
                    mask[y] = (int)(255 * c);
                }
            }

        }

    }
}
