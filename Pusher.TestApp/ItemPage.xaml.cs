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
    public partial class ItemPage : PhoneApplicationPage
    {
        public ItemPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
           // string category = "", name = "";
           // NavigationContext.QueryString.TryGetValue("category", out category);
           // NavigationContext.QueryString.TryGetValue("name", out name);       

           //Pusher.KhanApi.App.ViewModel.GetVideo(category, name, vid => DataContext = vid);

           DataContext = App.SelectedVideoItem;
        }

        

        private void PlayVideo(object sender, System.Windows.RoutedEventArgs e)
        {
            var video = LayoutRoot.DataContext as VideoItem;
            video.Navigate();
            
        }
    }
}