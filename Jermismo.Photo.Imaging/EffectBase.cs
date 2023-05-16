using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media.Imaging;

namespace Jermismo.Photo.Imaging
{
    /// <summary>
    /// Defines properties of an Effect Pass
    /// </summary>
    public class EffectPass
    {
        /// <summary>
        /// The Pass Requires (X,Y) Coordinates to be passed in.
        /// </summary>
        public bool RequireXY = false;
        /// <summary>
        /// The Pass needs perform the XY loop using Y in the first loop.
        /// </summary>
        public bool YCoordFirst = true;
        /// <summary>
        /// The Pass needs to write its output to a temp array instead of the output bitmap.
        /// </summary>
        public bool WriteToTemp = false;
    }

    /// <summary>
    /// The color-space the effect uses.
    /// </summary>
    public enum EffectColorSpace { RGB, HSL, LAB }

    public abstract class EffectBase
    {

        protected List<EffectPass> Passes = new List<EffectPass>();
        protected int currentPass = 0;
        protected EffectColorSpace colorspace = EffectColorSpace.RGB;
        protected int maxX = 0;
        protected int maxY = 0;
        protected IPixel[] tempPixels = null;
        
        public ReadOnlyCollection<EffectParamBase> Parameters { get; protected set; }

        public EffectColorSpace ColorSpace { get { return colorspace; } }

        /// <summary>
        /// Applies the effect to the Writeable Bitmap.
        /// </summary>
        /// <param name="pixels">The unmodified pixels.</param>
        /// <param name="modified">The image to modify.</param>
        /// <param name="worker">The worker thread instance.</param>
        public virtual void Apply(int[] pixels, WriteableBitmap modified, BackgroundWorker worker)
        {
            // set extra info
            maxX = modified.PixelWidth;
            maxY = modified.PixelHeight;
            // initialize parameters
            GetParameters();
            // set pixel getters
            GetPixelXy = (x, y) => Convert(pixels[y * maxX + x], this.colorspace);
            GetTempXy = (x, y) => tempPixels[y * maxX + x];

            // add default pass if it doesn't exist
            if (Passes.Count == 0) Passes.Add(new EffectPass());
            // check if we need to initialize the temp array
            foreach (EffectPass pass in Passes)
            {
                if (pass.WriteToTemp)
                {
                    tempPixels = new IPixel[pixels.Length];
                    break;
                }
            }
            // run passes
            currentPass = 0;
            foreach (EffectPass pass in Passes)
            {
                if (!pass.RequireXY) RunPass(pixels, modified, worker);
                else
                {
                    if (pass.YCoordFirst) RunPassYFirst(pixels, modified, worker);
                    else RunPassXFirst(pixels, modified, worker);
                }
                currentPass++;
            }
        }

        /// <summary>
        /// Applies the effect to the Writeable Bitmap.
        /// </summary>
        /// <param name="pixels">The unmodified pixels.</param>
        /// <param name="modified">The image to modify.</param>
        /// <param name="worker">The worker thread instance.</param>
        public virtual void Apply(IPixel[] pixels, WriteableBitmap modified, BackgroundWorker worker)
        {
            // set extra info
            maxX = modified.PixelWidth;
            maxY = modified.PixelHeight;
            // initialize parameters
            GetParameters();
            // set pixel getters
            GetPixelXy = (x, y) => pixels[y * maxX + x];
            GetTempXy = (x, y) => tempPixels[y * maxX + x];
            
            // add default pass if it doesn't exist
            if (Passes.Count == 0) Passes.Add(new EffectPass());
            // check if we need to initialize the temp array
            foreach (EffectPass pass in Passes)
            {
                if (pass.WriteToTemp)
                {
                    tempPixels = new IPixel[pixels.Length];
                    break;
                }
            }
            // run passes
            currentPass = 0;
            foreach (EffectPass pass in Passes)
            {
                if (!pass.RequireXY) RunPass(pixels, modified, worker);
                else
                {
                    if (pass.YCoordFirst) RunPassYFirst(pixels, modified, worker);
                    else RunPassXFirst(pixels, modified, worker);
                }
                currentPass++;
            }
        }

