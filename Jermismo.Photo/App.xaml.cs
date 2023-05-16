using System;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Marketplace;

namespace Jermismo.Photo
{
    public partial class App : Application
    {

        private static LicenseInformation _licenseInfo = new LicenseInformation();

        /// <summary>
        /// Provides easy access to the root frame of the Phone Application.
        /// </summary>
        /// <returns>The root frame of the Phone Application.</returns>
        public PhoneApplicationFrame RootFrame { get; private set; }

        /// <summary>
        /// The current bitmap that is being worked on.
        /// </summary>
        public WriteableBitmap CurrentBitmap { get; set; }

        /// <summary>
        /// Returns the current instance
        /// </summary>
        public static new App Current { get { return Application.Current as App; } }

        /// <summary>
        /// Returns the Actual Width of the current Content Host.
        /// </summary>
        public static double ActualWidth { get { return Application.Current.Host.Content.ActualWidth; } }

        /// <summary>
        /// Returns the Actual Height of the current Content Host.
        /// </summary>
        public static double ActualHeight { get { return Application.Current.Host.Content.ActualHeight; } }

        private static bool _isTrial = true;
        /// <summary>
        /// Cached Value for if we are in trial mode.
        /// </summary>
        public bool IsTrial
        {
            get
            {
                return _isTrial;
            }
        }

        /// <summary>
        /// Constructor for the Application object.
        /// </summary>
        public App()
        {
            // Global handler for uncaught exceptions. 
            UnhandledException += Application_UnhandledException;

            // Show graphics profiling information while debugging.
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // Display the current frame rate counters.
                //Application.Current.Host.Settings.EnableFrameRateCounter = true;

                // Show the areas of the app that are being redrawn in each frame.
                //Application.Current.Host.Settings.EnableRedrawRegions = true;

                // Enable non-production analysis visualization mode, 
                // which shows areas of a page that are being GPU accelerated with a colored overlay.
                //Application.Current.Host.Settings.EnableCacheVisualization = true;

                MemoryDiagnostics.MemoryDiagnosticsHelper.Start(TimeSpan.FromSeconds(1), false);
            }

            // Standard Silverlight initialization
            InitializeComponent();

            // Phone-specific initialization
            InitializePhoneApplication();

            // fix for drawing Japanese Kanji with the correct font
            RootFrame.Language = XmlLanguage.GetLanguage(CultureInfo.CurrentUICulture.Name); 
        }

        /// <summary>
        /// Check the current license information for this application
        /// </summary>
        private void CheckLicense()
        {
            // When debugging, we want to simulate a trial mode experience. The following conditional allows us to set the _isTrial 
            // property to simulate trial mode being on or off. 
            //#if DEBUG
            //            string message = "This sample demonstrates the implementation of a trial mode in an application." +
            //                               "Press 'OK' to simulate trial mode. Press 'Cancel' to run the application in normal mode.";
            //            if (MessageBox.Show(message, "Debug Trial",
            //                 MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            //            {
            //                _isTrial = true;
            //            }
            //            else
            //            {
            //                _isTrial = false;
            //            }
            //#else
            //            _isTrial = _licenseInfo.IsTrial();
            //#endif

            // use the above code when testing trial mode stuff
            _isTrial = _licenseInfo.IsTrial();
        }

        #region Application Level Events

        // Code to execute when the application is launching (eg, from Start)
        // This code will not execute when the application is reactivated
        private void Application_Launching(object sender, LaunchingEventArgs e)
        {
            CheckLicense();
        }

        // Code to execute when the application is activated (brought to foreground)
        // This code will not execute when the application is first launched
        private void Application_Activated(object sender, ActivatedEventArgs e)
        {
            CheckLicense();
        }

        // Code to execute when the application is deactivated (sent to background)
        // This code will not execute when the application is closing
        private void Application_Deactivated(object sender, DeactivatedEventArgs e)
        {
          
        }

        // Code to execute when the application is closing (eg, user hit Back)
        // This code will not execute when the application is deactivated
        private void Application_Closing(object sender, ClosingEventArgs e)
        {

        }

        // Code to execute if a navigation fails
        private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // A navigation has failed; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
        }

        // Code to execute on Unhandled Exceptions
        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (System.Diagnostics.Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                System.Diagnostics.Debugger.Break();
            }
            else
            {

                var culture = System.Globalization.CultureInfo.CurrentUICulture;
                var errorTitle = LocalizedStrings.GetError(culture);
                var errorMsg = LocalizedStrings.GetSystemErrorMessage(culture);

                var res = System.Windows.MessageBox.Show(errorMsg, errorTitle, MessageBoxButton.OKCancel);
                if (res == MessageBoxResult.OK)
                {
                    var body = new System.Text.StringBuilder();
                    body.AppendLine("Hey Jermismo,").AppendLine();
                    body.AppendLine("Pictomaphone crashed on my device. You need to look into this, like ASAP!");
                    body.AppendLine("Here are the details.");
                    body.AppendLine();
                    body.AppendLine("MESSAGE:");
                    body.AppendLine(e.ExceptionObject.Message);
                    body.AppendLine();
                    body.AppendLine("STACK TRACE:");
                    body.AppendLine(e.ExceptionObject.StackTrace);
                    body.AppendLine();
                    
                    string modelname = null;
                    object modelobject = null;
                    if (Microsoft.Phone.Info.DeviceExtendedProperties.TryGetValue("DeviceName", out modelobject))
                        modelname = modelobject as string;
                    string manufacturerName="";
                    object manufacturerobject;
                    if (Microsoft.Phone.Info.DeviceExtendedProperties.TryGetValue("DeviceManufacturer", out manufacturerobject))
                        manufacturerName = manufacturerobject.ToString();

                    body.AppendLine("MANUFACTURER: " + manufacturerName);
                    body.AppendLine("MODEL: " + modelname);
                    body.AppendLine("CULTURE: " + culture.ToString());

                    var emailCompose = new Microsoft.Phone.Tasks.EmailComposeTask()
                    {
                        To = "pictomaphone+error@jermismo.com",
                        Subject = "Pictomaphone Error",
                        Body = body.ToString()
                    };
                    try
                    {
                        emailCompose.Show();
                    }
                    catch { /* we failed hard */ }
                }
            }
            // will this make it not crash? YES!
            e.Handled = true;
        }

        #endregion

        #region Phone application initialization

        // Avoid double-initialization
        private bool phoneApplicationInitialized = false;

        // Do not add any additional code to this method
        private void InitializePhoneApplication()
        {
            if (phoneApplicationInitialized)
                return;

            // Create the frame but don't set it as RootVisual yet; this allows the splash
            // screen to remain active until the application is ready to render.
            RootFrame = new PhoneApplicationFrame();
            RootFrame.Navigated += CompleteInitializePhoneApplication;

            // Handle navigation failures
            RootFrame.NavigationFailed += RootFrame_NavigationFailed;

            // Ensure we don't initialize again
            phoneApplicationInitialized = true;
        }

        // Do not add any additional code to this method
        private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
        {
            // Set the root visual to allow the application to render
            if (RootVisual != RootFrame)
                RootVisual = RootFrame;

            // Remove this handler since it is no longer needed
            RootFrame.Navigated -= CompleteInitializePhoneApplication;
        }

        #endregion
    }
}