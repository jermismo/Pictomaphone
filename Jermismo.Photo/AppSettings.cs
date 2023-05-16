using System;
using System.ComponentModel;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO.IsolatedStorage;

namespace Jermismo.Photo
{

    public enum ColorOptions
    { 
        Default = 0,
        PhoneAccent = 1,
        Transparent = 2
    }

    public enum SaveLocation { SavedPhotos = 0, CameraRoll }

    public class AppSettings: System.ComponentModel.INotifyPropertyChanged
    {

        /// <summary>
        /// The max preview size in an effect page (width or height)
        /// </summary>
        public const int MaxEditImageSize = 480;

        /// <summary>
        /// Singleton instance - so Binding works.
        /// </summary>
        public static AppSettings Instance = new AppSettings();

        // file names
        private const string originalImageName = "Original.jpg";

        // setting names
        private const string listItemImageVisibleSetting = "ListItemImageVisible";
        private const string listItemDescVisibleSetting = "ListItemDescVisible";
        private const string listItemColorSetting = "ListItemColor";
        private const string saveQualitySetting = "SaveQuality";
        private const string saveLocationSetting = "SaveLocation";
        private const string savedPhotoCountSetting = "SavedPhotoCount";
        private const string selectedEffectSetting = "SelectedEffect";
        private const string savedPhotoExifSetting = "SavedPhotoExif";
        private const string lastTrialMessageDate = "LastTrialMessageDate";
        private const string originalFileName = "OriginalFileName";

        public event PropertyChangedEventHandler PropertyChanged;

        private AppSettings()
        { 
            // whoo, empty private contructor!
        }

