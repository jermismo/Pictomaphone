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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Microsoft.Xna.Framework.Media;
using System.Threading;

namespace Jermismo.Photo
{
    public partial class BrowsePhotosPage : PhoneApplicationPage
    {

        MediaLibrary library = new MediaLibrary();
        PictureAlbum currentAlbum;

        List<Controls.IBrowseItem> itemList = new List<Controls.IBrowseItem>();
       
        public BrowsePhotosPage()
        {
            InitializeComponent();
            listList.IsFlatList = true;
            listList.Link += new EventHandler<LinkUnlinkEventArgs>(List_Link);
            TiltEffect.TiltableItems.Add(typeof(Controls.IBrowseItem));
        }

        void List_Link(object sender, LinkUnlinkEventArgs e)
        {
            var item = e.ContentPresenter.Content as Controls.IBrowseItem;
            item.LoadPicture();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            currentAlbum = library.RootPictureAlbum;
            pivot.SelectedItem = gridPivot;
            LoadImages();
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            // clear stuff when leaving the page
            currentAlbum = null;
            itemList.Clear();
            wrapList.Children.Clear();
            listList.ItemsSource = null;
            base.OnNavigatedFrom(e);
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            // handle navigating back
            if (currentAlbum != library.RootPictureAlbum)
            {
                currentAlbum = currentAlbum.Parent;
                e.Cancel = true;
                LoadImages();
            }
        }

        private void LoadImages()
        {
            itemList.Clear();
            wrapList.Children.Clear();
            listList.ItemsSource = null;
            // load
            if (pivot.SelectedIndex == 0)
            {
                LoadGrid();
            }
            else
            {
                LoadList();
            }
            
        }

        private void LoadGrid()
        {
            foreach (PictureAlbum album in currentAlbum.Albums)
            {
                if (album.Pictures.Count == 0) continue;
                Controls.BrowseGridItem item = new Controls.BrowseGridItem();
                item.SetAlbum(album);
                item.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(item_Tap);
                wrapList.Children.Add(item);
                this.Dispatcher.BeginInvoke(() => item.LoadPicture());
            }

            foreach (Picture picture in currentAlbum.Pictures)
            {
                Controls.BrowseGridItem item = new Controls.BrowseGridItem();
                item.SetPicture(picture);
                item.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(item_Tap);
                wrapList.Children.Add(item);
                this.Dispatcher.BeginInvoke(() => item.LoadPicture());
            }
        }

        private void LoadList()
        {
            foreach (PictureAlbum album in currentAlbum.Albums)
            {
                if (album.Pictures.Count == 0) continue;
                Controls.BrowseListItem item = new Controls.BrowseListItem();
                item.SetAlbum(album);
                item.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(item_Tap);
                itemList.Add(item);
            }

            foreach (Picture picture in currentAlbum.Pictures)
            {
                Controls.BrowseListItem item = new Controls.BrowseListItem();
                item.SetPicture(picture);
                item.Tap += new EventHandler<System.Windows.Input.GestureEventArgs>(item_Tap);
                itemList.Add(item);
            }

            listList.ItemsSource = itemList;
        }

        private void pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadImages();
        }

        void item_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            Controls.IBrowseItem item = sender as Controls.IBrowseItem;
            if (item == null) return;

            if (item.BrowseMode == Controls.BrowsePhotoMode.Album)
            {
                this.currentAlbum = item.Album;
                LoadImages();
            }
            else
            {
                // TODO save original title and location
                AppSettings.Instance.SetOriginalImage(item.Picture.GetImage(), item.Picture.Name);
                NavigationService.GoBackOrNavigate("/MainPan.xaml");
            }

        }

    }
}