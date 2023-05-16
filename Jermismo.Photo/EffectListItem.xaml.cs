using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

using n = System.Windows.Navigation;

namespace Jermismo.Photo
{
    public partial class EffectListItem : UserControl
    {

        public static readonly DependencyProperty TitleProperty = 
            DependencyProperty.Register("Title", typeof(string), typeof(EffectListItem), 
            new PropertyMetadata("Effect Name", TitleProperty_Changed));
        /// <summary>
        /// The Title for the Effect
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(EffectListItem),
            new PropertyMetadata("Effect Description", DescriptionProperty_Changed));
        /// <summary>
        /// The Description for the Effect
        /// </summary>
        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        /// <summary>
        /// The Icon for the Effect
        /// </summary>
        public ImageSource Icon
        {
            get { return EffectIconImage.Source; }
            set { EffectIconImage.Source = value; }
        }

        /// <summary>
        /// The name of the effect class to use
        /// </summary>
        public string EffectName { get; set; }

        public event RoutedEventHandler Click;

        public EffectListItem()
        {
            InitializeComponent();
            this.Tap += new EventHandler<GestureEventArgs>(EffectListItem_Tap);
            this.ManipulationStarted += new EventHandler<ManipulationStartedEventArgs>(EffectListItem_ManipulationStarted);
            this.ManipulationCompleted += new EventHandler<ManipulationCompletedEventArgs>(EffectListItem_ManipulationCompleted);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            // Only do this when not in design-mode
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(Application.Current.RootVisual))
            {
                // get display settings (updating here so that they always pick up changes)
                bool icon = AppSettings.Instance.ListItemImageVisible;
                bool desc = AppSettings.Instance.ListItemDescVisible;
                EffectIconImage.Visibility = icon.ToVisibility();
                EffectDesciptionText.Visibility = desc.ToVisibility();

                switch (AppSettings.Instance.ListItemColor)
                {
                    case ColorOptions.PhoneAccent:
                        this.LayoutRoot.Background = (SolidColorBrush)Application.Current.Resources["PhoneAccentBrush"];
                        break;
                    case ColorOptions.Transparent:
                        this.LayoutRoot.Background = (SolidColorBrush)Application.Current.Resources["TransparentBrush"];
                        break;
                    default:
                        this.LayoutRoot.Background = (SolidColorBrush)Application.Current.Resources["PhoneChromeBrush"];
                        break;
                }

                if (!icon && !desc)
                {
                    EffectNameText.FontSize = 32;
                    EffectNameText.Margin = new Thickness(0, 10, 0, 0);
                }
                else
                {
                    EffectNameText.FontSize = 20;
                    EffectNameText.Margin = new Thickness(0);
                }
            }
            
            return base.MeasureOverride(availableSize);
        }

        void EffectListItem_Tap(object sender, GestureEventArgs e)
        {
            OnClick();
        }

        void EffectListItem_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            scaleXAni.To = 0.95;
            scaleYAni.To = 0.95;
            storyboard.Begin();
        }

        private void EffectListItem_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            storyboard.Stop();
            scaleXAni.To = 1;
            scaleYAni.To = 1;
            storyboard.Begin();
        }

        private void OnClick()
        {
            if (Click != null)
            {
                Click(this, new RoutedEventArgs());
            }
        }

        private void OnTitleChanged(string newValue)
        {
            EffectNameText.Text = newValue;
        }

        private void OnDescriptionChanged(string newValue)
        {
            EffectDesciptionText.Text = newValue;
        }

        private static void TitleProperty_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((EffectListItem)d).OnTitleChanged((string)e.NewValue);
        }

        private static void DescriptionProperty_Changed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((EffectListItem)d).OnDescriptionChanged((string)e.NewValue);
        }

    }
}
