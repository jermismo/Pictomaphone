using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Tasks;
using Microsoft.Xna.Framework.Media;

using Jermismo.Photo.Resources;

namespace Jermismo.Photo
{
    public partial class MainPan : PhoneApplicationPage
    {

        private PanoramaItem lastSelectedPanItem = null;
        private bool firstLoad = true;

        public MainPan()
        {
            InitializeComponent();
        }

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
            }

            if (App.Current.CurrentBitmap != null)
            {
                mainImage.Source = App.Current.CurrentBitmap;
                if (lastSelectedPanItem != null)
                    mainPanorama.DefaultItem = lastSelectedPanItem;
                else
                    mainPanorama.DefaultItem = ImagePanItem;
            }
            else
            { 
                // move to the file menu
                mainPanorama.DefaultItem = FilePanItem;
            }
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            // save the pan item they were on when they navigated away
            lastSelectedPanItem = mainPanorama.SelectedItem as PanoramaItem;
            base.OnNavigatingFrom(e);
        }

        #region ** Tool Bar Buttons **

        private void ViewButton_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckForLoadedImage()) return;
            this.NavigationService.Navigate(new Uri("/EffectPages/ViewPage.xaml", UriKind.Relative));
        }

        #endregion

        #region ** File Menu Click Handlers **

        private void TakePhoto_Click(object sender, RoutedEventArgs e)
        {
            CameraCaptureTask cameraCaptureClass = new CameraCaptureTask();
            cameraCaptureClass.Completed += new EventHandler<PhotoResult>(GenericImageTask_Completed);
            cameraCaptureClass.Show();
        }

        private void OpenPhoto_Click(object sender, RoutedEventArgs e)
        {
            PhotoChooserTask task = new PhotoChooserTask();
            task.Completed += new EventHandler<PhotoResult>(GenericImageTask_Completed);
            task.Show();

            //NavigationService.Navigate(new Uri("/BrowsePhotosPage.xaml", UriKind.Relative));
        }

        private void SavePhoto_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckForLoadedImage()) return;
            NavigationService.Navigate(new Uri("/EffectPages/SavePage.xaml", UriKind.Relative));
        }

        // runs after an image has been selected from either the camera capture or photo chooser tasks
        private void GenericImageTask_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                string name = Path.GetFileName(e.OriginalFileName);
                name =  (name.StartsWith("PhotoChooser-")) ? "[chooser]" : e.OriginalFileName;
                AppSettings.Instance.SetOriginalImage(e.ChosenPhoto, name);

                Dispatcher.BeginInvoke(() =>
                {
                    mainImage.Source = App.Current.CurrentBitmap;
                });
            }
        }

        private void RotatePhoto_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckForLoadedImage()) return;
            this.NavigationService.Navigate(new Uri("/EffectPages/RotateFlip.xaml", UriKind.Relative));
        }

        private void Revert_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show(AppResources.RevertMessageMessage, AppResources.RevertMessageTitle, MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                App.Current.CurrentBitmap = AppSettings.Instance.GetOriginalImage();
                Dispatcher.BeginInvoke(() =>
                {
                    mainImage.Source = App.Current.CurrentBitmap;
                });
            }
        }

        private void Crop_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckForLoadedImage()) return;
            NavigationService.Navigate(new Uri("/EffectPages/Crop.xaml", UriKind.Relative));
        }

        private void ViewExif_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckForLoadedImage()) return;

            if (AppSettings.Instance.ExifInfo == null)
            {
                MessageBox.Show(AppResources.MissingExifMessage, AppResources.MissingExifTitle, MessageBoxButton.OK);
                return;
            }

            NavigationService.Navigate(new Uri("/EffectPages/ExifInfo.xaml", UriKind.Relative));
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            //NavigationService.Navigate(new Uri("/About.xaml", UriKind.Relative));
            NavigationService.Navigate(new Uri("/YourLastAboutDialog;component/AboutPage.xaml", UriKind.Relative));
        }

        private void Resize_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckForLoadedImage()) return;
            NavigationService.Navigate(new Uri("/EffectPages/Resize.xaml", UriKind.Relative));
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Settings.xaml", UriKind.Relative));
        }

        #endregion

        #region ** Effect Button Click Handlers **

        private void EffectListItem_Click(object sender, RoutedEventArgs e)
        {
            if (!CheckForLoadedImage()) return;

            EffectListItem item = sender as EffectListItem;
            if (item != null)
            {
                AppSettings.Instance.SelectedEffect = item.EffectName;
                this.NavigationService.Navigate(new Uri("/EffectPages/ImageEffect.xaml", UriKind.Relative));
            }
        }

        #endregion

        private void PerfTest_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/PerfTest.xaml", UriKind.Relative));
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

    }
}