        /// <summary>
        /// Whether or not to show icons for the effect list items
        /// </summary>
        public bool ListItemImageVisible
        {
            get 
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(listItemImageVisibleSetting))
                {
                    return (bool)IsolatedStorageSettings.ApplicationSettings[listItemImageVisibleSetting];
                }
                else
                {
                    return true;
                }
            }
            set 
            {
                IsolatedStorageSettings.ApplicationSettings[listItemImageVisibleSetting] = value;
                HandlePropertyChanged("ListItemImageVisible");
            }
        }

        /// <summary>
        /// Whether or not to show descriptions for the effect list items
        /// </summary>
        public bool ListItemDescVisible
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(listItemDescVisibleSetting))
                {
                    return (bool)IsolatedStorageSettings.ApplicationSettings[listItemDescVisibleSetting];
                }
                else
                {
                    return true;
                }
            }
            set
            {
                IsolatedStorageSettings.ApplicationSettings[listItemDescVisibleSetting] = value;
                HandlePropertyChanged("ListItemDescVisible");
            }
        }

        /// <summary>
        /// The background color for effect list items
        /// </summary>
        public ColorOptions ListItemColor
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(listItemColorSetting))
                {
                    return (ColorOptions)IsolatedStorageSettings.ApplicationSettings[listItemColorSetting];
                }
                else 
                {
                    return ColorOptions.Default;
                }
            }
            set
            {
                IsolatedStorageSettings.ApplicationSettings[listItemColorSetting] = value;
                HandlePropertyChanged("ListItemColor");
            }
        }

        /// <summary>
        /// The default quality level to save images with
        /// </summary>
        public int SaveQuality
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(saveQualitySetting))
                {
                    return (int)IsolatedStorageSettings.ApplicationSettings[saveQualitySetting];
                }
                else
                {
                    return 95;
                }
            }
            set
            {
                IsolatedStorageSettings.ApplicationSettings[saveQualitySetting] = value;
                HandlePropertyChanged("SaveQuality");
            }
        }

        /// <summary>
        /// The default location that photos are saved to
        /// </summary>
        public Photo.SaveLocation SaveLocation
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(saveLocationSetting))
                {
                    return (SaveLocation)IsolatedStorageSettings.ApplicationSettings[saveLocationSetting];
                }
                else
                {
                    return Photo.SaveLocation.SavedPhotos;
                }
            }
            set
            {
                IsolatedStorageSettings.ApplicationSettings[saveLocationSetting] = value;
                HandlePropertyChanged("SaveLocation");
            }
        }

        /// <summary>
        /// Number of photos that have been saved.
        /// </summary>
        public int SavedPhotoCount
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(savedPhotoCountSetting))
                {
                    return (int)IsolatedStorageSettings.ApplicationSettings[savedPhotoCountSetting];
                }
                else
                {
                    return 1;
                }
            }
            set
            {
                IsolatedStorageSettings.ApplicationSettings[savedPhotoCountSetting] = value;
                HandlePropertyChanged("SavedPhotoCount");
            }
        }

        /// <summary>
        /// The current effect that is selected.
        /// </summary>
        public string SelectedEffect
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(selectedEffectSetting))
                {
                    return (string)IsolatedStorageSettings.ApplicationSettings[selectedEffectSetting];
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                IsolatedStorageSettings.ApplicationSettings[selectedEffectSetting] = value;
                HandlePropertyChanged("SelectedEffect");
            }
        }

        /// <summary>
        /// Returns the original name of the user opened image.
        /// </summary>
        public string OriginalFileName
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(originalFileName))
                {
                    return (string)IsolatedStorageSettings.ApplicationSettings[originalFileName];
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// Last last time the Trial Message was shown to the user.
        /// </summary>
        public DateTime LastTrialMessageDate
        {
            get
            {
                if (IsolatedStorageSettings.ApplicationSettings.Contains(lastTrialMessageDate))
                {
                    return (DateTime)IsolatedStorageSettings.ApplicationSettings[lastTrialMessageDate];
                }
                else
                {
                    return DateTime.MinValue;
                }
            }
            set
            {
                IsolatedStorageSettings.ApplicationSettings[lastTrialMessageDate] = value;
                HandlePropertyChanged("LastTrialMessageDate");
            }
        }

        /// <summary>
        /// Gets the image that the user originally opened
        /// (and loads exif data)
        /// </summary>
        public WriteableBitmap GetOriginalImage()
        {
            using (var file = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (file.FileExists(originalImageName))
                {
                    BitmapImage bitmap = new BitmapImage();
                    using (var image = file.OpenFile(originalImageName, System.IO.FileMode.Open))
                    {
                        bitmap.SetSource(image);
                        // try read exif
                        try { this.ExifInfo = ExifPlus.ExifReader.ReadJpeg(image); }
                        catch { /* throw away */ }
                    }
                    return new WriteableBitmap(bitmap);
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Saves the originally opened image stream to isolated storage
        /// (and loads exif data)
        /// </summary>
        /// <param name="stream">The image stream</param>
        public void SetOriginalImage(System.IO.Stream stream, string fileName)
        {
            // first get exif info
            try 
            { 
                this.ExifInfo = ExifPlus.ExifReader.ReadJpeg(stream);
                if (!this.ExifInfo.FileName.HasValue) this.ExifInfo.FileName.Value = fileName;
            }
            catch { /* throw away */ }
            // now save a copy of the original
            using (var file = IsolatedStorageFile.GetUserStoreForApplication())
            {
                // delete old copy if it exists
                if (file.FileExists(originalImageName))
                {
                    file.DeleteFile(originalImageName);
                }
                // save
                using (var fileStream = file.CreateFile(originalImageName))
                {
                    // reset the position, just in case it was read before being sent in
                    stream.Position = 0;
                    stream.CopyTo(fileStream);
                }
            }

            // reset position again
            stream.Position = 0;

            // set current writeable bitmap
            BitmapImage bitmap = new BitmapImage();
            bitmap.SetSource(stream);
            App.Current.CurrentBitmap = new WriteableBitmap(bitmap);
            // save original file name
            IsolatedStorageSettings.ApplicationSettings[originalFileName] = fileName;
            IsolatedStorageSettings.ApplicationSettings.Save();
        }

        /// <summary>
        /// The original image's Exif info
        /// </summary>
        public ExifPlus.ExifInfo ExifInfo { get; set; }

        private void HandlePropertyChanged(string propertyName)
        {
            // save each time something is set
            // this makes sure a setting is never missed (such as during a crash or something)
            IsolatedStorageSettings.ApplicationSettings.Save();
            // call property changed event
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
