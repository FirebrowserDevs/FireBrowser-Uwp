using FireBrowser.Core;
using FireBrowserCore.Models;
using FireBrowserCore.ViewModel;
using System;
using System.Net.Http;
using System.Text.Json;
using Windows.ApplicationModel.Resources;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using static FireBrowser.MainPage;

namespace FireBrowser.Pages
{
    public sealed partial class NewTab : Page
    {

        bool isAuto;
        bool isMode;
        public HomeViewModel ViewModel { get; set; }
        public NewTab()
        {
            this.InitializeComponent();
            HomeSync();
        }

        public void HomeSync()
        {
            isAuto = FireBrowserInterop.SettingsHelper.GetSetting("Auto") == "1";
            Type.IsOn = isAuto;

            isMode = FireBrowserInterop.SettingsHelper.GetSetting("LightMode") == "1";
            Mode.IsOn = isMode;

            var set = FireBrowserInterop.SettingsHelper.GetSetting("Background");
            var cls = FireBrowserInterop.SettingsHelper.GetSetting("ColorBackground");

            // ViewModel setup
            ViewModel = new HomeViewModel
            {
                BackgroundType = set switch
                {
                    "2" => Settings.NewTabBackground.Costum,
                    "1" => Settings.NewTabBackground.Featured,
                    _ => Settings.NewTabBackground.None,
                }
            };

            NewColor.IsEnabled = set == "2";

            GridSelect.SelectedValue = ViewModel.BackgroundType.ToString();

            //NewColor.Text = cls;

            // Visibility setup based on LightMode setting
            Edit.Visibility = isMode ? Visibility.Collapsed : Visibility.Visible;
            SetTab.Visibility = isMode ? Visibility.Collapsed : Visibility.Visible;
            BigGrid.Visibility = isMode ? Visibility.Collapsed : Visibility.Visible;
        }


        public void sidesync()
        {
            GridSelect.SelectedValue = ViewModel.BackgroundType.ToString();
        }

        Passer param;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            param = e.Parameter as Passer;
            ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView();
        }

        private class ImageRoot
        {
            public ImageTab[] images { get; set; }
        }


        public static Brush GetGridBackgroundAsync(Settings.NewTabBackground backgroundType)
        {
            string colorString = FireBrowserInterop.SettingsHelper.GetSetting("ColorBackground");
            switch (backgroundType)
            {
                case Settings.NewTabBackground.None:
                    return new SolidColorBrush(Colors.Transparent);

                case Settings.NewTabBackground.Costum:

                    if (colorString == "")
                    {
                        return new SolidColorBrush(Colors.Transparent);
                    }
                    else
                    {
                        var color = (Windows.UI.Color)XamlBindingHelper.ConvertValue(typeof(Windows.UI.Color), colorString);

                        return new SolidColorBrush(color);
                    }

                case Settings.NewTabBackground.Featured:

                    var client = new HttpClient();
                    try
                    {
                        var request = client.GetStringAsync(new Uri("http://www.bing.com/HPImageArchive.aspx?format=js&idx=0&n=1")).Result;
                        try
                        {
                            var images = JsonSerializer.Deserialize<ImageRoot>(request);


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
                            return null;
                        }
                    }
                    catch
                    {
                        return new SolidColorBrush(Colors.Transparent);
                    }

            }
            return new SolidColorBrush();
        }

        private void BackgroundGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selection = (sender as GridView).SelectedItem as GridViewItem;

            switch (selection.Tag)
            {
                case "None":
                    FireBrowserInterop.SettingsHelper.SetSetting("Background", "0");
                    ViewModel.BackgroundType = Settings.NewTabBackground.None;
                    NewColor.IsEnabled = false;
                    break;
                case "Featured":
                    FireBrowserInterop.SettingsHelper.SetSetting("Background", "1");
                    ViewModel.BackgroundType = Settings.NewTabBackground.Featured;
                    NewColor.IsEnabled = false;
                    break;
                case "Custom":
                    FireBrowserInterop.SettingsHelper.SetSetting("Background", "2");
                    ViewModel.BackgroundType = Settings.NewTabBackground.Costum;
                    NewColor.IsEnabled = true;
                    break;
                default:

                    break;
            }
        }

        private void NewTabSearchBox_PreviewKeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (!isAuto && e.Key is Windows.System.VirtualKey.Enter)
            {
                UseContent.MainPageContent.FocusUrlBox(NewTabSearchBox.Text);
            }
        }

        private void Type_Toggled(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                isAuto = toggleSwitch.IsOn;
                string autoValue = isAuto ? "1" : "0";
                FireBrowserInterop.SettingsHelper.SetSetting("Auto", autoValue);
            }
        }

        private void NewTabSearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (isAuto)
            {
                UseContent.MainPageContent.FocusUrlBox(NewTabSearchBox.Text);
            }
        }

        private void NewColor_TextChanged(object sender, TextChangedEventArgs e)
        {
            FireBrowserInterop.SettingsHelper.SetSetting("ColorBackground", $"{NewColor.Text.ToString()}");
        }

        private void Mode_Toggled(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                string lightModeValue = toggleSwitch.IsOn ? "1" : "0";
                FireBrowserInterop.SettingsHelper.SetSetting("LightMode", lightModeValue);
            }
        }

      
    }
}
