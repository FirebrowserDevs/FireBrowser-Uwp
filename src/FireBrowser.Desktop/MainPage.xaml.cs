using Windows.ApplicationModel.Core;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Core.Preview;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI;
using CommunityToolkit.Mvvm.ComponentModel;
using Windows.UI.Xaml.Input;
using FireBrowser.Core;
using static FireBrowser.Core.Resources;
using FireBrowser.Pages;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml.Navigation;
using System.Collections.Generic;
using Windows.Storage;
using System.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.ExtendedExecution.Foreground;
using Windows.UI.Xaml.Media;
using FireBrowser.Pages.Workspaces;
using FireBrowser.Controls;
using FireBrowser.Pages.EasterEgg;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using FireBrowser.History;
using QRCoder;
using System.Net.Http;
using System.Xml.Serialization;
using Dots.SDK.UWP;
using static FireBrowser.MainPage;
using JsonSerializer = System.Text.Json.JsonSerializer;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.UI.Xaml.Controls.Primitives;
using FireBrowser.Controls.BrowserEngines;
using Dots.SDK;
using System.Diagnostics;
using static FireBrowser.Core.Favorites;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Data.Sqlite;
using System.Data;
using Windows.UI.WebUI;

namespace FireBrowser
{
    public class StringOrIntTemplateSelector : DataTemplateSelector
    {
        // Define the (currently empty) data templates to return
        // These will be "filled-in" in the XAML code.
        public DataTemplate StringTemplate { get; set; }

        public DataTemplate IntTemplate { get; set; }
        public DataTemplate DefaultTemplate { get; set; }
        protected override DataTemplate SelectTemplateCore(object item)
        {
            var suggestion = item as EverythingBoxSuggestion;
            // Return the correct data template based on the item's type.
            if (suggestion.Type == EverythingBoxSuggestionType.Search)
            {
                return DefaultTemplate;
            }
            else if (item.GetType() == typeof(int))
            {
                return IntTemplate;
            }
            else
            {
                return null;
            }
        }
    }
    public sealed partial class MainPage : Page
    {
        /// <summary>
        /// The Passer class is using for passing a Tab and it's data from the TabView to the actual
        /// page inside of the tab.
        /// </summary>
        public class Passer
        {
            public TabViewItem Tab { get; set; }
            public TabView TabView { get; set; }
            public ToolbarViewModel ViewModel { get; set; }
            public object Param { get; set; }
        }
        private Passer CreatePasser(object parameter = null)
        {
            Passer passer = new()
            {
                Tab = Tabs.SelectedItem as TabViewItem,
                TabView = Tabs,
                ViewModel = ViewModel,
                Param = parameter
            };
            return passer;
        }
        /* 
         private void ShareSourceLoad()
{
    DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
    dataTransferManager.DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.DataRequested);
}

private void DataRequested(DataTransferManager sender, DataRequestedEventArgs e)
{
    DataRequest request = e.Request;
    request.Data.Properties.Title = "Share Text Example";
    request.Data.Properties.Description = "An example of how to share text.";
    request.Data.SetText("Hello World!");
}
         */
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("ODk3Nzc5QDMyMzAyZTM0MmUzMGo1ZUMxYkxZRCsrU1NpZ2M1dnJQaDlEU1JRYlB5MTFsYjB3clRWSGU2YUE9");

