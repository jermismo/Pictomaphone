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
using System.ComponentModel;
using System.IO;

namespace Jermismo.Photo.Imaging
{
    public class EffectImage : DependencyObject
    {

        private WriteableBitmap originalImage;
        private WriteableBitmap workingImage;
        private BackgroundWorker worker;
        private EffectBase effect;

        private IPixel[] pixels;

        private int workingHeight;
        private int workingWidth;

        private bool waitingToRunAgain = false;
        private bool isSaveMode = false;

        public event EventHandler<EventArgs> EffectCompleted;

        /// <summary>
        /// Dependency Property for IsBusy.
        /// </summary>
        public static readonly DependencyProperty IsBusyProperty = DependencyProperty.Register("IsBusy", typeof(bool), typeof(EffectImage), null);
        /// <summary>
        /// Tells whether or not there is an effect currently running.
        /// </summary>
        public bool IsBusy 
        { 
            get 
            { 
                return (bool)this.GetValue(IsBusyProperty); 
            }
            private set
            {
                this.SetValue(IsBusyProperty, value);
            }
        }

        /// <summary>
        /// The original full-size image.
        /// </summary>
        public WriteableBitmap OriginalImage { get { return originalImage; } }

        /// <summary>
        /// The modified version of the image.
        /// </summary>
        public WriteableBitmap ModifiedImage { get { return workingImage; } }

        /// <summary>
        /// Creates a new instance of the Effect Image class.
        /// </summary>
        /// <param name="original">The original image</param>
        /// <param name="maxWorkingSize">The max size of the working copy</param>
        public EffectImage(WriteableBitmap original, int maxWorkingSize, EffectBase effect)
        {
            originalImage = original;
            this.effect = effect;

            GetWorkingSize(maxWorkingSize);

            CreateWorkingImage();

            worker = new BackgroundWorker();
            worker.WorkerSupportsCancellation = true;
            worker.WorkerReportsProgress = false;
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
        }

        /// <summary>
        /// Applies the specified effect.
        /// </summary>
        public void ApplyEffect()
        {
            if (effect == null) throw new ArgumentNullException("effect");

            if (!worker.IsBusy)
            {
                waitingToRunAgain = false;
                worker.RunWorkerAsync();
            }
            else
            {
                waitingToRunAgain = true;
                worker.CancelAsync();
            }
            // mark self as busy
            this.IsBusy = true;
        }

        public void SaveEffect()
        {
            CancelEffect();
            isSaveMode = true;
            worker.RunWorkerAsync();
        }

        public void CancelEffect()
        {
            waitingToRunAgain = false;
            if (worker.IsBusy)
            {
                worker.CancelAsync();
                while (worker.IsBusy) { System.Threading.Thread.Sleep(5); }
            }
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (isSaveMode)
            {
                int[] pix = new int[originalImage.Pixels.Length];
                originalImage.Pixels.CopyTo(pix, 0);
                effect.Apply(pix, originalImage, worker);
            }
            else
            {
                effect.Apply(pixels, workingImage, worker);
            }
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (waitingToRunAgain && !isSaveMode && !worker.IsBusy)
            {
                waitingToRunAgain = false;
                worker.RunWorkerAsync();
            }
            else if (!isSaveMode)
            {
                this.IsBusy = false;
                if (EffectCompleted != null) EffectCompleted(this, EventArgs.Empty);
            }
            else // save mode
            {
                this.IsBusy = false;
                if (EffectCompleted != null) EffectCompleted(this, EventArgs.Empty);
            }
            workingImage.Dispatcher.BeginInvoke(() => workingImage.Invalidate());
        }

        /// <summary>
        /// Gets the size for the image that will be edited.
        /// </summary>
        private void GetWorkingSize(int maxWorkingSize)
        {
            workingWidth = originalImage.PixelWidth;
            workingHeight = originalImage.PixelHeight;

            if (workingHeight > workingWidth)
            {
                float ratio = (float)workingWidth / (float)workingHeight;
                workingHeight = maxWorkingSize;
                workingWidth = (int)(maxWorkingSize * ratio);
            }
            else
            {
                float ratio = (float)workingHeight / (float)workingWidth;
                workingWidth = maxWorkingSize;
                workingHeight = (int)(maxWorkingSize * ratio);
            }
        }

        /// <summary>
        /// Creates the working copy.
        /// </summary>
        private void CreateWorkingImage()
        {
            // create a scaled version of the original to work on
            using (MemoryStream mem = new MemoryStream())
            {
                originalImage.SaveJpeg(mem, workingWidth, workingHeight, 0, 100);
                workingImage = new WriteableBitmap(workingWidth, workingHeight);
                mem.Position = 0;
                workingImage.LoadJpeg(mem);
            }

            // get the pixel collections
            pixels = new IPixel[workingImage.Pixels.Length];
            //originalPixels = new IPixel[originalImage.Pixels.Length];
            if (effect.ColorSpace == EffectColorSpace.RGB)
            {
                pixels = workingImage.GetIPixelRgb();
            }
            else if (effect.ColorSpace == EffectColorSpace.HSL)
            {
                pixels = workingImage.GetIPixelHsl();
            }
            else
            {
                pixels = workingImage.GetIPixelLab();
            }

        }

    }
}
