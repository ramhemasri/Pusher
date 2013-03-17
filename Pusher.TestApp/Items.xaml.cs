using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Pusher.KhanApi;

namespace Pusher.TestApp
{
    public partial class Items : PhoneApplicationPage
    {
        public VideoItem VideoItem { get; set; }

        public Items()
        {
            InitializeComponent();
        }

        // When page is navigated to set data context to selected item in list
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string selectedIndex = "";
            if (NavigationContext.QueryString.TryGetValue("name", out selectedIndex))
            {
                var category = Pusher.KhanApi.App.ViewModel.GetCategory(selectedIndex);
                category.LoadVideos();
                LayoutRoot.DataContext = category;
            }
        }

        private void MainListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // If selected index is -1 (no selection) do nothing
            if (MainListBox.SelectedIndex == -1)
                return;
            this.VideoItem = MainListBox.SelectedItem as VideoItem;
            App.SelectedVideoItem = this.VideoItem;
            // Navigate to the new page
            NavigationService.Navigate(new Uri(string.Format("/ItemPage.xaml?name={0}&category={1}", this.VideoItem.Name,this.VideoItem.Parent), UriKind.Relative));

           

            // Reset selected index to -1 (no selection)
            MainListBox.SelectedIndex = -1;
        }
    }
}