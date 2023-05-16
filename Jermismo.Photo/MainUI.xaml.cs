using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Globalization;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using Microsoft.Xna.Framework.Media;
using Jermismo.Photo.Resources;
using Jermismo.Photo.Imaging;
using adj = Jermismo.Photo.Imaging.Adjustments;
using efc = Jermismo.Photo.Imaging.Effects;
using brd = Jermismo.Photo.Imaging.Borders;
using flt = Jermismo.Photo.Imaging.Filters;
using Jermismo.Photo.Controls;

namespace Jermismo.Photo
{
    public partial class MainUI : PhoneApplicationPage
    {

        private enum NavigationState { Root, Tools, Effect }

        private bool firstLoad = true;
        private bool isEffectSaving = false;
        private NavigationState navState = NavigationState.Root;
        private FrameworkElement lastTool = null;

        private EffectImage image = null;
        private EffectBase effect = null;
        private WriteableBitmap tempImage = null;

        public MainUI()
        {
            InitializeComponent();
        }

        #region * Override Page Events *

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            // check for show trial message if the app just loaded and is in trial mode
            if (firstLoad && App.Current.IsTrial)
            {
                TimeSpan diff = DateTime.Now - AppSettings.Instance.LastTrialMessageDate;
                if (diff.TotalDays > 14)
                {
                    AppSettings.Instance.LastTrialMessageDate = DateTime.Now;
                    NavigationService.Navigate(new Uri("/TrialInfo.xaml", UriKind.Relative));
                }
            }
            firstLoad = false;

            base.OnNavigatedTo(e);

            //Gets a dictionary of query string keys and values
            IDictionary<string, string> queryStrings = this.NavigationContext.QueryString;

            //This code ensures that there is at least one key in the query string, and check if the "token" key is present.
            if (queryStrings.ContainsKey("token"))
            {
                //This code retrieves the picture from the local Zune Media Database using the token passed to the application.
                try
                {
                    MediaLibrary library = new MediaLibrary();
                    Picture picture = library.GetPictureFromToken(queryStrings["token"]);

                    using (Stream stream = picture.GetImage())
                    {
                        AppSettings.Instance.SetOriginalImage(stream, picture.Name);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(AppResources.OpenPhotoError + "\n" + ex.Message, AppResources.ErrorMessageTitle, MessageBoxButton.OK);
                }
                finally
                {
                    // remove the token so this doesn't load again
                    if (this.NavigationContext.QueryString.ContainsKey("token"))
                        this.NavigationContext.QueryString.Remove("token");
                }
            }
            else if (App.Current.CurrentBitmap == null)
            {
                // load the last bitmap the user had opened
                WriteableBitmap image = AppSettings.Instance.GetOriginalImage();
                if (image != null)
                {
                    App.Current.CurrentBitmap = image;
                }
                else
                {
                    DisableBottomTools();
                }
            }

            mainImage.Source = App.Current.CurrentBitmap;
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (navState == NavigationState.Root)
            {
                base.OnBackKeyPress(e);
            }
            else
            {
                if (navState == NavigationState.Effect)
                {
                    CancelEffect();
                }
                GoBackMenu();
                e.Cancel = true;
            }
        }

        #endregion

        #region * Top Tools Click Handlers *

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            PhotoChooserTask task = new PhotoChooserTask();
            task.Completed += new EventHandler<PhotoResult>(GenericImageTask_Completed);
            task.Show();
        }

        private void TakeButton_Click(object sender, RoutedEventArgs e)
        {
            CameraCaptureTask cameraCaptureClass = new CameraCaptureTask();
            cameraCaptureClass.Completed += new EventHandler<PhotoResult>(GenericImageTask_Completed);
            cameraCaptureClass.Show();
        }

        // runs after an image has been selected from either the camera capture or photo chooser tasks
        private void GenericImageTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                string name = Path.GetFileName(e.OriginalFileName);
                name = (name.StartsWith("PhotoChooser-")) ? "[chooser]" : e.OriginalFileName;
                AppSettings.Instance.SetOriginalImage(e.ChosenPhoto, name);