            if (e.Parameter != null)
            {
                AppLaunchPasser passer = (AppLaunchPasser)e.Parameter;

                switch (passer.LaunchType)
                {
                    case AppLaunchType.LaunchIncognito:
                        RequestedTheme = ElementTheme.Dark;
                        ViewModel.IsIncognito = true;
                        History.IsEnabled = false;
                        ViewModel.UserName = "Incognito";
                        Incognito.IsEnabled = true;
                        Profile.IsEnabled = false;
                        //To-Do...
                        Tabs.AddTabButtonClick += (sender, args) =>
                        {
                            sender.TabItems.Add(CreateNewTab(typeof(NewTabIncognito)));
                        };
                        break;
                    case AppLaunchType.LaunchStartup:
                        var startup = await StartupTask.GetAsync("AirplaneStartUp");
                        break;
                    case AppLaunchType.FirstLaunch:
                        Tabs.TabItems.Add(CreateNewTab(typeof(FirstLaunchPage), 1));
                        break;
                    case AppLaunchType.FilePDF:
                        Dots.SDK.UWP.Log.WriteLog("App was launched from a file", "MainPage.OnNavigatedTo", Dots.SDK.Log.LogType.Info);

                        var files = passer.LaunchData as IReadOnlyList<IStorageItem>;
                        Tabs.TabItems.Add(CreateNewTab(typeof(PDFReader), files[0]));
                        break;
                    case AppLaunchType.URIHttp:
                        Tabs.TabItems.Add(CreateNewTab(typeof(WebContent),
                                                       new Uri(passer.LaunchData.ToString())));
                        break;
                }
            }
            else
            {
                Tabs.TabItems.Add(CreateNewTab());
            }

        }

        public MainPage()
        {
            //https://github.com/microsoft/microsoft-ui-xaml/blob/6aed8d97fdecfe9b19d70c36bd1dacd9c6add7c1/dev/Materials/Acrylic/AcrylicBrush_19h1_themeresources.xaml#L11
            //To-Do: Use this Acrylic Brush for the Flyouts
            InitializeComponent();

            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;

            ApplicationViewTitleBar formattableTitleBar = ApplicationView.GetForCurrentView().TitleBar;
            formattableTitleBar.ButtonBackgroundColor = Colors.Transparent;
            formattableTitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

            Window.Current.SetTitleBar(CustomDragRegion);

            ViewModel = new ToolbarViewModel
            {
                UserName = "Temporary", //Settings.currentProfile.AccountData.Name,
                LoadingState = new FontIcon()
                {
                    Glyph = "\uF13E",
                }
            };

            SystemNavigationManagerPreview.GetForCurrentView().CloseRequested += async (sender, args) =>
            {
                if (/*Settings.currentProfile.UI.ShowCloseAllDialog && */ Tabs.TabItems.Count > 1)
                {
                    args.Handled = true;

                    var result = await CloseWithOpenTabsDialog.ShowWithAnimationAsync();

                    if (result == ContentDialogResult.Secondary)
                    {
                        if (DontAskAgainCheckbox.IsChecked == true)
                        {
                            //Settings.currentProfile.UI.ShowCloseAllDialog = false;
                            //AppData.SaveData(Settings.appSettings, AppData.DataType.AppSettings);
                        }
                        await ApplicationView.GetForCurrentView().TryConsolidateAsync();
                    }
                }
                else
                {
                    await ApplicationView.GetForCurrentView().TryConsolidateAsync();
                }
                args.Handled = true;
            };

            Tabs.TabItemsChanged += Tabs_TabItemsChanged;
        }


