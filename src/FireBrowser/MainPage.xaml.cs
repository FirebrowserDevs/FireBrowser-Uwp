using CommunityToolkit.Mvvm.ComponentModel;
using FireBrowser.Controls;
using FireBrowser.Core;
using FireBrowser.Pages;
using FireBrowserCore.Models;
using FireBrowserCore.Overlay;
using FireBrowserDataBase;
using FireBrowserDataBase.BankC;
using FireBrowserDialogs.DialogTypes.UpdateChangelog;
using FireBrowserFavorites;
using FireBrowserHelpers.AdBlocker;
using FireBrowserHelpers.DarkMode;
using FireBrowserHelpers.ReadingMode;
using FireBrowserQr;
using FireBrowserUrlHelper;
using Microsoft.Data.Sqlite;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation.Metadata;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Image = Windows.UI.Xaml.Controls.Image;
using NewTab = FireBrowser.Pages.NewTab;

namespace FireBrowser
{
    public sealed partial class MainPage : Page
    {
        AppWindow RootAppWindow = null;

        public MainPage()
        {
            this.InitializeComponent();
            ButtonVisible();
            UpdateYesNo();
            ColorsTools();
        }


        #region MainWindowAndButtons
        void SetupWindow(AppWindow window)
        {
            if (window == null)
            {
                // Extend into the titlebar
                var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
                coreTitleBar.ExtendViewIntoTitleBar = true;

                coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;

                var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                titleBar.ButtonBackgroundColor = Windows.UI.Colors.Transparent;
                titleBar.ButtonInactiveBackgroundColor = Windows.UI.Colors.Transparent;
                titleBar.ButtonHoverBackgroundColor = Windows.UI.Colors.Transparent;
                titleBar.InactiveBackgroundColor = Windows.UI.Colors.Transparent;
                titleBar.ButtonPressedBackgroundColor = Windows.UI.Colors.Transparent;

                ViewModel = new ToolbarViewModel
                {
                    UserName = "FireBrowser 0x0",
                    SecurityIcon = "\uE946",
                    SecurityIcontext = "FireBrowser Home Page",
                    Securitytext = "This The Default Home Page Of Firebrowser Internal Pages Secure",
                    Securitytype = "Link - FireBrowser://NewTab",//Settings.currentProfile.AccountData.Name,             
                };

                Window.Current.SetTitleBar(CustomDragRegion);
            }
            else
            {
                // Secondary AppWindows --- keep track of the window
                RootAppWindow = window;

                // Extend into the titlebar
                window.TitleBar.ExtendsContentIntoTitleBar = true;
                window.TitleBar.ButtonBackgroundColor = Windows.UI.Colors.Transparent;
                window.TitleBar.ButtonInactiveBackgroundColor = Windows.UI.Colors.Transparent;
                window.TitleBar.ButtonInactiveForegroundColor = Windows.UI.Colors.Transparent;
                window.TitleBar.ButtonPressedBackgroundColor = Windows.UI.Colors.Transparent;
                window.TitleBar.ButtonHoverBackgroundColor = Windows.UI.Colors.Transparent;

                ViewModel = new ToolbarViewModel
                {
                    UserName = "FireBrowser 0x0",
                    SecurityIcon = "\uE946",
                    SecurityIcontext = "FireBrowser Home Page",
                    Securitytext = "This The Default Home Page Of Firebrowser Internal Pages Secure",
                    Securitytype = "Link - FireBrowser://NewTab",//Settings.currentProfile.AccountData.Name,             
                };

                // Due to a bug in AppWindow, we cannot follow the same pattern as CoreWindow when setting the min width.
                // Instead, set a hardcoded number. 
                CustomDragRegion.MinWidth = 188;

                window.Frame.DragRegionVisuals.Add(CustomDragRegion);
            }
        }

        public static string ReadButton { get; set; }
        public static string AdblockBtn { get; set; }
        public static string Downloads { get; set; }
        public static string Translate { get; set; }
        public static string Favorites { get; set; }
        public static string Historybtn { get; set; }
        public static string QrCode { get; set; }
        public static string FavoritesL { get; set; }
        public static string ToolIcon { get; set; }
        public static string DarkIcon { get; set; }

