﻿using CommunityToolkit.Mvvm.ComponentModel;
using FireBrowser.Controls;
using FireBrowser.Core;
using FireBrowser.Pages;
using FireBrowserFavorites;
using FireBrowserHelpers.AdBlocker;
using FireBrowserHelpers.ReadingMode;
using FireBrowserQr;
using FireBrowserUrlHelper;
using Microsoft.Data.Sqlite;
using Microsoft.UI.Xaml.Controls;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Graphics.Display;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using static FireBrowser.App;
using NewTab = FireBrowser.Pages.NewTab;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FireBrowser
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    /// 
    public class StringOrIntTemplateSelector : DataTemplateSelector
    {
        public DataTemplate StringTemplate { get; set; }
        public DataTemplate IntTemplate { get; set; }
        public DataTemplate DefaultTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item is string)
            {
                return StringTemplate;
            }
            else if (item is int)
            {
                return IntTemplate;
            }
            else
            {
                return DefaultTemplate;
            }
        }
    }


    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            ButtonVisible();

            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;

            var formattableTitleBar = ApplicationView.GetForCurrentView().TitleBar;
            formattableTitleBar.ButtonBackgroundColor = Colors.Transparent;
            formattableTitleBar.ButtonHoverBackgroundColor = Colors.Transparent;
            formattableTitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            formattableTitleBar.InactiveBackgroundColor = Colors.Transparent;
            formattableTitleBar.ButtonPressedBackgroundColor = Colors.Transparent; 


            ViewModel = new ToolbarViewModel
            {
                UserName = "", //Settings.currentProfile.AccountData.Name,
                LoadingState = new FontIcon()
                {
                    Glyph = "\uE72C",
                }
            };
            Window.Current.SetTitleBar(CustomDragRegion);
        }
        private void ResetOutput()
        {
            DisplayInformation displayInformation = DisplayInformation.GetForCurrentView();
            String scaleValue = (displayInformation.RawPixelsPerViewPixel * 100.0).ToString("F0");
        }

        #region buttons

        public static string ReadButton { get; set; }
        public static string AdblockBtn { get; set; }
        public static string Downloads { get; set; }
        public static string Translate { get; set; }
        public static string Favorites { get; set; }
        public static string Historybtn { get; set; }
        public static string QrCode { get; set; }
        public void ButtonVisible()
        {
            ReadButton = FireBrowserInterop.SettingsHelper.GetSetting("Readbutton");
            AdblockBtn = FireBrowserInterop.SettingsHelper.GetSetting("AdBtn");
            Downloads = FireBrowserInterop.SettingsHelper.GetSetting("DwBtn");
            Translate = FireBrowserInterop.SettingsHelper.GetSetting("TransBtn");
            Favorites = FireBrowserInterop.SettingsHelper.GetSetting("FavBtn");
            Historybtn = FireBrowserInterop.SettingsHelper.GetSetting("HisBtn");
            QrCode = FireBrowserInterop.SettingsHelper.GetSetting("QrBtn");

            ReadBtn.Visibility = ReadButton switch
            {
                "True" => Visibility.Visible,
                _ => Visibility.Collapsed,
            };
            AdBlock.Visibility = AdblockBtn switch
            {
                "True" => Visibility.Visible,
                _ => Visibility.Collapsed,
            };
            DownBtn.Visibility = Downloads switch
            {
                "True" => Visibility.Visible,
                _ => Visibility.Collapsed,
            };
            BtnTrans.Visibility = Translate switch
            {
                "True" => Visibility.Visible,
                _ => Visibility.Collapsed,
            };
            FavoritesButton.Visibility = Favorites switch
            {
                "True" => Visibility.Visible,
                _ => Visibility.Collapsed,
            };
            History.Visibility = Historybtn switch
            {
                "True" => Visibility.Visible,
                _ => Visibility.Collapsed,
            };
            QrBtn.Visibility = QrCode switch
            {
                "True" => Visibility.Visible,
                _ => Visibility.Collapsed,
            };
        }

        #endregion

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            ResetOutput();

            if (e.Parameter is AppLaunchPasser passer)
            {
                switch (passer.LaunchType)
                {
                    case AppLaunchType.LaunchBasic:
                        Tabs.TabItems.Add(CreateNewTab(typeof(NewTab)));
                        break;
                    case AppLaunchType.LaunchIncognito:
                        RequestedTheme = ElementTheme.Dark;
                        ViewModel.IsIncognito = true;
                        VBtn.Visibility = Visibility.Collapsed;
                        History.IsEnabled = false;
                        DownBtn.IsEnabled = false;
                        AddFav.IsEnabled = false;
                        FavoritesButton.IsEnabled = false;
                        WebContent.IsIncognitoModeEnabled = true;
                        Tabs.TabItems.Add(CreateNewIncog());
                        break;
                    case AppLaunchType.LaunchStartup:
                        Tabs.TabItems.Add(CreateNewTab(typeof(NewTab)));
                        var startup = await StartupTask.GetAsync("FireBrowserStartUp");
                        break;
                    case AppLaunchType.FirstLaunch:
                        Tabs.TabItems.Add(CreateNewTab());
                        break;
                    case AppLaunchType.URIHttp:
                        Tabs.TabItems.Add(CreateNewTab(typeof(WebContent), new Uri(passer.LaunchData.ToString())));
                        break;
                    case AppLaunchType.URIFireBrowser:
                        Tabs.TabItems.Add(CreateNewTab(typeof(NewTab)));
                        break;
                }
            }
            else
            {
                Tabs.TabItems.Add(CreateNewTab());
            }
        }



        #region toolbar
        public ToolbarViewModel ViewModel { get; set; }

        public partial class ToolbarViewModel : ObservableObject
        {
            [ObservableProperty]
            private UIElement loadingState;
            [ObservableProperty]
            private bool canRefresh;
            [ObservableProperty]
            private bool canGoBack;
            [ObservableProperty]
            private bool canGoForward;
            [ObservableProperty]
            private string favoriteIcon;
            [ObservableProperty]
            private bool permissionDialogIsOpen;
            [ObservableProperty]
            private string permissionDialogTitle;
            [ObservableProperty]
            private Microsoft.UI.Xaml.Controls.FontIconSource permissionDialogIcon;
            [ObservableProperty]
            private bool isIncognito;
            [ObservableProperty]
            private bool translatableSite;
            [ObservableProperty]
            private string currentAddress;
            [ObservableProperty]
            private string securityIcon;
            [ObservableProperty]
            private Visibility homeButtonVisibility;

            private string _userName;

            public string UserName
            {
                get
                {
                    if (_userName == "DefaultFireBrowserUser") return "DefaultFireBrowserUserName";
                    else return _userName;
                }
                set { SetProperty(ref _userName, value); }
            }
        }

        #endregion
        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            CustomDragRegion.MinWidth = (FlowDirection == FlowDirection.LeftToRight) ? sender.SystemOverlayRightInset : sender.SystemOverlayLeftInset;
            CustomDragRegion.Height = sender.Height;
        }

        public class Passer
        {
            public TabViewItem Tab { get; set; }
            public TabView TabView { get; set; }
            public object Param { get; set; }
            public ToolbarViewModel ViewModel { get; set; }

        }

        #region hidepasser
        public void HideToolbar(bool hide)
        {
            var visibility = hide ? Visibility.Collapsed : Visibility.Visible;
            var margin = hide ? new Thickness(0, -40, 0, 0) : new Thickness(0, 35, 0, 0);

            ClassicToolbar.Visibility = visibility;
            TabContent.Margin = margin;
        }

        private Passer CreatePasser(object parameter = null)
        {
            Passer passer = new()
            {
                Tab = Tabs.SelectedItem as TabViewItem,
                TabView = Tabs,
                ViewModel = ViewModel,
                Param = parameter,
            };
            return passer;
        }
        #endregion

        #region frames
        public Frame TabContent
        {
            get
            {
                CustomTabViewItem selectedItem = (FireBrowser.Controls.CustomTabViewItem)Tabs.SelectedItem;
                if (selectedItem != null)
                {
                    return (Frame)selectedItem.Content;
                }
                return null;
            }
        }

        public WebView2 TabWebView
        {
            get
            {
                if (TabContent.Content is WebContent)
                {
                    return (TabContent.Content as WebContent).WebViewElement;
                }
                return null;
            }
        }

        #endregion

        public CustomTabViewItem CreateNewIncog(Type page = null, object param = null, int index = -1)
        {
            if (index == -1) index = Tabs.TabItems.Count;

            UrlBox.Text = "";

            CustomTabViewItem newItem = new()
            {
                Header = $"Incognito",
                IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = Symbol.BlockContact }
            };

            newItem.Style = (Style)Application.Current.Resources["FloatingTabViewItemStyle"];

            // The content of the tab is often a frame that contains a page, though it could be any UIElement.
            double Margin = 0;
            Margin = ClassicToolbar.ActualHeight;
            Frame frame = new()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Margin = new Thickness(0, Margin, 0, 0)
            };

            if (page != null)
            {
                frame.Navigate(page, param);
            }
            else
            {
                frame.Navigate(typeof(Pages.Incognito));
            }

            newItem.Content = frame;
            return newItem;
        }

        public CustomTabViewItem CreateNewTab(Type page = null, object param = null, int index = -1)
        {
            if (index == -1) index = Tabs.TabItems.Count;

            UrlBox.Text = "";

            CustomTabViewItem newItem = new()
            {
                Header = $"FireBrowser HomePage",
                IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = Symbol.Home }
            };


            Passer passer = new()
            {
                Tab = newItem,
                TabView = Tabs,
                ViewModel = ViewModel,
                Param = param
            };


            newItem.Style = (Style)Application.Current.Resources["FloatingTabViewItemStyle"];

            // The content of the tab is often a frame that contains a page, though it could be any UIElement.
            double Margin = 0;
            Margin = ClassicToolbar.Height;
            Frame frame = new()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Margin = new Thickness(0, Margin, 0, 0)
            };



            if (page != null)
            {
                frame.Navigate(page, passer);
            }
            else
            {
                frame.Navigate(typeof(Pages.NewTab), passer);
            }

            string GetTitle()
            {
                if (frame.Content is WebContent)
                    return (frame.Content as WebContent)?.WebViewElement?.CoreWebView2?.DocumentTitle;
                else
                    return "No title";
            }

            ToolTip toolTip = new();
            Grid grid = new();
            Image previewImage = new();
            TextBlock textBlock = new();
            //textBlock.Text = GetTitle();
            grid.Children.Add(previewImage);
            grid.Children.Add(textBlock);
            toolTip.Content = grid;
            ToolTipService.SetToolTip(newItem, toolTip);

            newItem.Content = frame;
            return newItem;
        }

        public void FocusUrlBox(string text)
        {
            UrlBox.Text = text;
            UrlBox.Focus(FocusState.Programmatic);
        }
        public void NavigateToUrl(string uri)
        {
            if (TabContent.Content is WebContent webContent)
            {
                webContent.WebViewElement.CoreWebView2.Navigate(uri.ToString());
            }
            else
            {
                launchurl ??= uri;
                TabContent.Navigate(typeof(WebContent), CreatePasser(uri));
            }
        }


        private async void UrlBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            string input = UrlBox.Text.ToString();
            string inputtype = UrlHelper.GetInputType(input);
            if (input.Contains("firebrowser://"))
            {
                switch (input)
                {
                    case "firebrowser://newtab":
                        Tabs.TabItems.Add(CreateNewTab());
                        SelectNewTab();
                        break;
                    case "firebrowser://settings":
                    case "firebrowser://design":
                    case "firebrowser://privacy":
                    case "firebrowser://newtabset":
                    case "firebrowser://access":
                    case "firebrowser://about":
                        TabContent.Navigate(typeof(SettingsPage), CreatePasser(), new DrillInNavigationTransitionInfo());
                        break;
                    default:
                        // default behavior
                        break;
                }
            }
            else if (inputtype == "url")
            {
                NavigateToUrl(input.Trim());
            }
            else if (inputtype == "urlNOProtocol")
            {
                NavigateToUrl("https://" + input.Trim());
            }
            else
            {
                string searchurl;
                if (SearchUrl == null) searchurl = "https://www.google.nl/search?q=";
                else
                {
                    searchurl = SearchUrl;
                }
                string query = searchurl + input;
                NavigateToUrl(query);
            }
        }

        public static string launchurl { get; set; }
        public static string SearchUrl { get; set; }

        #region cangochecks
        private bool CanGoBack()
        {
            /*ViewModel.CanGoBack = (bool)(TabContent?.Content is WebContent
                ? TabWebView?.CoreWebView2.CanGoBack
                : TabContent?.CanGoBack);*/

            //return ViewModel.CanGoBack;
            return true;
        }


        private bool CanGoForward()
        {
            /*ViewModel.CanGoForward = (bool)(TabContent?.Content is WebContent
                ? TabWebView?.CoreWebView2.CanGoForward
                : TabContent?.CanGoForward);
            return ViewModel.CanGoForward;*/
            return true;
        }


        private void GoBack()
        {
            if (CanGoBack() && TabContent != null)
            {
                if (TabContent.Content is WebContent && TabWebView.CoreWebView2.CanGoBack) TabWebView.CoreWebView2.GoBack();
                else if (TabContent.CanGoBack) TabContent.GoBack();
                else ViewModel.CanGoBack = false;
            }
        }

        private void GoForward()
        {
            if (CanGoForward() && TabContent != null)
            {
                if (TabContent.Content is WebContent && TabWebView.CoreWebView2.CanGoForward) TabWebView.CoreWebView2.GoForward();
                else if (TabContent.CanGoForward) TabContent.GoForward();
                else ViewModel.CanGoForward = false;
            }
        }
        #endregion

        private async void Tabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TabContent != null)
            {
                TabContent.Navigated += async (s, e) =>
                {
                    CanGoBack(); CanGoForward();
                    if (TabContent?.Content is WebContent)
                    {
                        await TabWebView.EnsureCoreWebView2Async();
                        TabWebView.CoreWebView2.NavigationStarting += (s, e) =>
                        {
                            ViewModel.CanGoBack = CanGoBack();
                            ViewModel.CanGoForward = CanGoForward();
                        };
                    }
                };
            }

            if (TabContent?.Content is WebContent webContent)
            {
                ViewModel.CanRefresh = true;
                await TabWebView.EnsureCoreWebView2Async();
                ViewModel.CurrentAddress = TabWebView.CoreWebView2.Source.ToString();
            }
            else
            {
                ViewModel.CanRefresh = false;
                ViewModel.CurrentAddress = null;
            }
            ViewModel.FavoriteIcon = "\uF714";
        }

        public static WebContent WebContent
        {
            get { return (Window.Current.Content as Frame)?.Content as WebContent; }
        }

        public static MainPage MainPageContent
        {
            get { return (Window.Current.Content as Frame)?.Content as MainPage; }
        }

        public static SettingsPage SettingsContent
        {
            get { return (Window.Current.Content as Frame)?.Content as SettingsPage; }
        }

        private async void ToolbarButtonClick(object sender, RoutedEventArgs e)
        {
            Passer passer = new()
            {
                Tab = Tabs.SelectedItem as TabViewItem,
                TabView = Tabs,
                ViewModel = ViewModel
            };

            switch ((sender as Button).Tag)
            {
                case "Back":
                    GoBack();
                    break;
                case "Forward":
                    GoForward();
                    break;
                case "Refresh":
                    if (TabContent.Content is WebContent) TabWebView.CoreWebView2.Reload();
                    //else TabContent.Navigate(typeof(TabContent.Content));
                    break;
                case "Home":
                    if (TabContent.Content is WebContent)
                    {
                        Hmbtn.IsEnabled = true;
                        TabWebView.Close();
                        if (WebContent.IsIncognitoModeEnabled)
                        {
                            TabContent.Navigate(typeof(Pages.Incognito), passer, new DrillInNavigationTransitionInfo());
                            passer.Tab.Header = "Incognito";
                            passer.Tab.IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = Symbol.BlockContact };
                            UrlBox.Text = "";
                        }
                        else
                        {
                            TabContent.Navigate(typeof(Pages.NewTab), passer, new DrillInNavigationTransitionInfo());
                            passer.Tab.Header = "New Tab";
                            passer.Tab.IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = Symbol.Home };
                            UrlBox.Text = "";
                        }

                    }
                    Hmbtn.IsEnabled = false;
                    break;
                case "DownloadFlyout":
                    if (TabContent.Content is WebContent)
                    {
                        if (TabWebView.CoreWebView2.IsDefaultDownloadDialogOpen == true)
                        {
                            (TabContent.Content as WebContent).WebViewElement.CoreWebView2.CloseDefaultDownloadDialog();
                        }
                        else
                        {
                            (TabContent.Content as WebContent).WebViewElement.CoreWebView2.OpenDefaultDownloadDialog();
                        }
                    }
                    break;
                case "Translate":
                    if (TabContent.Content is WebContent)
                    {
                        string url = (TabContent.Content as WebContent).WebViewElement.CoreWebView2.Source.ToString();
                        (TabContent.Content as WebContent).WebViewElement.CoreWebView2.Navigate("https://translate.google.com/translate?hl&u=" + url);
                    }
                    break;
                case "QRCode":
                    try
                    {
                        if (TabContent.Content is WebContent)
                        {
                            //Create raw qr code data
                            QRCodeGenerator qrGenerator = new QRCodeGenerator();
                            QRCodeData qrCodeData = qrGenerator.CreateQrCode((TabContent.Content as WebContent).WebViewElement.CoreWebView2.Source.ToString(), QRCodeGenerator.ECCLevel.M);

                            //Create byte/raw bitmap qr code
                            BitmapByteQRCode qrCodeBmp = new BitmapByteQRCode(qrCodeData);
                            byte[] qrCodeImageBmp = qrCodeBmp.GetGraphic(20);
                            using (InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream())
                            {
                                using (DataWriter writer = new DataWriter(stream.GetOutputStreamAt(0)))
                                {
                                    writer.WriteBytes(qrCodeImageBmp);
                                    await writer.StoreAsync();
                                }
                                var image = new BitmapImage();
                                await image.SetSourceAsync(stream);

                                QRCodeImage.Source = image;
                            }
                        }
                        else
                        {
                            await UI.ShowDialog("Information", "No Webcontent Detected ( Url )");
                            QRCodeFlyout.Hide();
                        }

                    }
                    catch
                    {
                        await UI.ShowDialog("Error", "An error occurred while trying to generate your qr code");
                        QRCodeFlyout.Hide();
                    }
                    break;
                case "ReadingMode":
                    if (TabContent.Content is WebContent)
                    {
                        string jscript = await ReadabilityHelper.GetReadabilityScriptAsync();
                        await (TabContent.Content as WebContent).WebViewElement.CoreWebView2.ExecuteScriptAsync(jscript);
                    }
                    break;
                case "AdBlock":
                    if (TabContent.Content is WebContent)
                    {
                        string jscript = await AdBlockHelper.GetAdblockReadAsync();
                        await (TabContent.Content as WebContent).WebViewElement.CoreWebView2.ExecuteScriptAsync(jscript);
                        (TabContent.Content as WebContent).WebViewElement.CoreWebView2.Reload();
                    }
                    break;
                case "AddFavoriteFlyout":
                    if (TabContent.Content is WebContent)
                    {
                        FavoriteTitle.Text = TabWebView.CoreWebView2.DocumentTitle;
                        FavoriteUrl.Text = TabWebView.CoreWebView2.Source;
                    }
                    break;
                case "AddFavorite":
                    FavoritesHelper.AddFavoritesItem(FavoriteTitle.Text, FavoriteUrl.Text);
                    break;
                case "Favorites":
                    Globals.JsonItemsList = await Json.GetListFromJsonAsync("Favorites.json");
                    if (Globals.JsonItemsList is not null)
                    {
                        FavoritesListView.ItemsSource = Globals.JsonItemsList;
                    }
                    break;
            }
        }

        #region tabsbuttons
        private void Tabs_AddTabButtonClick(TabView sender, object args)
        {
            sender.TabItems.Add(CreateNewTab());
            SelectNewTab();
        }

        public void SelectNewTab()
        {
            var tabToSelect = Tabs.TabItems.Count - 1;
            Tabs.SelectedIndex = tabToSelect;
        }

        private void Tabs_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            TabViewItem selectedItem = args.Tab;
            var tabcontent = (Frame)selectedItem.Content;

            if (tabcontent.Content is WebContent)
            {
                (tabcontent.Content as WebContent).WebViewElement.Close();
            }        
            sender.TabItems.Remove(args.Tab);
            if (Tabs.TabItems.Count == 0)
            {
                CoreApplication.Exit();
            }
        }
        #endregion

       
        private void FetchBrowserHistory()
        {
            Batteries.Init();

            try
            {
                // Create a connection to the SQLite database
                var connectionStringBuilder = new SqliteConnectionStringBuilder();
                connectionStringBuilder.DataSource = Path.Combine(ApplicationData.Current.LocalFolder.Path, "History.db");

                using (var connection = new SqliteConnection(connectionStringBuilder.ConnectionString))
                {
                    // Open the database connection
                    connection.Open();

                    // Define the SQL query to fetch the browser history
                    string sql = "SELECT url, title, visit_count, last_visit_time FROM urlsDb ORDER BY last_visit_time DESC";

                    // Create a command object with the SQL query and connection
                    using (SqliteCommand command = new SqliteCommand(sql, connection))
                    {
                        // Execute the SQL query and get the results
                        using (SqliteDataReader reader = command.ExecuteReader())
                        {
                            // Create a list to store the browser history items
                            List<HistoryItem> historyItems = new List<HistoryItem>();

                            // Iterate through the query results and create a BrowserHistoryItem for each row
                            while (reader.Read())
                            {
                                HistoryItem historyItem = new HistoryItem
                                {
                                    Url = reader.GetString(0),
                                    Title = reader.IsDBNull(1) ? null : reader.GetString(1),
                                    VisitCount = reader.GetInt32(2),
                                    LastVisitTime = DateTimeOffset.FromFileTime(reader.GetInt64(3)).DateTime
                                };

                                var item = historyItem;
                                item.ImageSource = new BitmapImage(new Uri("https://t3.gstatic.com/faviconV2?client=SOCIAL&type=FAVICON&fallback_opts=TYPE,SIZE,URL&url=" + item.Url + "&size=32"));
                                historyItems.Add(historyItem);
                            }


                            // Bind the browser history items to the ListView
                            HistoryTemp.ItemsSource = historyItems;
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that might be thrown during the execution of the code
                Debug.WriteLine($"Error: {ex.Message}");
            }
        }
        private void FavoritesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView listView = sender as ListView;
            if (listView.ItemsSource != null)
            {
                // Get selected item
                Globals.JsonItems item = (Globals.JsonItems)listView.SelectedItem;
                string launchurlfav = item.Url;
                if (TabContent.Content is WebContent)
                {
                    (TabContent.Content as WebContent).WebViewElement.CoreWebView2.Navigate(launchurlfav);
                }
                else
                {
                    TabContent.Navigate(typeof(WebContent), CreatePasser(launchurlfav));
                }

            }
            listView.ItemsSource = null;
            FavoritesFlyout.Hide();
        }

        private void History_Click(object sender, RoutedEventArgs e)
        {
            FetchBrowserHistory();
        }
        private void HistoryTemp_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView listView = sender as ListView;
            if (listView.ItemsSource != null)
            {
                // Get selected item
                HistoryItem item = (HistoryItem)listView.SelectedItem;
                string launchurlfav = item.Url;
                if (TabContent.Content is WebContent)
                {
                    (TabContent.Content as WebContent).WebViewElement.CoreWebView2.Navigate(launchurlfav);
                }
                else
                {
                    TabContent.Navigate(typeof(WebContent), CreatePasser(launchurlfav));
                }
            }
            listView.ItemsSource = null;
            HistoryFlyoutMenu.Hide();
        }

        private void SearchHistoryMenuFlyout_Click(object sender, RoutedEventArgs e)
        {
            if (HistorySearchMenuItem.Visibility == Visibility.Collapsed)
            {
                HistorySearchMenuItem.Visibility = Visibility.Visible;
                HistorySmallTitle.Visibility = Visibility.Collapsed;
            }
            else
            {
                HistorySearchMenuItem.Visibility = Visibility.Collapsed;
                HistorySmallTitle.Visibility = Visibility.Visible;
            }
        }
        private void ClearHistoryDataMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // open a connection to the database
            using (var connection = new SqliteConnection($"Data Source={ApplicationData.Current.LocalFolder.Path}\\History.db"))
            {
                connection.Open();

                // create a command that deletes all rows from the table
                var command = new SqliteCommand("DELETE FROM urlsDb", connection);

                // execute the command to clear the table
                command.ExecuteNonQuery();

                connection.Close();

                HistoryTemp.ItemsSource = null;
            }
        }

        private void OpenHistoryMenuItem_Click(object sender, RoutedEventArgs e)
        {
            TabContent.Navigate(typeof(Pages.TimeLine.Timeline));
        }


        public static async void OpenNewWindow(Uri uri)
        {
            await Windows.System.Launcher.LaunchUriAsync(uri);
        }


        private void TabMenuClick(object sender, RoutedEventArgs e)
        {
            switch ((sender as Button).Tag)
            {
                case "NewTab":
                    Tabs.TabItems.Add(CreateNewTab());
                    SelectNewTab();
                    break;
                case "NewWindow":
                    OpenNewWindow(new Uri("firebrowser://"));
                    break;
                case "Share":
                    if (TabWebView != null)
                        FireBrowserInterop.SystemHelper.ShowShareUIURL(TabWebView.CoreWebView2.DocumentTitle, TabWebView.CoreWebView2.Source);
                    break;
                case "DevTools":
                    if (TabContent.Content is WebContent) (TabContent.Content as WebContent).WebViewElement.CoreWebView2.OpenDevToolsWindow();
                    break;
                case "Settings":
                    Tabs.TabItems.Add(CreateNewTab(typeof(SettingsPage)));
                    SelectNewTab();
                    break;
                case "FullScreen":
                    ApplicationView view = ApplicationView.GetForCurrentView();
                    bool isfullmode = view.IsFullScreenMode;
                    if (!isfullmode)
                    {
                        view.TryEnterFullScreenMode();
                        TextFull.Text = "Exit FullScreen";
                        Icon.Glyph = "\uE73f";
                    }
                    else
                    {
                        view.ExitFullScreenMode();
                        TextFull.Text = "Full Screen";
                        Icon.Glyph = "\uE740";
                    }
                    break;
                case "History":
                    TabContent.Navigate(typeof(Pages.TimeLine.Timeline));
                    break;
                case "InPrivate":
                    OpenNewWindow(new Uri("firebrowser://incognito"));
                    break;
            }
        }

        public void userMenuExpend(object sender, RoutedEventArgs e)
        {
            UserBorder.Visibility = UserBorder.Visibility == Visibility.Visible
               ? Visibility.Collapsed
               : Visibility.Visible;

            UserFrame.Visibility = UserFrame.Visibility == Visibility.Visible
                ? Visibility.Collapsed
                : Visibility.Visible;
        }
    }  
}