        private void NewTabKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            var senderTabView = args.Element as TabView;
            senderTabView.TabItems.Add(CreateNewTab());
            SelectNewTab();
            args.Handled = true;
        }

        /// <summary>
        /// Computer\HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\BackgroundAccessApplications\57453DotsStudio.Airplane_ngs6csccwd0b4
        /// </summary>
        /// <param name="window"></param>


        void SetupWindow(AppWindow window)
        {
            if (window == null)
            {
                // Main Window -- add some default items
                for (int i = 0; i < 1; i++)
                {
                    Tabs.TabItems.Add(CreatePasser());
                }

                Tabs.SelectedIndex = 0;


                // Extend into the titlebar
                var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
                coreTitleBar.ExtendViewIntoTitleBar = true;

                coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;

                var titleBar = ApplicationView.GetForCurrentView().TitleBar;
                titleBar.ButtonBackgroundColor = Colors.Transparent;
                titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                titleBar.BackgroundColor = Colors.Transparent;

                Window.Current.SetTitleBar(CustomDragRegion);
            }
            else
            {

                // Extend into the titlebar
                window.TitleBar.ExtendsContentIntoTitleBar = true;
                window.TitleBar.ButtonBackgroundColor = Colors.Transparent;
                window.TitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
                window.TitleBar.BackgroundColor = Colors.Transparent;

                // Due to a bug in AppWindow, we cannot follow the same pattern as CoreWindow when setting the min width.
                // Instead, set a hardcoded number. 
                CustomDragRegion.MinWidth = 188;

                window.Frame.DragRegionVisuals.Add(CustomDragRegion);
            }

        }

        private void CloseSelectedTabKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            var InvokedTabView = (args.Element as TabView);

            // Only close the selected tab if it is closeable
            if (((TabViewItem)InvokedTabView.SelectedItem).IsClosable)
            {
                InvokedTabView.TabItems.Remove(InvokedTabView.SelectedItem);
            }
            args.Handled = true;
        }

        private void NavigateToNumberedTabKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            var InvokedTabView = (args.Element as TabView);

            int tabToSelect = 0;

            switch (sender.Key)
            {
                case Windows.System.VirtualKey.Number1:
                    tabToSelect = 0;
                    break;
                case Windows.System.VirtualKey.Number2:
                    tabToSelect = 1;
                    break;
                case Windows.System.VirtualKey.Number3:
                    tabToSelect = 2;
                    break;
                case Windows.System.VirtualKey.Number4:
                    tabToSelect = 3;
                    break;
                case Windows.System.VirtualKey.Number5:
                    tabToSelect = 4;
                    break;
                case Windows.System.VirtualKey.Number6:
                    tabToSelect = 5;
                    break;
                case Windows.System.VirtualKey.Number7:
                    tabToSelect = 6;
                    break;
                case Windows.System.VirtualKey.Number8:
                    tabToSelect = 7;
                    break;
                case Windows.System.VirtualKey.Number9:
                    // Select the last tab
                    tabToSelect = InvokedTabView.TabItems.Count - 1;
                    break;
            }

            // Only select the tab if it is in the list
            if (tabToSelect < InvokedTabView.TabItems.Count)
            {
                InvokedTabView.SelectedIndex = tabToSelect;
            }
            args.Handled = true;
        }
        private async void Tabs_TabItemsChanged(TabView sender, IVectorChangedEventArgs args)
        {
            // If there are no more tabs, close the window.
            if (sender.TabItems.Count == 0)
            {
                await ApplicationView.GetForCurrentView().TryConsolidateAsync();
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

            private string _userName;

            public string UserName
            {
                get
                {
                    if (_userName == "DefaultFireBrowserUser") return resourceLoader.GetString("DefaultUserName");
                    else return _userName;
                }
                set { SetProperty(ref _userName, value); }
            }
            [ObservableProperty]
            private string currentAddress;
            [ObservableProperty]
            private string securityIcon;
            [ObservableProperty]
            private Visibility homeButtonVisibility;
        }

        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            CustomDragRegion.MinWidth = (FlowDirection == FlowDirection.LeftToRight) ? sender.SystemOverlayRightInset : sender.SystemOverlayLeftInset;
            CustomDragRegion.Height = sender.Height;
            //ClassicToolbar.Margin = new Thickness(0, CustomDragRegion.Height, 0, 0);
        }
        private void Tabs_Loaded(object sender, RoutedEventArgs e)
        {
            if (Tabs.TabItems.Count == 0) Tabs.TabItems.Add(CreateNewTab());
        }

        private void TabView_AddButtonClick(TabView sender, object args)
        {
            sender.TabItems.Add(CreateNewTab());
            SelectNewTab();
        }

        private void TabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            TabViewItem selectedItem = args.Tab;
            var tabcontent = (Frame)selectedItem.Content;
            if (tabcontent.Content is WebContent)
            {
                (tabcontent.Content as WebContent).WebViewElement.Close();
            }
            sender.TabItems.Remove(args.Tab);
        }
        public CustomTabViewItem CreateNewTab(Type page = null, object param = null, int index = -1)
        {
            if (index == -1) index = Tabs.TabItems.Count;

            CustomTabViewItem newItem = new()
            {
                Header = $"Document {index}",
                IconSource = new Microsoft.UI.Xaml.Controls.SymbolIconSource() { Symbol = Symbol.Document }
            };

         
            Passer passer = new()
            {
                Tab = newItem,
                TabView = Tabs,
                ViewModel = ViewModel,
                Param = param
            };

            if (Settings.currentProfile.UI.FloatingTabs)
            {
                newItem.Style = (Style)Application.Current.Resources["FloatingTabViewItemStyle"];
            }
            else
            {
                newItem.Style = (Style)Application.Current.Resources["DefaultTabViewItemStyle"];
            }
            var contextMenu = (FlyoutBase)Resources["TabContextMenu"];

            newItem.ContextFlyout = contextMenu;
            newItem.ContextFlyout.ShouldConstrainToRootBounds = true;


            // The content of the tab is often a frame that contains a page, though it could be any UIElement.
            double Margin = 0;
            if (Settings.currentProfile.UI.Layout == Settings.UILayout.Classic)
            {
                Margin = ClassicToolbar.Height;
            }
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
        private void TabMenuOnClick(object sender, RoutedEventArgs e)
        {
            switch ((sender as MenuFlyoutItem).Tag)
            {
                case "New-Tab":
                    Tabs.TabItems.Add(CreateNewTab());
                    break;
                case "Edit":

                    break;
                case "Kill":
                    if (TabContent.Content is WebContent)
                    {
                        (TabContent.Content as WebContent).WebViewElement.Close();
                    }
                    Tabs.TabItems.Remove((TabViewItem)Tabs.SelectedItem);
                    break;
                case "Mute":
                    if (TabContent.Content is WebContent)
                    {
                    }
                    break;
                case "Duplicate":
                    if (TabContent.Content is WebContent)
                    {
                        string url = (TabContent.Content as WebContent).WebViewElement.CoreWebView2.Source.ToString();
                        Tabs.TabItems.Add(CreateNewTab(typeof(WebContent), new Uri(url)));
                    }
                    break;
                case "Sleep":
                    if (true)
                    {
                       
                    }
                    else
                    {
                        
                    }
                    break;
            }
        }

        Frame TabContent
        {
            get
            {
                TabViewItem selectedItem = (TabViewItem)Tabs.SelectedItem;
                if (selectedItem != null)
                {
                    return (Frame)selectedItem.Content;
                }
                return null;
            }
        }
        Controls.WebView TabWebView
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
                        TabWebView.Close();
                    }
                    TabContent.Navigate(typeof(NewTab), passer, new DrillInNavigationTransitionInfo());
                    break;
                case "AddFavoriteFlyout":
                    if (TabContent.Content is WebContent)
                    {
                        FavoriteTitle.Text = TabWebView.CoreWebView2.DocumentTitle;
                        FavoriteUrl.Text = TabWebView.CoreWebView2.Source;
                    }
                    break;
                case "AddFavorite":
                    Favorites.AddFavoritesItem(FavoriteTitle.Text, FavoriteUrl.Text);
                    await UI.ShowDialog("Favorite added!", FavoriteTitle.Text);
                    break;
                case "ReadingMode":
                    if (TabContent.Content is WebContent)
                    {
                        StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/JS/simplyread-firebrowser-min.js"));
                        string jscript = await FileIO.ReadTextAsync(file);
                        await (TabContent.Content as WebContent).WebViewElement.CoreWebView2.ExecuteScriptAsync(jscript);
                    }        
                    break;
                case "DownloadFlyout":
                    if (TabContent.Content is WebContent)
                    {
                        (TabContent.Content as WebContent).WebViewElement.CoreWebView2.OpenDefaultDownloadDialog();
                    }
                    break;
                case "Translate":
                    if (TabContent.Content is WebContent)
                    {
                        string url = (TabContent.Content as WebContent).WebViewElement.CoreWebView2.Source.ToString();
                        (TabContent.Content as WebContent).WebViewElement.CoreWebView2.Navigate("https://translate.google.com/translate?hl&u=" + url);
                    }
                    break;
                case "Favorites":
                    LoadFavorites();
                    break;
                case "QRCode":
                    try
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
                    catch
                    {
                        await UI.ShowDialog("Error", "An error occurred while trying to generate your qr code");
                    }
                    break;
            }
        }
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
            return false;
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

        public static async void OpenNewWindow(Uri uri)
        {
            await Windows.System.Launcher.LaunchUriAsync(uri);
        }

        private async void ActionClicked(object sender, RoutedEventArgs e)
        {
            switch ((sender as FrameworkElement)?.Tag)
            {
                case "NewTab":
                    Tabs.TabItems.Add(CreateNewTab());
                    break;
                case "NewIncognitoTab":
                    OpenNewWindow(new Uri("airplane://incognito"));
                    break;
                case "NewWindow":
                    OpenNewWindow(new Uri("airplane://"));
                    break;
                case "Share":
                    var dt = DataTransferManager.GetForCurrentView();
                    dt.DataRequested += (sender, args) =>
                    {
                        DataRequest request = args.Request;
                        if (TabContent.GetType() == typeof(WebContent))
                        {
                            request.Data.SetWebLink(new Uri(TabWebView.CoreWebView2.Source));
                            request.Data.Properties.Title = TabWebView.CoreWebView2.DocumentTitle;
                        }
                        else
                        {
                            request.Data.SetWebLink(new Uri("https://alur.me/go/FireBrowser"));
                            //To-Do: localization
                            request.Data.Properties.Title = "Check out FireBrowser!";
                        }
                    };
                    DataTransferManager.ShowShareUI();
                    break;
                case "Timeline":
                    TabContent.Navigate(typeof(TimelinePage));
                    break;
                case "Focus":
                    TabContent.Navigate(typeof(FocusPage));
                    break;
                case "Print":

                    break;
                case "Note":
                    if (TabContent.Content is WebContent) (TabContent.Content as WebContent).PrepareForNoting();
                    //To-Do for the future: allow noting on non WebView pages
                    break;
                case "FullScreen":
                    ApplicationView view = ApplicationView.GetForCurrentView();
                    bool isfullmode = view.IsFullScreenMode;
                    if (!isfullmode)
                    {
                        view.TryEnterFullScreenMode();
                    }
                    else
                    {
                        view.ExitFullScreenMode();
                    }
                    break;
                case "CompactOverlay":
                    compact();
                    break;
                case "SplitBrowsing":
                    TabContent.Navigate(typeof(SplitBrowsing), CreatePasser(),
                                        new DrillInNavigationTransitionInfo());
                    break;
                case "Freeform":
                    Frame.Navigate(typeof(FreeformBrowsing));
                    break;
                // Tools
                case "DevTools":
                    if (TabContent.Content is WebContent) (TabContent.Content as WebContent).WebViewElement.CoreWebView2.OpenDevToolsWindow();
                    break;
                case "Find":
                    break;
                // Settings and more
                case "Settings":
                    Tabs.TabItems.Add(CreateNewTab(typeof(SettingsPage)));
                    SelectNewTab();
                    break;
                case "Help":

                    break;
                case "About":
                    //DEBUG
                   

                    //TabContent.Navigate(typeof(Pages.Settings.About), CreatePasser(), new DrillInNavigationTransitionInfo());
                    break;
            }
        }

        public async void compact()
        {
            CoreApplicationView newView = CoreApplication.CreateNewView();
            int newViewId = 0;
            await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                Frame frame = new();
                frame.Navigate(typeof(CompactOverlay), null);
                Window.Current.Content = frame;
                // You have to activate the window in order to show it later.
                Window.Current.Activate();
                newViewId = ApplicationView.GetForCurrentView().Id;
                var view = ApplicationView.GetForCurrentView();
                await view.TryEnterViewModeAsync(ApplicationViewMode.CompactOverlay);
            });
            bool viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
        }

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

        private void AirplaneCommands()
        {
            if (UrlBox.Text.Contains("Settings"))
            {
                TabContent.Navigate(typeof(SettingsPage), CreatePasser(),
                                       new DrillInNavigationTransitionInfo());
            }
            if (UrlBox.Text.Contains("Fly?"))
            {
                TabContent.Navigate(typeof(AirplaneFlappy), CreatePasser(),
                                       new DrillInNavigationTransitionInfo());
            }
        }

        private async void TextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            ///to-do
        }

        private void Tabs_TabDroppedOutside(TabView sender, TabViewTabDroppedOutsideEventArgs args) { }

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

        private void TeachingTip_ActionButtonClick(TeachingTip sender, object args)
        {
            (TabContent.Content as WebContent).PermissionChanged(true);
            sender.IsOpen = false;
        }

        private void TeachingTip_CloseButtonClick(TeachingTip sender, object args)
        {
            (TabContent.Content as WebContent).PermissionChanged(false);
        }

        private void Tabs_DragOver(object sender, DragEventArgs e)
        {
            Tabs.ContextFlyout = tabpreview;
        }

        private void UrlBox_GotFocus(object sender, RoutedEventArgs e)
        {
            AutoSuggestBox textBox = sender as AutoSuggestBox;
            // Select all text to allow entering a new search query or url instantly
            //textBox.SelectAll();
        }

        private void NavigateToUrl(Uri uri)
        {
            // If there is no wv2 process running inside a tab, create a new one.
            // Else the current wv2 process will be used to navigate to the url
            if (TabContent.Content is WebContent)
            {
                (TabContent.Content as WebContent).WebViewElement.CoreWebView2.Navigate(uri.ToString());
            }
            else
            {
                TabContent.Navigate(typeof(WebContent), CreatePasser(uri));
            }
        }

        private void SelectNewTab()
        {
            var tabToSelect = Tabs.TabItems.Count - 1;
            Tabs.SelectedIndex = tabToSelect;
        }

        #region AutoSuggestBox

        // Handle text change and present suitable items
        // using System.Xml.Serialization;
        // XmlSerializer serializer = new XmlSerializer(typeof(Toplevel));
        // using (StringReader reader = new StringReader(xml))
        // {
        //    var test = (Toplevel)serializer.Deserialize(reader);
        // }

        public class suggestion
        {

            [XmlAttribute(AttributeName = "data")]
            public string Data { get; set; }
        }

        [XmlRoot(ElementName = "CompleteSuggestion")]
        public class CompleteSuggestion
        {

            public suggestion suggestion { get; set; }
        }

        public class toplevel
        {

            public List<CompleteSuggestion> CompleteSuggestion { get; set; }
        }

        /// <summary>
        /// The Google Suggest search URL.
        /// </summary>
        /// <remarks>
        /// Add gl=dk for Google Denmark. Add lr=lang_da for danish results. Add hl=da to indicate the language of the UI making the request.
        /// </remarks>
        private const string _suggestSearchUrl = "http://www.google.com/complete/search?output=toolbar&q={0}&hl=en";

        /// <summary>
        /// Gets the search suggestions from Google.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>A list of <see cref="GoogleSuggestion"/>s.</returns>
        public async Task<List<GoogleSuggestion>> GetSearchSuggestions(string query = null)
        {
            if (!String.IsNullOrWhiteSpace(query))
            {
                //throw new ArgumentException("Argument cannot be null or empty!", "query");
                string result = String.Empty;

                using (HttpClient client = new HttpClient())
                {
                    try
                    {
                        result = await client.GetStringAsync(String.Format(_suggestSearchUrl, query));
                    }
                    catch
                    {
                        result = string.Empty;
                    }
                }
                try
                {
                    XDocument doc = XDocument.Parse(result);

                    var suggestions = from suggestion in doc.Descendants("CompleteSuggestion")
                                      select new GoogleSuggestion
                                      {
                                          Phrase = suggestion.Element("suggestion").Attribute("data").Value
                                      };

                    return suggestions.ToList();
                }
                catch (Exception ex)
                {
                    Dots.SDK.UWP.Log.WriteLog(ex, "GetSearchSuggestions", Dots.SDK.Log.LogType.Warning);
                    return new List<GoogleSuggestion>();
                }
            }
            else
            {
                return new List<GoogleSuggestion>();
            }
        }

        /// <summary>
        /// Encapsulates a suggestion from Google.
        /// </summary>
        public class GoogleSuggestion
    {
        /// <summary>
        /// Gets or sets the phrase.
        /// </summary>
        /// <value>The phrase.</value>
        public string Phrase { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.Phrase;
        }
    }
     public enum EverythingBoxSuggestionType
        {
            Search,
            Site, //history, favorites, etc
            Weather
        }
        public class EverythingBoxSuggestion
        {
            public EverythingBoxSuggestionType Type { get; set; }
            public Brush Color { get; set; }
            public string Icon { get; set; }
            public string Text { get; set; }
            public object CustomContent { get; set; }
        }
    public async void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            // Since selecting an item will also change the text,
            // only listen to changes caused by user entering text.
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var suitableItems = new List<EverythingBoxSuggestion>();

                //search suggestions
                var searches = await GetSearchSuggestions(sender.Text);
                var splitText = sender.Text.ToLower().Split(" ");
                foreach (var cat in searches)
                {
                    var found = splitText.All((key) =>
                    {
                        return cat.Phrase.ToLower().Contains(key);
                    });
                    if (found)
                    {
                        var newItem = new EverythingBoxSuggestion
                        {
                            Color = new SolidColorBrush(ColorConverter.GetColorFromHex("#006AFF")),
                            Icon = "\uF690",
                            Text = cat.Phrase,
                            Type = EverythingBoxSuggestionType.Search
                        };
                        suitableItems.Add(newItem);

                    }
                }
                if (suitableItems.Count == 0)
                {
                    //suitableItems.Add("No results found");
                }
                sender.ItemsSource = suitableItems;

            }
            else if (args.Reason == AutoSuggestionBoxTextChangeReason.SuggestionChosen)
            {
                //temporary
                return;
            }
        }

        // Handle user selecting an item, in our case just output the selected item.
        private void AutoSuggestBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            var item = args.SelectedItem as EverythingBoxSuggestion;
            sender.Text = item.Text;
        }
        private async void UrlBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (UrlBox.Text.Contains("airplane://"))
            {
                AirplaneCommands();
            }
            else
            {
                // Top level domain list
                StorageFolder appInstalledFolder = Package.Current.InstalledLocation;
                StorageFolder assets = await appInstalledFolder.GetFolderAsync("Assets");
                var file = await assets.GetFileAsync("tldlist.json");
                string filecontent = await FileIO.ReadTextAsync(file);
                string[] knownDomains = JsonSerializer.Deserialize<string[]>(filecontent);

                // If input does not contain a space and contains a valid tld it is a url
                // else airplane will search for query with the selected search engine

                int pos = UrlBox.Text.LastIndexOf(".") + 1;
                string tld = UrlBox.Text.Substring(pos, UrlBox.Text.Length - pos);

                if (UrlBox.Text.Contains(".") && knownDomains.Any(tld.Contains))
                {
                    if (UrlBox.Text.Contains("http://") || UrlBox.Text.Contains("https://"))
                    {
                        NavigateToUrl(new Uri(UrlBox.Text.Trim()));
                    }
                    else
                    {
                        UrlBox.Text = "https://" + UrlBox.Text;
                        NavigateToUrl(new Uri(UrlBox.Text.Trim()));
                    }
                }
                else
                {
                    var settings = await Settings.GetSettingsDataAsync();
                    string searchQuery = settings.Privacy.SearchEngine + sender.Text;
                    NavigateToUrl(new Uri(searchQuery));
                }
            }
            #endregion
        }
        const string DATA_IDENTIFIER = "AirplaneDesktop_Tab";
        private void Tabs_TabDragStarting(TabView sender, TabViewTabDragStartingEventArgs args)
        {
            // We can only drag one tab at a time, so grab the first one...
            var firstItem = args.Tab;

            // ... set the drag data to the tab...
            args.Data.Properties.Add(DATA_IDENTIFIER, firstItem);

            // ... and indicate that we can move it 
            args.Data.RequestedOperation = DataPackageOperation.Move;
        }

        public async void History_Click(object sender, RoutedEventArgs e)
        {
            var historyList = await DataAccess.GetHistoryDetails();
            foreach (var item in historyList)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.ContentTemplate = Application.Current.Resources["SmallHistoryDataTemplate"] as DataTemplate;
                item.ImageSource = new BitmapImage(new Uri("https://t3.gstatic.com/faviconV2?client=SOCIAL&type=FAVICON&fallback_opts=TYPE,SIZE,URL&url=" + item.Url + "&size=16"));
                lvi.DataContext = item;
                lvi.Tag = item.Url;

                // Check if the item already exists in the ListView
                bool itemExists = false;
                foreach (ListViewItem existingItem in SmallHistoryMenu.Items)
                {
                    if (existingItem.Tag.Equals(item.Url))
                    {
                      
                            itemExists = true;
                            break;
                        
                    }
                }

                // If the item does not exist, add it to the ListView
                if (!itemExists)
                {
                    SmallHistoryMenu.Items.Add(lvi);
                    SmallHistoryMenu.AllowFocusOnInteraction = true;
                }
            }

            HistoryFlyoutMenu.ShowAt(History);
            HistoryFlyoutMenu.Placement = FlyoutPlacementMode.TopEdgeAlignedRight;
        }

        private void SmallHistoryMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListViewItem item = SmallHistoryMenu.SelectedItem as ListViewItem;

            if (TabContent.Content is WebContent)
            {
                (TabContent.Content as WebContent).WebViewElement.CoreWebView2.Navigate(item.Tag.ToString());
                SmallHistoryMenu.SelectedItem = false;
            }
        }

        private void HistorySearchMenuItem_TextChanged(object sender, TextChangedEventArgs e)
        {
           
        }

        public void FocusUrlBox(string text)
        {
            UrlBox.Text = text;
            UrlBox.Focus(FocusState.Programmatic);
        }

        public void HideToolbar(bool hide)
        {
            if (hide)
            {
                ClassicToolbar.Visibility = Visibility.Collapsed;
                TabContent.Margin = new Thickness(0, -40, 0, 0);
            }
            else
            {
                ClassicToolbar.Visibility = Visibility.Visible;
                TabContent.Margin = new Thickness(0, 35, 0, 0);
            }
        }

        private List<JsonItems> FavoritesList;
        private async void LoadFavorites()
        {
            FavoritesList = await GetListFromJsonAsync("Favorites.json");
            if (FavoritesList != null)
            {
                FavoritesListView.ItemsSource = FavoritesList;
            }
            else
            {
                await UI.ShowDialog("Error", "Your favorites are empty!");
            }
        }
        private async void FavoritesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Get listview sender
            ListView listView = sender as ListView;
            if (listView.ItemsSource != null)
            {
                // Get selected item
                JsonItems item = (JsonItems)listView.SelectedItem;
                string url = item.Url;
                NavigateToUrl(new Uri(url));
                FavoritesButton.Flyout.Hide();
                FavoritesListView.ItemsSource = null;
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textbox = sender as TextBox;
            // Get all ListView items with the submitted search query
            var SearchResults = from s in FavoritesList where s.Title.Contains(textbox.Text, StringComparison.OrdinalIgnoreCase) select s;
            // Set SearchResults as ItemSource for HistoryListView
            FavoritesListView.ItemsSource = SearchResults;
        }

        private void ClearHistoryDataMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SmallHistoryMenu.Items.Clear();
            DelHistory();
        }

   
        public void DelHistory()
        {
            using (var con = new SqliteConnection($"Data Source={ApplicationData.Current.LocalFolder.Path}/FireBrowserData/{AppData.CurrentProfileCore.FriendlyID}/FireBrowserHistory.Db"))
            {
                con.Open();

                using (var command = new SqliteCommand("DELETE FROM urls", con))
                {
                    command.ExecuteNonQuery();
                }

                con.Close();
            }

        }

        private void OpenHistoryMenuItem_Click(object sender, RoutedEventArgs e)
        {
            TabContent.Navigate(typeof(TimelinePage));
        }
    }
}