        public void ButtonVisible()
        {
            var buttons = new Dictionary<Button, string>
            {
               { ReadBtn, "Readbutton" },
               { AdBlock, "AdBtn" },
               { DownBtn, "DwBtn" },
               { BtnTrans, "TransBtn" },
               { FavoritesButton, "FavBtn" },
               { History, "HisBtn" },
               { QrBtn, "QrBtn" },
               { AddFav, "FlBtn" },
               { ToolBoxMore, "ToolBtn" },
               { BtnDark, "DarkBtn" }
            };

            foreach (var button in buttons)
            {
                var settingValue = FireBrowserInterop.SettingsHelper.GetSetting(button.Value);

                if (settingValue == "True")
                {
                    button.Key.Visibility = Visibility.Visible;
                }
                else
                {
                    button.Key.Visibility = Visibility.Collapsed;
                }
            }
        }


        #endregion

        #region TabsCode

        public void AddTabToTabs(TabViewItem tab)
        {
            Tabs.TabItems.Add(tab);
        }

        string testSetting = FireBrowserInterop.SettingsHelper.GetSetting("DragOutSideExperiment");  

        private async void Tabs_TabItemsChanged(TabView sender, Windows.Foundation.Collections.IVectorChangedEventArgs args)
        {
            // If there are no more tabs, close the window.
            if (sender.TabItems.Count == 0)
            {
                if (RootAppWindow != null)
                {
                    await RootAppWindow.CloseAsync();
                }
                else
                {
                    CoreApplication.Exit();
                }
            }
            // If there is only one tab left, disable dragging and reordering of Tabs.
            else if (sender.TabItems.Count == 1)
            {
                sender.CanReorderTabs = false;
                sender.CanDragTabs = false;
            }
            else
            {
                sender.CanReorderTabs = true;
                sender.CanDragTabs = true;
            }
        }

        private const string DataIdentifier = "Tab";
        private void Tabs_TabDragStarting(TabView sender, TabViewTabDragStartingEventArgs args)
        {
            // We can only drag one tab at a time, so grab the first one...
            var firstItem = args.Tab;

            // ... set the drag data to the tab...
            args.Data.Properties.Add(DataIdentifier, firstItem);

            // ... and indicate that we can move it 
            args.Data.RequestedOperation = DataPackageOperation.Move;
        }

        private void Tabs_TabStripDragOver(object sender, DragEventArgs e)
        {
            if (e.DataView.Properties.ContainsKey(DataIdentifier))
            {
                e.AcceptedOperation = DataPackageOperation.Move;
            }
        }

        private void Tabs_TabStripDrop(object sender, DragEventArgs e)
        {

            if (e.DataView.Properties.TryGetValue(DataIdentifier, out object obj))
            {
                // Ensure that the obj property is set before continuing. 
                if (obj == null)
                {
                    return;
                }

                var destinationTabView = sender as TabView;
                var destinationItems = destinationTabView.TabItems;

                if (destinationItems != null)
                {
                    // First we need to get the position in the List to drop to
                    var index = -1;

                    // Determine which items in the list our pointer is between.
                    for (int i = 0; i < destinationTabView.TabItems.Count; i++)
                    {
                        var item = destinationTabView.ContainerFromIndex(i) as TabViewItem;

                        if (e.GetPosition(item).X - item.ActualWidth < 0)
                        {
                            index = i;
                            break;
                        }
                    }

                    // The TabView can only be in one tree at a time. Before moving it to the new TabView, remove it from the old.
                    var destinationTabViewListView = ((obj as TabViewItem).Parent as TabViewListView);
                    destinationTabViewListView.Items.Remove(obj);

                    if (index < 0)
                    {
                        // We didn't find a transition point, so we're at the end of the list
                        destinationItems.Add(obj);
                    }
                    else if (index < destinationTabView.TabItems.Count)
                    {
                        // Otherwise, insert at the provided index.
                        destinationItems.Insert(index, obj);
                    }

                    // Select the newly dragged tab
                    destinationTabView.SelectedItem = obj;
                }
            }
        }

        #endregion
        public async void UpdateYesNo()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            var currentVersion = localSettings.Values["AppVersion"] as string;
            var appVersion = Windows.ApplicationModel.Package.Current.Id.Version;
            var versionString = $"{appVersion.Major}.{appVersion.Minor}.{appVersion.Build}.{appVersion.Revision}";

            if (currentVersion != versionString)
            {
                // Create and show content dialog
                var dialog = new UpdateChangelog
                {
                    Title = "Update - Changelog",
                    PrimaryButtonText = "OK"
                };
                await dialog.ShowAsync();

                // Update current version in local settings
                localSettings.Values["AppVersion"] = versionString;
            }
        }

