using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Jermismo.Photo.Controls;

namespace Jermismo.Photo
{
    public partial class Settings : PhoneApplicationPage
    {

        public Settings()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            BindDisplaySettings();
        }

        protected void BindDisplaySettings()
        {
            // show icons
            Binding iconBinding = new Binding("ListItemImageVisible") { Mode = BindingMode.TwoWay, Source = AppSettings.Instance };
            ShowIconsControl.SetBinding(OnOffSetting.IsCheckedProperty, iconBinding);
            // show descriptions
            Binding descBinding = new Binding("ListItemDescVisible") { Mode = BindingMode.TwoWay, Source = AppSettings.Instance };
            ShowDescControl.SetBinding(OnOffSetting.IsCheckedProperty, descBinding);
            // background
            backgroundPicker.Loaded += new RoutedEventHandler(backgroundPicker_Loaded);
        }

        void backgroundPicker_Loaded(object sender, RoutedEventArgs e)
        {
            // need to set here because of bug with list picker
            backgroundPicker.SelectedIndex = (int)AppSettings.Instance.ListItemColor;
        }

        private void backgroundPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0) return;
            ListPickerItem item = e.AddedItems[0] as ListPickerItem;
            if (item == null) return;

            switch ((string)item.Tag)
            {
                case "accent": AppSettings.Instance.ListItemColor = ColorOptions.PhoneAccent;
                    break;
                case "transparent": AppSettings.Instance.ListItemColor = ColorOptions.Transparent;
                    break;
                default: AppSettings.Instance.ListItemColor = ColorOptions.Default;
                    break;
            }
        }

    }
}