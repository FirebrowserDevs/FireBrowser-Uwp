using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using static FireBrowser.MainPage;
using muxc = Microsoft.UI.Xaml.Controls;

namespace FireBrowser.Pages
{
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("SelectedNavViewItemId"))
            {
                string selectedItemId = (string)ApplicationData.Current.LocalSettings.Values["SelectedNavViewItemId"];
                NavView_Navigate(selectedItemId, new EntranceNavigationTransitionInfo()); // Navigate to the previously selected item
            }
        }

        private readonly List<(string Tag, Type Page)> _pages = new List<(string Tag, Type Page)>
        {
            ("SettingsHome", typeof(SettingsPages.Home)),
            ("Privacy", typeof(SettingsPages.Privacy)),
            ("NewTab", typeof(SettingsPages.NewTab)),
            ("Design", typeof(SettingsPages.Design)),
            ("Accessibility", typeof(SettingsPages.Accessibility)),
            ("About", typeof(SettingsPages.About))
        };
        private void NavView_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            // Add handler for ContentFrame navigation.
            ContentFrame.Navigated += On_Navigated;
            var urlBoxText = mainFrame.UrlBox.Text;
            (string navigateToPage, object navigateToItem) = (null, null);

            switch (urlBoxText)
            {
                case string s when s.Contains("firebrowser://settings"):
                    navigateToPage = "SettingsHome";
                    navigateToItem = NavView.MenuItems[0];
                    break;
                case string s when s.Contains("firebrowser://design"):
                    navigateToPage = "Design";
                    navigateToItem = NavView.MenuItems[1];
                    break;
                case string s when s.Contains("firebrowser://privacy"):
                    navigateToPage = "Privacy";
                    navigateToItem = NavView.MenuItems[2];
                    break;
                case string s when s.Contains("firebrowser://newtabset"):
                    navigateToPage = "NewTab";
                    navigateToItem = NavView.MenuItems[3];
                    break;
                case string s when s.Contains("firebrowser://access"):
                    navigateToPage = "Accessibility";
                    navigateToItem = NavView.MenuItems[4];
                    break;
                case string s when s.Contains("firebrowser://about"):
                    navigateToPage = "About";
                    navigateToItem = NavView.MenuItems[5];
                    break;
                default:
                    // Default case
                    break;
            }


            if (navigateToPage != null && navigateToItem != null)
            {
                NavView.SelectedItem = navigateToItem;
                NavView_Navigate(navigateToPage, new Windows.UI.Xaml.Media.Animation.EntranceNavigationTransitionInfo());
            }
            else
            {

                // Set the default selected item to the "Settings" item
                NavView.SelectedItem = NavView.MenuItems[0];
                NavView_Navigate("SettingsHome", new Windows.UI.Xaml.Media.Animation.EntranceNavigationTransitionInfo());
            }


            Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated +=
                CoreDispatcher_AcceleratorKeyActivated;

            Window.Current.CoreWindow.PointerPressed += CoreWindow_PointerPressed;

            SystemNavigationManager.GetForCurrentView().BackRequested += System_BackRequested;
        }

        private void NavView_ItemInvoked(muxc.NavigationView sender, muxc.NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked == true)
            {
                NavView_Navigate("settings", args.RecommendedNavigationTransitionInfo);
            }
            else if (args.InvokedItemContainer != null)
            {
                var navItemTag = args.InvokedItemContainer.Tag.ToString();
                NavView_Navigate(navItemTag, args.RecommendedNavigationTransitionInfo);
            }
        }

        MainPage mainFrame = (MainPage)((Frame)Window.Current.Content).Content;

        private Passer passer;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            passer = e.Parameter as Passer;
            passer.Tab.IconSource = new muxc.FontIconSource()
            {
                Glyph = "\uE713"
            };
        }

        private void NavView_Navigate(
            string navItemTag,
            Windows.UI.Xaml.Media.Animation.NavigationTransitionInfo transitionInfo)
        {
            Type _page = null;
            if (navItemTag == "settings")
            {
                _page = typeof(SettingsPage);
            }
            else
            {
                var item = _pages.FirstOrDefault(p => p.Tag.Equals(navItemTag));
                _page = item.Page;
            }
            // Get the page type before navigation so you can prevent duplicate
            // entries in the backstack.
            var preNavPageType = ContentFrame.CurrentSourcePageType;

            // Only navigate if the selected page isn't currently loaded.
            if (!(_page is null) && !Type.Equals(preNavPageType, _page))
            {
                ContentFrame.Navigate(_page, passer, transitionInfo);
            }
        }

        private void NavView_BackRequested(muxc.NavigationView sender,
                                           muxc.NavigationViewBackRequestedEventArgs args)
        {
            TryGoBack();
        }

        private void CoreDispatcher_AcceleratorKeyActivated(CoreDispatcher sender, AcceleratorKeyEventArgs e)
        {
            // When Alt+Left are pressed navigate back
            if (e.EventType == CoreAcceleratorKeyEventType.SystemKeyDown
                && e.VirtualKey == VirtualKey.Left
                && e.KeyStatus.IsMenuKeyDown == true
                && !e.Handled)
            {
                e.Handled = TryGoBack();
            }
        }

        private void System_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = TryGoBack();
            }
        }

        private void CoreWindow_PointerPressed(CoreWindow sender, PointerEventArgs e)
        {
            // Handle mouse back button.
            if (e.CurrentPoint.Properties.IsXButton1Pressed)
            {
                e.Handled = TryGoBack();
            }
        }

        private bool TryGoBack()
        {
            if (!ContentFrame.CanGoBack)
                return false;

            // Don't go back if the nav pane is overlayed.
            if (NavView.IsPaneOpen &&
                (NavView.DisplayMode == muxc.NavigationViewDisplayMode.Compact ||
                 NavView.DisplayMode == muxc.NavigationViewDisplayMode.Minimal))
                return false;

            ContentFrame.GoBack();
            return true;
        }

        private void On_Navigated(object sender, NavigationEventArgs e)
        {
            NavView.IsBackEnabled = ContentFrame.CanGoBack;

            if (ContentFrame.SourcePageType != null)
            {
                var item = _pages.FirstOrDefault(p => p.Page == e.SourcePageType);


                NavView.Header =
                    ((muxc.NavigationViewItem)NavView.SelectedItem)?.Content?.ToString();
                passer.Tab.Header = ((muxc.NavigationViewItem)NavView.SelectedItem)?.Content?.ToString();
            }
        }


        private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }
    }
}