        private void SetBackground(string colorKey, Panel panel)
        {
            var colorString = FireBrowserInterop.SettingsHelper.GetSetting(colorKey);
            if (colorString == "#000000")
            {
                panel.Background = new SolidColorBrush(Colors.Transparent);
            }
            else
            {
                var color = (Windows.UI.Color)XamlBindingHelper.ConvertValue(typeof(Windows.UI.Color), colorString);
                var brush = new SolidColorBrush(color);
                panel.Background = brush;
            }
        }

        private void SetBackgroundTabs(string colorKey, TabView panel)
        {
            var colorString = FireBrowserInterop.SettingsHelper.GetSetting(colorKey);
            if (colorString == "#000000")
            {
                panel.Background = new SolidColorBrush(Colors.Transparent);
            }
            else
            {
                var color = (Windows.UI.Color)XamlBindingHelper.ConvertValue(typeof(Windows.UI.Color), colorString);
                var brush = new SolidColorBrush(color);
                panel.Background = brush;
            }
        }
     
        public void ColorsTools()
        {
           SetBackground("ColorTool", ClassicToolbar);
           SetBackgroundTabs("ColorTv", Tabs);
        }

        bool incog = false;
    
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            SetupWindow(null);
           

            if (e.Parameter is AppOverlay.AppLaunchPasser passer)
            {
                switch (passer.LaunchType)
                {
                    case AppOverlay.AppLaunchType.LaunchBasic:
                        Tabs.TabItems.Add(CreateNewTab(typeof(NewTab)));
                        break;
                    case AppOverlay.AppLaunchType.LaunchIncognito:
                        RequestedTheme = ElementTheme.Dark;
                        VBtn.Visibility = Visibility.Collapsed;
                        ViewModel.Securitytype = "Link - Firebrowser://incognito";
                        History.IsEnabled = false;
                        DownBtn.IsEnabled = false;
                        AddFav.IsEnabled = false;
                        incog = true;
                        FavoritesButton.IsEnabled = false;
                        WebContent.IsIncognitoModeEnabled = true;
                        ClassicToolbar.Height = 35;
                        Tabs.TabItems.Add(CreateNewIncog());
                        break;
                    case AppOverlay.AppLaunchType.LaunchStartup:
                        Tabs.TabItems.Add(CreateNewTab(typeof(NewTab)));
                        var startup = await StartupTask.GetAsync("FireBrowserStartUp");
                        break;
                    case AppOverlay.AppLaunchType.FirstLaunch:
                        Tabs.TabItems.Add(CreateNewTab());
                        break;
                    case AppOverlay.AppLaunchType.FilePDF:
                        var files = passer.LaunchData as IReadOnlyList<IStorageItem>;
                        Tabs.TabItems.Add(CreateNewTab(typeof(Pages.PdfReader), files[0]));
                        break;
                    case AppOverlay.AppLaunchType.URIHttp:
                        Tabs.TabItems.Add(CreateNewTab(typeof(WebContent), new Uri(passer.LaunchData.ToString())));
                        break;
                    case AppOverlay.AppLaunchType.URIFireBrowser:
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
            public bool canRefresh;
            [ObservableProperty]
            public bool canGoBack;
            [ObservableProperty]
            public bool canGoForward;
            [ObservableProperty]
            public string favoriteIcon;
            [ObservableProperty]
            public bool permissionDialogIsOpen;
            [ObservableProperty]
            public string permissionDialogTitle;
            [ObservableProperty]
            public string currentAddress;
            [ObservableProperty]
            public string securityIcon;
            [ObservableProperty]
            public string securityIcontext;
            [ObservableProperty]
            public string securitytext;
            [ObservableProperty]
            public string securitytype;
            [ObservableProperty]
            public Visibility homeButtonVisibility;

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
            var rightInset = FlowDirection == FlowDirection.LeftToRight ? sender.SystemOverlayRightInset : sender.SystemOverlayLeftInset;
            var leftInset = FlowDirection == FlowDirection.LeftToRight ? sender.SystemOverlayLeftInset : sender.SystemOverlayRightInset;

            CustomDragRegion.MinWidth = rightInset;
            VBtn.MinWidth = leftInset;

            CustomDragRegion.Height = VBtn.Height = sender.Height;
        }


        public class Passer
        {
            public FireBrowserTabViewItem Tab { get; set; }
            public FireBrowserTabView TabView { get; set; }
            public object Param { get; set; }
            public ToolbarViewModel ViewModel { get; set; }

            public string UserName { get; set; }
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
                Tab = Tabs.SelectedItem as FireBrowserTabViewItem,
                TabView = Tabs,
                ViewModel = ViewModel,
                Param = parameter,
            };
            return passer;
        }
        #endregion

