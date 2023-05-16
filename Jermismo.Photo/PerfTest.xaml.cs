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
using Microsoft.Phone.Controls;

using System.Diagnostics;
using System.Windows.Media.Imaging;
using Jermismo.Photo.Imaging;

using Microsoft.Xna.Framework;

namespace Jermismo.Photo
{
    public partial class PerfTest : PhoneApplicationPage
    {

        WriteableBitmap image;

        public PerfTest()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            // get image
            image = App.Current.CurrentBitmap.Crop(new Rect(0, 0, 600, 600));
        }

        private void ClearResults()
        {
            ResultsPanel.Children.Clear();
        }

        private void WriteResult(string testName, string result)
        {
            TextBlock block = new TextBlock();
            block.Text = string.Format("{0}: {1}", testName, result);
            ResultsPanel.Children.Add(block);
        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            RunTests();
        }

        private void RunTests()
        {
            ClearResults();

            GC.Collect();
            MathTest();
        }

        private void MathTest()
        {
            
        }

    }

}