        internal abstract void GetParameters();

        internal virtual IPixel PixelOperation(IPixel pixel) { return pixel; }
        internal virtual IPixel PixelOperation(IPixel pixel, int x, int y) { return pixel; }

        protected Func<int, int, IPixel> GetPixelXy;
        protected Func<int, int, IPixel> GetTempXy;

        protected virtual IPixel Convert(int value, EffectColorSpace space)
        {
            if (space == EffectColorSpace.RGB) return new ColorB(value);
            else if (space == EffectColorSpace.HSL) return new Hsl(value);
            else return new CieLab(value);
        }

        internal virtual void SetSize(int x, int y)
        {
            maxX = x;
            maxY = y;
        }

        internal virtual void RunPass(IPixel[] pixels, WriteableBitmap modified, BackgroundWorker worker)
        {
            for (int i = 0; i < pixels.Length; i++)
            {
                if (worker != null && worker.CancellationPending) break;
                modified.Pixels[i] = PixelOperation(pixels[i]).GetInt();
            }
        }

        internal virtual void RunPass(int[] pixels, WriteableBitmap modified, BackgroundWorker worker)
        {
            for (int i = 0; i < pixels.Length; i++)
            {
                if (worker != null && worker.CancellationPending) break;
                IPixel p = Convert(pixels[i], this.colorspace);
                modified.Pixels[i] = PixelOperation(p).GetInt();
            }
        }

        internal virtual void RunPassYFirst(IPixel[] pixels, WriteableBitmap modified, BackgroundWorker worker)
        {
            int index = 0;
            bool useTemp = Passes[currentPass].WriteToTemp;
            for (int y = 0; y < maxY; y++)
            {
                for (int x = 0; x < maxX; x++)
                {
                    if (worker != null && worker.CancellationPending) break;
                    IPixel pixel = PixelOperation(pixels[index], x, y);
                    if (useTemp)
                    {
                        tempPixels[index] = pixel;
                    }
                    else
                    {
                        modified.Pixels[index] = pixel.GetInt();
                    }
                    index++;
                }
            }
        }

        internal virtual void RunPassYFirst(int[] pixels, WriteableBitmap modified, BackgroundWorker worker)
        {
            int index = 0;
            bool useTemp = Passes[currentPass].WriteToTemp;
            for (int y = 0; y < maxY; y++)
            {
                for (int x = 0; x < maxX; x++)
                {
                    if (worker != null && worker.CancellationPending) break;
                    IPixel pixel = PixelOperation(Convert(pixels[index], colorspace), x, y);
                    if (useTemp)
                    {
                        tempPixels[index] = pixel;
                    }
                    else
                    {
                        modified.Pixels[index] = pixel.GetInt();
                    }
                    index++;
                }
            }
        }

        internal virtual void RunPassXFirst(IPixel[] pixels, WriteableBitmap modified, BackgroundWorker worker)
        {
            bool useTemp = Passes[currentPass].WriteToTemp;
            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    int index = y * maxX + x;
                    if (worker != null && worker.CancellationPending) break;
                    IPixel pixel = PixelOperation(pixels[index], x, y);
                    if (useTemp)
                    {
                        tempPixels[index] = pixel;
                    }
                    else
                    {
                        modified.Pixels[index] = pixel.GetInt();
                    }
                    index++;
                }
            }
        }

        internal virtual void RunPassXFirst(int[] pixels, WriteableBitmap modified, BackgroundWorker worker)
        {
            bool useTemp = Passes[currentPass].WriteToTemp;
            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    int index = y * maxX + x;
                    if (worker != null && worker.CancellationPending) break;
                    IPixel pixel = PixelOperation(Convert(pixels[index], colorspace), x, y);
                    if (useTemp)
                    {
                        tempPixels[index] = pixel;
                    }
                    else
                    {
                        modified.Pixels[index] = pixel.GetInt();
                    }
                    index++;
                }
            }
        }

    }
}