        #region frames
        public Frame TabContent => (Tabs.SelectedItem as FireBrowserTabViewItem)?.Content as Frame;

        public WebView2 TabWebView => (TabContent?.Content as WebContent)?.WebViewElement;

        #endregion


        public FireBrowserTabViewItem CreateNewIncog(Type page = null, object param = null, int index = -1)
        {
            if (index == -1) index = Tabs.TabItems.Count;


            UrlBox.Text = "";

            FireBrowserTabViewItem newItem = new()
            {
                Header = $"Incognito",
                IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = Symbol.BlockContact }
            };

            Passer passer = new()
            {
                Tab = newItem,
                TabView = Tabs,
                ViewModel = new ToolbarViewModel(),
                Param = param
            };

            newItem.Style = (Style)Application.Current.Resources["FloatingTabViewItemStyle"];

            // The content of the tab is often a frame that contains a page, though it could be any UIElement.

            Frame frame = new()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Margin = new Thickness(0, 37, 0, 0)
            };

            if (page != null)
            {
                frame.Navigate(page, passer);
            }
            else
            {
                frame.Navigate(typeof(Pages.Incognito), passer);
            }


            newItem.Content = frame;
            return newItem;
        }


        public FireBrowserTabViewItem CreateNewTab(Type page = null, object param = null, int index = -1)
        {
            if (index == -1) index = Tabs.TabItems.Count;


            UrlBox.Text = "";

            FireBrowserTabViewItem newItem = new()
            {
                Header = $"FireBrowser HomePage",
                IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = Symbol.Home }
            };


            Passer passer = new()
            {
                Tab = newItem,
                TabView = Tabs,
                ViewModel = new ToolbarViewModel(),
                Param = param,
            };

            passer.ViewModel.CurrentAddress = null;

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

         

            ToolTip toolTip = new();
            Grid grid = new();
            Image previewImage = new();
            TextBlock textBlock = new();
           // textBlock.Text = GetTitle();
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

        string test2 = FireBrowserInterop.SettingsHelper.GetSetting("VaultExperiment");

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
                    case "firebrowser://webview":
                        TabContent.Navigate(typeof(SettingsPage), CreatePasser(), new DrillInNavigationTransitionInfo());
                        break;
                    case "firebrowser://apps":
                    case "firebrowser://history":
                    case "firebrowser://favorites":
                        TabContent.Navigate(typeof(Pages.TimeLine.Timeline));
                        break;
                    case "firebrowser://pdf":
                        TabContent.Navigate(typeof(Pages.PdfReader), CreatePasser(), new DrillInNavigationTransitionInfo());
                        break;
                    case "firebrowser://hidden":
                        TabContent.Navigate(typeof(Pages.HiddenFeatures.HiddenFt), CreatePasser(), new DrillInNavigationTransitionInfo());
                        break;
                    case "firebrowser://vault":
                        if(test2 == "0x1")
                        {
                            TabContent.Navigate(typeof(Pages.HiddenFeatures.CreditVault), CreatePasser(), new DrillInNavigationTransitionInfo());
                        }
                        else
                        {
                            ContentDialog cn = new ContentDialog();
                            cn.Title = "Turn This On In firebrowser://hidden";
                            cn.Content = "firebrowser://hidden,  vault experiment on to use this";
                            cn.PrimaryButtonText = "OK";
                            await cn.ShowAsync();
                        }                   
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
            ViewModel.CanGoBack = (bool)(TabContent?.Content is WebContent
                ? TabWebView?.CoreWebView2.CanGoBack
                : TabContent?.CanGoBack);

            return ViewModel.CanGoBack;
        }


        private bool CanGoForward()
        {
            ViewModel.CanGoForward = (bool)(TabContent?.Content is WebContent
                ? TabWebView?.CoreWebView2.CanGoForward
                : TabContent?.CanGoForward);
            return ViewModel.CanGoForward;
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
            if (TabContent?.Content is WebContent webContent)
            {
                TabWebView.NavigationStarting += (s, e) =>
                {
                    ViewModel.CanRefresh = false;
                };
                TabWebView.NavigationCompleted += (s, e) =>
                {
                    ViewModel.CanRefresh = true;
                };
                await TabWebView.EnsureCoreWebView2Async();
                SmallUpdates();
            }
            else
            {
                ViewModel.CanRefresh = false;
                ViewModel.CurrentAddress = null;
            }

            ViewModel.FavoriteIcon = "\uF714";
        }

        public void SmallUpdates()
        {
            var source = TabWebView.CoreWebView2.Source.ToString();
            UrlBox.Text = ViewModel.Securitytype = source;

            ViewModel.SecurityIcon = source.Contains("https") ? "\uE72E" : "\uE785";
            ViewModel.SecurityIcontext = source.Contains("https") ? "Https Secured Website" : "Http UnSecured Website";
            ViewModel.Securitytext = source.Contains("https") ? "This Page Is Secured By A Valid SSL Certificate, Trusted By Root Authorities" : "This Page Is Unsecured By A Un-Valid SSL Certificate, Please Be Careful";
        }


        private async void ToolbarButtonClick(object sender, RoutedEventArgs e)
        {
            Passer passer = new()
            {
                Tab = Tabs.SelectedItem as FireBrowserTabViewItem,
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
                    break;
                case "Home":
                    if (TabContent.Content is WebContent)
                    {
                        Hmbtn.IsEnabled = true;
                        TabContent.Navigate(WebContent.IsIncognitoModeEnabled
                            ? typeof(Pages.Incognito)
                            : typeof(Pages.NewTab), passer, new DrillInNavigationTransitionInfo());

                        passer.Tab.Header = WebContent.IsIncognitoModeEnabled
                            ? "Incognito"
                            : "FireBrowser HomePage";

                        passer.Tab.IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource()
                        {
                            Symbol = WebContent.IsIncognitoModeEnabled
                                ? Symbol.BlockContact
                                : Symbol.Home
                        };

                        UrlBox.Text = "";
                    }
                    else
                    {
                        Hmbtn.IsEnabled = true;
                        TabContent.Navigate(incog
                            ? typeof(Pages.Incognito)
                            : typeof(Pages.NewTab), passer, new DrillInNavigationTransitionInfo());

                        passer.Tab.Header = incog
                            ? "Incognito"
                            : "FireBrowser HomePage";

                        passer.Tab.IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource()
                        {
                            Symbol = incog
                                ? Symbol.BlockContact
                                : Symbol.Home
                        };

                        UrlBox.Text = "";
                    }
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
                    await FavoritesHelper.AddFavoritesItem(FavoriteTitle.Text, FavoriteUrl.Text);
                    break;
                case "Favorites":
                    Globals.JsonItemsList = await Json.GetListFromJsonAsync("Favorites.json");
                    if (Globals.JsonItemsList is not null)
                    {
                        FavoritesListView.ItemsSource = Globals.JsonItemsList;
                    }
                    break;
                case "DarkMode":
                    if (TabContent.Content is WebContent)
                    {
                        string jscript = await ForceDarkHelper.GetForceDark();
                        await (TabContent.Content as WebContent).WebViewElement.CoreWebView2.ExecuteScriptAsync(jscript);
                    }
                    break;
            }
        }

        #region tabsbuttons
        private void Tabs_AddTabButtonClick(TabView sender, object args)
        {
            sender.TabItems.Add(incog ? CreateNewIncog() : CreateNewTab());
            SelectNewTab();
        }

        public void SelectNewTab() => Tabs.SelectedIndex = Tabs.TabItems.Count - 1;


        private void Tabs_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            if (args.Tab.Content is WebContent webContent)
                webContent.WebViewElement?.Close();

            (sender as TabView)?.TabItems?.Remove(args.Tab);
        }

        #endregion

        private ObservableCollection<HistoryItem> browserHistory;
        private async void FetchBrowserHistory()
        {
            Batteries.Init();
            try
            {
                using (var connection = new SqliteConnection($"Data Source={ApplicationData.Current.LocalFolder.Path}/History.db;"))
                {
                    // Open the database connection asynchronously
                    await connection.OpenAsync();

                    // Define the SQL query to fetch the browser history
                    string sql = "SELECT url, title, visit_count, last_visit_time FROM urlsDb ORDER BY last_visit_time DESC";

                    // Create a command object with the SQL query and connection
                    using (SqliteCommand command = new SqliteCommand(sql, connection))
                    {
                        using (SqliteDataReader reader = command.ExecuteReader())
                        {
                            // Create an observable collection to store the browser history items
                            browserHistory = new ObservableCollection<HistoryItem>();

                            while (reader.Read())
                            {
                                HistoryItem historyItem = new HistoryItem
                                {
                                    Url = reader.GetString(0),
                                    Title = reader.IsDBNull(1) ? null : reader.GetString(1),
                                    VisitCount = reader.GetInt32(2),
                                    LastVisitTime = reader.GetString(3),
                                };

                               
                                historyItem.ImageSource = new BitmapImage(new Uri("https://t3.gstatic.com/faviconV2?client=SOCIAL&type=FAVICON&fallback_opts=TYPE,SIZE,URL&url=" + historyItem.Url + "&size=32"));
                                browserHistory.Add(historyItem);
                               
                            }


                            // Bind the browser history items to the ListView
                            HistoryTemp.ItemsSource = browserHistory;
                        }
                        connection.Close();
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
            ToggleVisibility(HistorySearchMenuItem, Visibility.Collapsed);
            ToggleVisibility(HistorySmallTitle, Visibility.Visible);
        }

        private async void ClearHistoryDataMenuItem_Click(object sender, RoutedEventArgs e) =>
            await DbClear.ClearDb().ContinueWith(_ => HistoryTemp.ItemsSource = null);

        private void OpenHistoryMenuItem_Click(object sender, RoutedEventArgs e)
        {
            UrlBox.Text = "firebrowser://history";
            Task.Delay(5).Wait();
            TabContent.Navigate(typeof(Pages.TimeLine.Timeline));
        }

        public static async void OpenNewWindow(Uri uri) => await Windows.System.Launcher.LaunchUriAsync(uri);

        private static void ToggleVisibility(UIElement element, Visibility visibility) =>
            element.Visibility = element.Visibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;


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
                case "Favorites":
                    UrlBox.Text = "firebrowser://favorites";
                    Thread.Sleep(5);
                    TabContent.Navigate(typeof(Pages.TimeLine.Timeline));
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

        private void OpenFavoritesMenu_Click(object sender, RoutedEventArgs e)
        {
            UrlBox.Text = "firebrowser://favorites";
            Task.Delay(5).Wait();
            TabContent.Navigate(typeof(Pages.TimeLine.Timeline));
        }

        private void FilterBrowserHistory(string searchText)
        {
            if (browserHistory == null) return;

            HistoryTemp.ItemsSource = null;

            var filteredHistory = new ObservableCollection<HistoryItem>(browserHistory
                .Where(item => item.Url.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                               item.Title?.Contains(searchText, StringComparison.OrdinalIgnoreCase) == true));

            HistoryTemp.ItemsSource = filteredHistory;
        }



        private void HistorySearchMenuItem_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = HistorySearchMenuItem.Text;
            FilterBrowserHistory(searchText);
        }

        private string selectedHistoryItem;

        private void Grid_RightTapped(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
        {
            // Get the selected HistoryItem object
            HistoryItem historyItem = ((FrameworkElement)sender).DataContext as HistoryItem;
            selectedHistoryItem = historyItem.Url;

            // Create a context menu flyout
            var flyout = new MenuFlyout();

            // Add a menu item for "Delete This Record" button
            var deleteMenuItem = new MenuFlyoutItem
            {
                Text = "Delete This Record",
            };

            // Set the icon for the menu item using the Unicode escape sequence
            deleteMenuItem.Icon = new FontIcon
            {
                Glyph = "\uE74D" // Replace this with the Unicode escape sequence for your desired icon
            };

            // Handle the click event directly within the right-tapped event handler
            deleteMenuItem.Click += (s, args) =>
            {
                // Perform the deletion logic here
                // Example: Delete data from the 'History' table where the 'Url' matches the selectedHistoryItem
                DbClearTableData db = new();
                db.DeleteTableData("urlsDb", $"Url = '{selectedHistoryItem}'");
                if (HistoryTemp.ItemsSource is ObservableCollection<HistoryItem> historyItems)
                {
                    var itemToRemove = historyItems.FirstOrDefault(item => item.Url == selectedHistoryItem);
                    if (itemToRemove != null)
                    {
                        historyItems.Remove(itemToRemove);
                    }
                }
                // After deletion, you may want to update the UI or any other actions
            };

            flyout.Items.Add(deleteMenuItem);

            // Show the context menu flyout
            flyout.ShowAt((FrameworkElement)sender, e.GetPosition((FrameworkElement)sender));
        }    
    }
}
