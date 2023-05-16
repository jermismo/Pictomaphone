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
using Microsoft.Phone.Tasks;

namespace Jermismo.Photo
{
    public partial class TrialInfo : PhoneApplicationPage
    {

        MarketplaceDetailTask _marketPlaceDetailTask = new MarketplaceDetailTask();

        public TrialInfo()
        {
            InitializeComponent();
        }

        private void BuyButton_Click(object sender, RoutedEventArgs e)
        {
            _marketPlaceDetailTask.Show();
        }

        private void ContinueButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBackOrNavigate("/MainPan.xaml");
        }
    }
}