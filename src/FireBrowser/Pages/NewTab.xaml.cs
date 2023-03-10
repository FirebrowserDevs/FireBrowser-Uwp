﻿using FireBrowser.Core;
using FireBrowser.ViewModel;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Net.Http;
using Windows.ApplicationModel.Resources;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using System.Text.Json;
using Windows.UI.Xaml.Shapes;
using static FireBrowser.MainPage;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FireBrowser.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NewTab : Page
    {
       
        public HomeViewModel ViewModel { get; set; }
        public NewTab()
        {
            this.InitializeComponent();

            var set = FireBrowserInterop.SettingsHelper.GetSetting("Background");

            if (set.Equals("0"))
            {
                ViewModel = new HomeViewModel()
                {
                    BackgroundType = Core.Settings.NewTabBackground.None,
                   
                };          
            }
            else
            {
                ViewModel = new HomeViewModel()
                {
                    BackgroundType = Core.Settings.NewTabBackground.Featured,
                };
            }
        }

        Passer param;

        private MainPage MainPage
        {
            get { return (Window.Current.Content as Frame)?.Content as MainPage; }
        }
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            param = e.Parameter as Passer;
            ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView();
            param.Tab.Header = resourceLoader.GetString("NewTabHeader");
            
            //ViewModel.Background = await GetGridBackgroundAsync();
        }

        private class ImageRoot
        {
            public Image[] images { get; set; }
        }
        private class Image
        {
            public string url { get; set; }
            public string copyright { get; set; }
            public string copyrightlink { get; set; }
            public string title { get; set; }
        }
        public static Brush GetGridBackgroundAsync(Core.Settings.NewTabBackground backgroundType)
        { 
            switch (backgroundType)
            {
                case Core.Settings.NewTabBackground.None:
                    return new SolidColorBrush(Colors.Transparent);

                case Core.Settings.NewTabBackground.Featured:
                    //get the cached bg, if there is a new one then set it, show the bing attribution label.
                    var client = new HttpClient();
                    try
                    {
                        var request = client.GetStringAsync(new Uri("http://www.bing.com/HPImageArchive.aspx?format=js&idx=0&n=1")).Result;
                        try
                        {
                            //Todo: this page needs a viewmodel with visibility to show bing image credit info.
                            var images = JsonSerializer.Deserialize<ImageRoot>(request);
                            //ViewModel.ImageTitle = images.images[0].title;
                            //ViewModel.ImageCopyright = images.images[0].copyright;
                            //ViewModel.ImageCopyrightLink = images.images[0].copyrightlink;

                            BitmapImage btpImg = new()
                            {
                                UriSource = new Uri("https://bing.com" + images.images[0].url)
                            };
                            return new ImageBrush()
                            {
                                ImageSource = btpImg,
                                Stretch = Stretch.UniformToFill
                            };
                        }
                        catch (Exception ex)
                        {
                           
                            throw;
                        }
                    }
                    catch
                    {
                        return new SolidColorBrush(Colors.Transparent);
                    }
            }
            return new SolidColorBrush();
        }

       
        private async void ActionClicked(object sender, RoutedEventArgs e)
        {
            switch ((sender as Button)?.Tag)
            {
                case "Google":
                  
                    break;
                case "Youtube":
                    
                    break;
            }
        }

        private async void BackgroundGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selection = (sender as GridView).SelectedItem as GridViewItem;


            switch (selection.Tag)
            {
                case "None":
                    FireBrowserInterop.SettingsHelper.SetSetting("Background", "0");
                    ViewModel.BackgroundType = Core.Settings.NewTabBackground.None;
                    
                    break;
                case "Featured":

                    FireBrowserInterop.SettingsHelper.SetSetting("Background", "1");
                    ViewModel.BackgroundType = Core.Settings.NewTabBackground.Featured;
                    
                    break;
                default:
                  
                    break;
            }
         

        }

        private void NewTabSearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            MainPage.FocusUrlBox(NewTabSearchBox.Text);
        }

     
    }
}