using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using static FireBrowser.MainPage;
using muxc = Microsoft.UI.Xaml.Controls;

namespace FireBrowser.Pages.TimeLine
{
    public sealed partial class Timeline : Page
    {
        public Timeline()
        {
            this.InitializeComponent();
        }

        private readonly List<(string Tag, Type Page)> _pages = new()
        {
             ("history", typeof(TimeLine.HistoryTime)),
             ("apps", typeof(TimeLine.Apps)),
             ("favorites", typeof(TimeLine.Favorites))
        };

        private void NavigationView_Loaded(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigated += On_Navigated;

            var urlBoxText = Core.UseContent.MainPageContent.UrlBox.Text;
            var navigateTo = urlBoxText switch
            {
                string s when s.Contains("firebrowser://apps") => ("apps", NavigationView.MenuItems[0]),
                string s when s.Contains("firebrowser://history") => ("history", NavigationView.MenuItems[1]),
                string s when s.Contains("firebrowser://favorites") => ("favorites", NavigationView.MenuItems[2]),
                _ => (null, null),// default case
            };
            if (navigateTo.Item1 != null && navigateTo.Item2 != null)
            {
                NavigationView.SelectedItem = navigateTo.Item2;
                NavigationView_Navigate(navigateTo.Item1, new Windows.UI.Xaml.Media.Animation.EntranceNavigationTransitionInfo());
            }
            else
            {
                NavigationView.SelectedItem = NavigationView.MenuItems[0];
                NavigationView_Navigate("apps", new Windows.UI.Xaml.Media.Animation.EntranceNavigationTransitionInfo());
            } // Default behavior

            // Listen to the window directly so the app responds
            // to accelerator keys regardless of which element has focus.
            Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated +=
                CoreDispatcher_AcceleratorKeyActivated;

            Window.Current.CoreWindow.PointerPressed += CoreWindow_PointerPressed;

            SystemNavigationManager.GetForCurrentView().BackRequested += System_BackRequested;
        }

        private void NavigationView_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked == true)
            {
                NavigationView_Navigate("history", args.RecommendedNavigationTransitionInfo);
            }
            else if (args.InvokedItemContainer != null)
            {
                var navItemTag = args.InvokedItemContainer.Tag.ToString();
                NavigationView_Navigate(navItemTag, args.RecommendedNavigationTransitionInfo);
            }
        }

        private void NavigationView_SelectionChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            if (args.IsSettingsSelected == true)
            {
                NavigationView_Navigate("history", args.RecommendedNavigationTransitionInfo);
            }
            else if (args.SelectedItemContainer != null)
            {
                var navItemTag = args.SelectedItemContainer.Tag.ToString();
                NavigationView_Navigate(navItemTag, args.RecommendedNavigationTransitionInfo);
                Core.UseContent.MainPageContent.UrlBox.Text = "firebrowser://" + navItemTag.ToString();
            }
        }


        private void NavigationView_Navigate(
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
                ContentFrame.Navigate(_page, transitionInfo);
            }
        }

        private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
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
            if (NavigationView.IsPaneOpen &&
                (NavigationView.DisplayMode == muxc.NavigationViewDisplayMode.Compact ||
                 NavigationView.DisplayMode == muxc.NavigationViewDisplayMode.Minimal))
                return false;

            ContentFrame.GoBack();
            return true;
        }

        private void On_Navigated(object sender, NavigationEventArgs e)
        {
            NavigationView.IsBackEnabled = ContentFrame.CanGoBack;

            if (ContentFrame.SourcePageType != null)
            {
                var item = _pages.FirstOrDefault(p => p.Page == e.SourcePageType);
            }
        }
    }
}
