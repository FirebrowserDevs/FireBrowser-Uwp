using FireBrowser.Core;
using System.Net.Http;
using System.Text.Json;
using Windows.ApplicationModel.Resources;
using Windows.UI;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using static FireBrowser.MainPage;
using Dots.SDK;
using FireBrowser.Core.ViewModels;
using Dots.SDK.Collections;
using Dots.SDK.CollectionItems;
using Windows.UI.Core;
//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace FireBrowser.Pages
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    public sealed partial class NewTab : Page
    {
        public static Brush GetGridBackgroundAsync(Core.Models.Settings.NewTabBackground backgroundType)
        {
            var settings = Core.Settings.currentProfile; //await Core.Settings.GetSettingsDataAsync();

            switch (backgroundType)
            {
                case Core.Models.Settings.NewTabBackground.None:
                    return new SolidColorBrush(Colors.Transparent);
                case Core.Models.Settings.NewTabBackground.Custom:
                    BitmapImage bitmapImage = new()
                    {
                        //Todo: check if empty and update app.xaml.cs with this path
                        UriSource = new Uri("ms-appdata:///local/FireBrowserData/NewTab.png")
                    };
                    return new ImageBrush()
                    {
                        ImageSource = bitmapImage,
                        Stretch = Stretch.UniformToFill
                    };
                case Core.Models.Settings.NewTabBackground.Featured:
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
                            Dots.SDK.UWP.Log.WriteLog(ex, "FeaturedImage error", Log.LogType.Error);
                            throw;
                        }
                    }
                    catch
                    {
                        return new SolidColorBrush(Colors.Transparent);
                    }
                    //If there is a newer image than cached:
            }
            return new SolidColorBrush();
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
        public Core.UserData.Site SiteType { get; set; }

        public NewTab()
        {
            this.InitializeComponent();
            if (param?.TabView.SelectedItem == param?.Tab)
            {
                //param.ViewModel.CurrentAddress = null;
            }
            ViewModel = new HomeViewModel()
            {
                ShowSearchSuggestions = Core.Settings.currentProfile.NewTab.ShowSearchSuggestions,
                BackgroundType = Core.Settings.currentProfile.NewTab.NewTabBackground,
                Pins = new()
            };
            Window.Current.SizeChanged += (s, e) =>
            {
                //To-Do: There's probably a better way to do this
                if(e.Size.Width * 0.60 < 640)
                {
                    NewTabSearchBox.Width = e.Size.Width * 0.60;
                }
                else
                {
                    NewTabSearchBox.Width = 640;
                }
            };
        }

        Passer param;
        public HomeViewModel ViewModel { get; set; }
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            param = e.Parameter as Passer;
            ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView();
            param.Tab.Header = resourceLoader.GetString("NewTabHeader");
            param.Tab.IconSource = new Microsoft.UI.Xaml.Controls.FontIconSource()
            {
                Glyph = "\uF751"
            };
            param.ViewModel.CurrentAddress = null;
            param.ViewModel.SecurityIcon = "\uF690";
            //ViewModel.Background = await GetGridBackgroundAsync();
        }
        private async void QuickLinkClicked(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var b = sender as Grid;
            var param2 = param;
            param2.Param = b.Tag;
            //To-Do: This should have a connected animation, however it crashed the app for some reason...
            if (b.Tag.ToString() == "NewTabEdit")
            {
                //To-Do
                await UI.ShowDialog("Information", "Editing pins coming soon");
            }
            else
            {
                Frame.Navigate(typeof(WebContent), param2, new DrillInNavigationTransitionInfo());
            }
        }

        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var collections = CollectionHelpers.GetCollection("AirplaneHomePins", Core.Resources.collectionPath);
            }
            catch(Exception ex)
            {
                var collection = new Collection()
                {
                    Name = "AirplaneHomePins",
                    Content = new()
                    {
                        new WebsiteItem()
                        {
                            Title = "google",
                            URL = new Uri("https://google.com")
                        }
                    }
                };
                collection.WriteCollection(Core.Resources.collectionPath, false);
            }
            foreach (var item in CollectionHelpers.GetCollection("AirplaneHomePins", Core.Resources.collectionPath).Content)
            {
                ViewModel.Pins.Add(item as WebsiteItem);
            }
        }

        private void LayoutGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private async void BackgroundGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selection = (sender as GridView).SelectedItem as GridViewItem;
            var settings = await Core.Settings.GetSettingsDataAsync();

            switch (selection.Tag)
            {
                case "None":
                    settings.NewTab.NewTabBackground = Core.Models.Settings.NewTabBackground.None;
                    break;
                case "Custom":
                    //To-Do: file picker dialog
                    break;
                case "Featured":
                    settings.NewTab.NewTabBackground = Core.Models.Settings.NewTabBackground.Featured;
                    break;
                default:
                    Dots.SDK.UWP.Log.WriteLog("Selection had no tag", "BackgroundGridView_SelectionChanged", Log.LogType.Warning);
                    break;
            }
            AppData.SaveData(settings, AppData.DataType.AppSettings);

            ViewModel.BackgroundType = settings.NewTab.NewTabBackground;
        }

        private MainPage MainPage
        {
            get { return (Window.Current.Content as Frame)?.Content as MainPage; }
        }

        private void NewTabSearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            MainPage.FocusUrlBox(NewTabSearchBox.Text);
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
