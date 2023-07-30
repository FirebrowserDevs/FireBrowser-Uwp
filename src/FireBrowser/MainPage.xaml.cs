using CommunityToolkit.Mvvm.ComponentModel;
using FireBrowser.Controls;
using FireBrowser.Core;
using FireBrowser.Pages;
using FireBrowser.Pages.TimeLine;
using FireBrowserCore.Models;
using FireBrowserDataBase;
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
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Threading;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation.Metadata;
using Windows.Graphics.Display;
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
using static FireBrowser.App;
using Image = Windows.UI.Xaml.Controls.Image;
using NewTab = FireBrowser.Pages.NewTab;
using Point = Windows.Foundation.Point;


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
        AppWindow RootAppWindow = null;

        public MainPage()
        {
            this.InitializeComponent();
            ButtonVisible();
            UpdateYesNo();
           // ColorsTools();
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

        private void ResetOutput()
        {
            DisplayInformation displayInformation = DisplayInformation.GetForCurrentView();
            String scaleValue = (displayInformation.RawPixelsPerViewPixel * 100.0).ToString();
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
        private void Tabs_TabDroppedOutside(TabView sender, TabViewTabDroppedOutsideEventArgs args)
        {
            if (testSetting == "0x1")
            {
               // MoveTabToNewWindow(args.Tab);
            }
            else
            {

            }
        }

        private async void MoveTabToNewWindow(TabViewItem tab)
        {
            // AppWindow was introduced in Windows 10 version 18362 (ApiContract version 8). 
            // If the app is running on a version earlier than 18362, simply no-op.
            // If your app needs to support multiple windows on earlier versions of Win10, you can use CoreWindow/ApplicationView.
            // More information about showing multiple views can be found here: https://docs.microsoft.com/windows/uwp/design/layout/show-multiple-views
            if (!ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 10))
            {
                return;
            }

            AppWindow newWindow = await AppWindow.TryCreateAsync();
            
            MainPage newPage = new MainPage();
                      
            newPage.SetupWindow(newWindow);
    
            ElementCompositionPreview.SetAppWindowContent(newWindow, newPage);
            newWindow.TitleBar.BackgroundColor = Colors.Transparent;
            BackdropMaterial.SetApplyToRootOrPageBackground(newPage, true);


            Tabs.TabItems.Remove(tab);
            newPage.AddTabToTabs(tab);

            await newWindow.TryShowAsync();
        }

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
           var color = colorString == "#000000" ? Colors.Transparent : Microsoft.Toolkit.Uwp.Helpers.ColorHelper.ToColor(colorString);
           var brush = new SolidColorBrush(color);
           panel.Background = brush;
        }

        private void SetBackgroundTabs(string colorKey, TabView panel)
        {
          var colorString = FireBrowserInterop.SettingsHelper.GetSetting(colorKey);
          var color = colorString == "#000000" ? Colors.Transparent : Microsoft.Toolkit.Uwp.Helpers.ColorHelper.ToColor(colorString);
          var brush = new SolidColorBrush(color);
          panel.Background = brush;
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
            ResetOutput();
            SetupWindow(null);

            if (e.Parameter is AppLaunchPasser passer)
            {
                switch (passer.LaunchType)
                {
                    case AppLaunchType.LaunchBasic:
                        Tabs.TabItems.Add(CreateNewTab(typeof(NewTab)));
                        break;
                    case AppLaunchType.LaunchIncognito:
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
                    case AppLaunchType.LaunchStartup:
                        Tabs.TabItems.Add(CreateNewTab(typeof(NewTab)));
                        var startup = await StartupTask.GetAsync("FireBrowserStartUp");
                        break;
                    case AppLaunchType.FirstLaunch:
                        Tabs.TabItems.Add(CreateNewTab());
                        break;
                    case AppLaunchType.FilePDF:
                        var files = passer.LaunchData as IReadOnlyList<IStorageItem>;
                        Tabs.TabItems.Add(CreateNewTab(typeof(Pages.PdfReader), files[0]));
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

        private bool isDragging = false;
        private Point lastPosition;
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
            if (FlowDirection == FlowDirection.LeftToRight)
            {
                CustomDragRegion.MinWidth = sender.SystemOverlayRightInset;
                VBtn.MinWidth = sender.SystemOverlayLeftInset;
            }
            else
            {
                CustomDragRegion.MinWidth = sender.SystemOverlayLeftInset;
                VBtn.MinWidth = sender.SystemOverlayRightInset;
            }

            // Ensure that the height of the custom regions are the same as the titlebar.
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
        public Frame TabContent
        {
            get
            {
                FireBrowserTabViewItem selectedItem = (FireBrowserTabViewItem)Tabs.SelectedItem;
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


        private void UrlBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
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
                TabWebView.NavigationStarting += async (s, e) =>
                {
                    ViewModel.CanRefresh = false;
                };
                TabWebView.NavigationCompleted += async (s, e) =>
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
            UrlBox.Text = TabWebView.CoreWebView2.Source.ToString();
            ViewModel.Securitytype = TabWebView.CoreWebView2.Source.ToString();

            if (TabWebView.CoreWebView2.Source.Contains("https"))
            {
                ViewModel.SecurityIcon = "\uE72E";
            }
            else if (TabWebView.CoreWebView2.Source.Contains("http"))
            {
                ViewModel.SecurityIcon = "\uE785";
            }
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
            if (incog == true)
            {
                sender.TabItems.Add(CreateNewIncog());
            }
            else
            {
                sender.TabItems.Add(CreateNewTab());
            }
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
            var tabContent = (Frame)selectedItem.Content;

            if (tabContent.Content is WebContent webContent)
            {
                var webView = webContent.WebViewElement;

                if (webView != null)
                {
                    webView.Close();
                }
            }

            var tabItems = (sender as TabView)?.TabItems;
            tabItems?.Remove(args.Tab);
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
                    // Open the database connection
                    connection.Open();
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
        private async void ClearHistoryDataMenuItem_Click(object sender, RoutedEventArgs e)
        {
            await DbClear.ClearDb();
            HistoryTemp.ItemsSource = null;
        }

        private void OpenHistoryMenuItem_Click(object sender, RoutedEventArgs e)
        {
            UrlBox.Text = "firebrowser://history";
            Thread.Sleep(5);
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
            Thread.Sleep(5);
            TabContent.Navigate(typeof(Pages.TimeLine.Timeline));
        }

    }
}