                Dispatcher.BeginInvoke(() =>
                {
                    mainImage.Source = App.Current.CurrentBitmap;
                });

                EnableBottomTools();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckForLoadedImage()) return;
            NavigationService.Navigate(new Uri("/EffectPages/SavePage.xaml", UriKind.Relative));
        }

        private void ShareButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/YourLastAboutDialog;component/AboutPage.xaml", UriKind.Relative));
        }

        #endregion

        #region * Bottom Tools Click Handlers *

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            BottomTools.Hide();
            EditTools.Show();
            lastTool = EditTools;
            navState = NavigationState.Tools;
        }

        private void AdjustButton_Click(object sender, RoutedEventArgs e)
        {
            BottomTools.Hide();
            AdjustTools.Show();
            lastTool = AdjustTools;
            navState = NavigationState.Tools;
        }

        private void EffectButton_Click(object sender, RoutedEventArgs e)
        {
            BottomTools.Hide();
            EffectTools.Show();
            lastTool = EffectTools;
            navState = NavigationState.Tools;
        }

        private void FilterButton_Click(object sender, RoutedEventArgs e)
        {
            BottomTools.Hide();
            FilterTools.Show();
            lastTool = FilterTools;
            navState = NavigationState.Tools;
        }

        #endregion

        #region * Effect Click Handlers *

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            TopTools.Hide();
            BottomTools.Hide();
            EditTools.Hide();

            EffectButton btn = sender as EffectButton;
            switch (btn.Name)
            {
                case "Crop": DoCrop(); break;
                case "Resize": DoResize(); break;
                case "Rotate": DoRotate(); break;
                case "Revert": DoRevert(); break;
                default: break;
            }
        }

        private void Adjustment_Click(object sender, RoutedEventArgs e)
        {
            TopTools.Hide();
            BottomTools.Hide();
            AdjustTools.Hide();
            
            EffectButton btn = sender as EffectButton;
            switch (btn.Name)
            {
                case "Brightness": effect = EffectList.Items[adj.BrightnessContrast.Name]; break;
                case "Levels": effect = EffectList.Items[adj.Levels.Name]; break;
                case "Overlay": effect = EffectList.Items[adj.ChannelOverlay.Name]; break;
                case "Mixer": effect = EffectList.Items[adj.ChannelMixer.Name]; break;
                case "Balance": effect = EffectList.Items[adj.ColorBalance.Name]; break;
                case "Temperature": effect = EffectList.Items[adj.TemperatureTone.Name]; break;
                case "Vibrance": effect = EffectList.Items[adj.Vibrance.Name]; break;
                case "HueSat": effect = EffectList.Items[adj.HueSaturation.Name]; break;
                case "Colorize": effect = EffectList.Items[adj.Colorize.Name]; break;
                case "ColorFilter": effect = EffectList.Items[adj.ColorFilter.Name]; break;
                case "Grayscale": effect = EffectList.Items[adj.Grayscale.Name]; break;
                case "Negative": effect = EffectList.Items[adj.Negative.Name]; break;
                case "Solarize": effect = EffectList.Items[adj.Solarize.Name]; break;
                case "Posterize": effect = EffectList.Items[adj.Posterize.Name]; break;
            }
            SetEffect();

            effectStack.Show();
        }

        private void Effect_Click(object sender, RoutedEventArgs e)
        {
            TopTools.Hide();
            BottomTools.Hide();
            EffectTools.Hide();

            EffectButton btn = sender as EffectButton;
            switch (btn.Name)
            {
                case "Blur": effect = EffectList.Items[efc.StackBlur.Name]; break;
                case "TiltShift": effect = EffectList.Items[efc.TiltShift.Name]; break;
                case "DreamyGlow": effect = EffectList.Items[efc.DreamyGlow.Name]; break;
                case "Vignette": effect = EffectList.Items[efc.Vignette.Name]; break;
                case "FrostedGlass": effect = EffectList.Items[efc.FrostedGlass.Name]; break;
                case "SquareBorder": effect = EffectList.Items[brd.SquareBorder.Name]; break;
            }
            SetEffect();

            effectStack.Show();
        }

        private void Filter_Click(object sender, RoutedEventArgs e)
        {
            TopTools.Hide();
            BottomTools.Hide();
            FilterTools.Hide();

            EffectButton btn = sender as EffectButton;
            switch (btn.Name)
            {
                case "Sepia": effect = EffectList.Items[adj.Sepia.Name]; break;
                case "CrossProcess": effect = EffectList.Items[flt.CrossProcess.Name]; break;
                case "Country": effect = EffectList.Items[flt.Country.Name]; break;
                case "Brawny": effect = EffectList.Items[flt.Brawny.Name]; break;
                case "SundayMorning": effect = EffectList.Items[flt.SundayMorning.Name]; break;
                case "SummerForest": effect = EffectList.Items[flt.SummerForest.Name]; break;
                case "OldNewYork": effect = EffectList.Items[flt.OldNewYork.Name]; break;
                case "Lomo": effect = EffectList.Items[flt.Lomo.Name]; break;
                case "Velvian": effect = EffectList.Items[flt.Velvian.Name]; break;
            }
            SetEffect();

            effectStack.Show();
        }

        private void SetEffect()
        {
            image = new EffectImage(App.Current.CurrentBitmap, AppSettings.MaxEditImageSize, effect);
            image.EffectCompleted += new EventHandler<EventArgs>(image_EffectCompleted);
            mainImage.Source = image.ModifiedImage;

            effectPanel.Clear();
            foreach (var item in effect.Parameters)
            {
                effectPanel.Items.Add(item);
            }

            navState = NavigationState.Effect;
            image.ApplyEffect();
            effectPanel.SetBusyStaus(true);
        }

        private void image_EffectCompleted(object sender, EventArgs e)
        {
            if (isEffectSaving)
            {
                ShowMainProgress(false);
                App.Current.CurrentBitmap = image.OriginalImage;
                GoBackMenu();
                effectPanel.IsEnabled = true;
                isEffectSaving = false;
                progress.Visibility = System.Windows.Visibility.Collapsed;
                progress.IsIndeterminate = false;
                // in case was using line select
                lineSelect.Visibility = System.Windows.Visibility.Collapsed;
            }
            effectPanel.SetBusyStaus(image.IsBusy);
        }

        private void effectPanel_ValueChanged(object sender, EffectPages.EffectControlPanelChangedEventArgs e)
        {
            // load new parameters
            var param = effect.Parameters.FirstOrDefault(p => p.Name == e.Name);
            if (param != null)
            {
                switch (param.Type)
                {
                    case EffectParamType.Range:
                        ((RangeParam)param).Value = (float)e.Value;
                        break;
                    case EffectParamType.List:
                        ((ListParam)param).Selected = (string)e.Value;
                        break;
                    case EffectParamType.ButtonList:
                        ((ButtonListParam)param).Selected = (string)e.Value;
                        break;
                    case EffectParamType.LineSelect:
                        ((LineSelectParam)param).Offset = (float)e.Value;
                        break;
                }
            }

            // apply effect
            image.ApplyEffect();
            effectPanel.SetBusyStaus(image.IsBusy);
        }

        private void EffectOkButton_Click(object sender, RoutedEventArgs e)
        {
            isEffectSaving = true;
            effectPanel.IsEnabled = false;
            progress.Visibility = System.Windows.Visibility.Visible;
            progress.IsIndeterminate = true;
            image.SaveEffect();
        }

        private void EffectCancelButton_Click(object sender, RoutedEventArgs e)
        {
            CancelEffect();
            GoBackMenu();
        }

        #endregion

        #region * In Page Effects - CROP *

        private void DoCrop()
        {
            navState = NavigationState.Effect;
            rectangleSelect.Show();
            cropStack.Show();

            SetSelectSize();
        }

        private void SetSelectSize()
        {
            if (mainImage == null || mainImage.ActualWidth == 0.0) return;
            rectangleSelect.SetSize(mainImage.ActualWidth, mainImage.ActualHeight, WorkArea.ActualWidth, WorkArea.ActualHeight);
        }

        private void cropPanel_ValueChanged(object sender, RoutedEventArgs e)
        {
            rectangleSelect.ShowGrid = cropPanel.ShowGrid;
            rectangleSelect.AspectRatio = cropPanel.RectSelectAspect;
            rectangleSelect.RotateAspect = cropPanel.SwapWidthHeight;
            SetSelectSize();
        }

        private void CropOkCancel_Okay(object sender, RoutedEventArgs e)
        {
            var orig = App.Current.CurrentBitmap;
            double scalex = orig.PixelWidth / mainImage.ActualWidth;
            double scaley = orig.PixelHeight / mainImage.ActualHeight;

            // pre-round (crop converts to int) so we can check bounds
            Rect rect = rectangleSelect.GetSelection();
            rect.X = Math.Round(rect.X * scalex);
            rect.Width = Math.Round(rect.Width * scalex);
            rect.Y = Math.Round(rect.Y * scaley);
            rect.Height = Math.Round(rect.Height * scaley);

            // left bound
            if (rect.X < 0)
            {
                rect.X = 0;
            }
            // top bound
            if (rect.Y < 0)
            {
                rect.Y = 0;
            }
            // right bound
            if (rect.Right > orig.PixelWidth)
            {
                rect.Width = orig.PixelWidth - rect.X - 1;
            }
            // bottom bound
            if (rect.Bottom > orig.PixelHeight)
            {
                rect.Height = orig.PixelHeight - rect.Y - 1;
            }
            // save
            App.Current.CurrentBitmap = orig.Crop(rect);
            mainImage.Source = App.Current.CurrentBitmap;
            GoBackFromCrop();
        }

        private void CropOkCancel_Cancel(object sender, RoutedEventArgs e)
        {
            GoBackFromCrop();
        }

        private void GoBackFromCrop()
        {
            rectangleSelect.Hide();
            cropStack.Hide();
            GoBackMenu();
        }

        #endregion

        #region * In Page Effects - RESIZE *

        private void DoResize()
        {
            image = null;
            tempImage = App.Current.CurrentBitmap.Clone();
            mainImage.Source = tempImage;

            navState = NavigationState.Effect;
            resizeStack.Show();
            resizePanel.SetSize(tempImage.PixelWidth, tempImage.PixelHeight);
        }

        private void ResizeOkCancel_Okay(object sender, RoutedEventArgs e)
        {
            var orig = App.Current.CurrentBitmap;

            int width = 0;
            int height = 0;
            if (!int.TryParse(resizePanel.widthTxt.Text, out width)) return;
            if (!int.TryParse(resizePanel.heightTxt.Text, out height)) return;

            App.Current.CurrentBitmap = orig.Resize(width, height);
            mainImage.Source = App.Current.CurrentBitmap;

            GoBackFromResize();
        }

        private void ResizeOkCancel_Cancel(object sender, RoutedEventArgs e)
        {
            tempImage = null;
            mainImage.Source = App.Current.CurrentBitmap;
            GoBackFromResize();
        }

        private void resizePanel_Resize(object sender, ResizeControlPanel.ResizeEventArgs e)
        {
            tempImage = App.Current.CurrentBitmap.Resize((int)e.NewSize.Width, (int)e.NewSize.Height);
            mainImage.Source = tempImage;
        }

        private void GoBackFromResize()
        {
            resizeStack.Hide();
            GoBackMenu();
        }

        #endregion

        #region * In Page Effects - ROTATE *

        private void DoRotate()
        {
            navState = NavigationState.Effect;
            rotateStack.Show();
            // Clear previous effect
            image = null;
            tempImage = App.Current.CurrentBitmap.Clone();
        }

        private void RotateOkCancel_Okay(object sender, RoutedEventArgs e)
        {
            App.Current.CurrentBitmap = tempImage.Clone();
            tempImage = null;
            mainImage.Source = App.Current.CurrentBitmap;
            GoBackFromRotate();
        }

        private void RotateOkCancel_Cancel(object sender, RoutedEventArgs e)
        {
            tempImage = null;
            GoBackFromRotate();
        }

        private void rotatePanel_Rotate(object sender, RotateControlPanel.RotateEventArgs e)
        {
            switch (e.Operation)
            {
                case RotateControlPanel.RotateOperation.Horizontal:
                    tempImage = tempImage.FlipHorizontal();
                    break;
                case RotateControlPanel.RotateOperation.Vertical:
                    tempImage = tempImage.FlipVertical();
                    break;
                case RotateControlPanel.RotateOperation.Left:
                    tempImage = tempImage.Rotate270();
                    break;
                case RotateControlPanel.RotateOperation.Right:
                    tempImage = tempImage.Rotate90();
                    break;
            }
            mainImage.Source = tempImage;
        }

        private void GoBackFromRotate()
        {
            rotateStack.Hide();
            GoBackMenu();
        }

        #endregion

        #region * In Page Effects - REVERT *

        private void DoRevert()
        {
            navState = NavigationState.Effect;
            MessageBoxResult result = MessageBox.Show(AppResources.RevertMessageMessage, AppResources.RevertMessageTitle, MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                App.Current.CurrentBitmap = AppSettings.Instance.GetOriginalImage();
                Dispatcher.BeginInvoke(() =>
                {
                    mainImage.Source = App.Current.CurrentBitmap;
                });
            }
            GoBackMenu();
        }

        #endregion

        #region * Other Control Events *

        private void WorkArea_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (rectangleSelect.Visibility == System.Windows.Visibility.Visible)
            {
                SetSelectSize();
            }
        }

        #endregion

        private void GoBackMenu()
        {
            if (navState == NavigationState.Root)
            {
                return;
            }
            if (navState == NavigationState.Effect)
            {
                ShowTools();
            }
            else
            {
                ShowRoot();
            }
        }

        private void ShowRoot()
        {
            BottomTools.Show();
            EditTools.Hide();
            AdjustTools.Hide();
            EffectTools.Hide();
            FilterTools.Hide();
            navState = NavigationState.Root;
        }

        private void ShowTools()
        {
            effectStack.Hide();
            TopTools.Show();
            lastTool.Show();
            navState = NavigationState.Tools;
        }

        private void ShowMainProgress(bool show)
        {
            if (show)
            {
                progress.Visibility = System.Windows.Visibility.Visible;
                progress.IsIndeterminate = true;
            }
            else
            {
                progress.Visibility = System.Windows.Visibility.Collapsed;
                progress.IsIndeterminate = false;
            }
        }

        private bool CheckForLoadedImage()
        {
            if (App.Current.CurrentBitmap == null)
            {
                MessageBox.Show(AppResources.NoImageOpenMessage, AppResources.NoImageOpenTitle, MessageBoxButton.OK);
                return false;
            }
            return true;
        }

        private void CancelEffect()
        {
            mainImage.Source = image.OriginalImage;
            // in case was using line select
            lineSelect.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void DisableBottomTools()
        {
            foreach (var control in BottomTools.Children)
            {
                if (control.GetType() == typeof(IconButton))
                {
                    var btn = control as IconButton;
                    btn.IsEnabled = false;
                }
            }
        }

        private void EnableBottomTools()
        {
            foreach (var control in BottomTools.Children)
            {
                if (control.GetType() == typeof(IconButton))
                {
                    var btn = control as IconButton;
                    btn.IsEnabled = true;
                }
            }
        }
      
    }
}