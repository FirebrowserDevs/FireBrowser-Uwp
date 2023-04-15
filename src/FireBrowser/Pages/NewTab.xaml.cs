using FireBrowser.Core;
using FireBrowser.ViewModel;
using System;
using System.Net.Http;
using System.Text.Json;
using Windows.ApplicationModel.Resources;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using static FireBrowser.MainPage;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FireBrowser.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NewTab : Page
    {

        bool isAuto;
        public HomeViewModel ViewModel { get; set; }
        public NewTab()
        {
            this.InitializeComponent();
            HomeSync();
        }

        public void HomeSync()
        {
            var ison = FireBrowserInterop.SettingsHelper.GetSetting("Auto");
            isAuto = ison.Equals("1");
            Type.IsOn = isAuto;

            var set = FireBrowserInterop.SettingsHelper.GetSetting("Background");
            ViewModel = new HomeViewModel
            {
                BackgroundType = set switch
                {
                    "1" => Core.Settings.NewTabBackground.Featured,
                    _ => Core.Settings.NewTabBackground.None,
                },
            };
            GridSelect.SelectedValue = ViewModel.BackgroundType.ToString();

            var isLightMode = FireBrowserInterop.SettingsHelper.GetSetting("LightMode").Equals("1");
            Edit.Visibility = isLightMode ? Visibility.Collapsed : Visibility.Visible;
            SetTab.Visibility = isLightMode ? Visibility.Collapsed : Visibility.Visible;
            BigGrid.Visibility = isLightMode ? Visibility.Collapsed : Visibility.Visible;
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
        }

        private class ImageRoot
        {
            public ImageTab[] images { get; set; }
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
                            client.Dispose();
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


        private void ActionClicked(object sender, RoutedEventArgs e)
        {
            switch ((sender as Button)?.Tag)
            {
                case "Google":

                    break;
                case "Youtube":

                    break;
            }
        }

        private void BackgroundGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

        private void NewTabSearchBox_PreviewKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (isAuto)
            {

            }
            else if (e.Key is Windows.System.VirtualKey.Enter)
            {
                MainPage.FocusUrlBox(NewTabSearchBox.Text);
            }
        }

        private void Type_Toggled(object sender, RoutedEventArgs e)
        {
            ToggleSwitch toggleSwitch = sender as ToggleSwitch;
            if (toggleSwitch != null)
            {
                if (toggleSwitch.IsOn)
                {
                    isAuto = true;
                    FireBrowserInterop.SettingsHelper.SetSetting("Auto", "1");
                }
                else
                {
                    isAuto = false;
                    FireBrowserInterop.SettingsHelper.SetSetting("Auto", "0");
                }
            }
        }

        private void NewTabSearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (isAuto)
            {
                MainPage.FocusUrlBox(NewTabSearchBox.Text);
            }
            else
            {

            }
        }
    }
}
