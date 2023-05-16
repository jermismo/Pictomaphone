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
    public class DreamyGlow : StackBlur
    {

        new public const string Name = "DreamyGlow";

        private int intensity;

        public DreamyGlow() : base()
        {
            Parameters = new System.Collections.ObjectModel.ReadOnlyCollection<EffectParamBase>(new EffectParamBase[] 
            {
                new RangeParam("Glow", 4, 15, 5),
                new RangeParam("Intensity", 0, 255, 200)
            });
        }

        internal override void GetParameters()
        {
            base.GetParameters(); // gets radius for blur
            intensity = (int)((RangeParam)Parameters[1]).Value;
        }

        public override void Apply(IPixel[] pixels, System.Windows.Media.Imaging.WriteableBitmap modified, System.ComponentModel.BackgroundWorker worker)
        {
            maxX = modified.PixelWidth;
            maxY = modified.PixelHeight;
            // initialize parameters
            GetParameters();
            // blur method
            ApplyInternal(pixels, modified, worker);

            // glow
            for (int i = 0; i < pixels.Length; i++)
            {
                if (worker != null && worker.CancellationPending) break;
                ColorB orig = (ColorB)pixels[i];
                ColorB temp = (ColorB)modified.Pixels[i];

                int alpha = (orig.GetIntensity() + intensity) / 2;
                modified.Pixels[i] = orig.BlendOverlay(temp, alpha);
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

            // glow
            for (int i = 0; i < pixels.Length; i++)
            {
                if (worker != null && worker.CancellationPending) break;
                ColorB orig = (ColorB)pixels[i];
                ColorB temp = (ColorB)modified.Pixels[i];

                int alpha = (orig.GetIntensity() + intensity) / 2;
                modified.Pixels[i] = orig.BlendOverlay(temp, alpha);
            }
        }

    }
}
