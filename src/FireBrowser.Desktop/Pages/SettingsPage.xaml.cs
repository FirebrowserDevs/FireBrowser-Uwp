using System.Collections.Generic;
using System.Linq;
using muxc = Microsoft.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Core;
using Windows.System;
using static FireBrowser.MainPage;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace FireBrowser.Pages
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
    /// </summary>
    /// 
    //To-Do: whenever the user navigates to a different page, save the settings data
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            this.InitializeComponent();

        }
        private double NavViewCompactModeThresholdWidth { get { return NavView.CompactModeThresholdWidth; } }

        private void ContentFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        // List of ValueTuple holding the Navigation Tag and the relative Navigation Page
        private readonly List<(string Tag, Type Page)> _pages = new List<(string Tag, Type Page)>
        {
            ("SettingsHome", typeof(Settings.Home)),
            ("SettingsLook", typeof(Settings.Look)),
            ("Privacy", typeof(Settings.Privacy)),
            ("NewTab", typeof(Settings.NewTab)),
            ("Design", typeof(Settings.Design)),
            ("Accessibility", typeof(Settings.Accessibility)),
            ("About", typeof(Settings.About))
        };

     
        private void NavView_Loaded(object sender, RoutedEventArgs e)
        {
            // Add handler for ContentFrame navigation.
            ContentFrame.Navigated += On_Navigated;

            // NavView doesn't load any page by default, so load home page.
            NavView.SelectedItem = NavView.MenuItems[0];
            // If navigation occurs on SelectionChanged, this isn't needed.
            // Because we use ItemInvoked to navigate, we need to call Navigate
            // here to load the home page.
            NavView_Navigate("SettingsHome", new Windows.UI.Xaml.Media.Animation.EntranceNavigationTransitionInfo());

            // Listen to the window directly so the app responds
            // to accelerator keys regardless of which element has focus.
            Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated +=
                CoreDispatcher_AcceleratorKeyActivated;

            Window.Current.CoreWindow.PointerPressed += CoreWindow_PointerPressed;

            SystemNavigationManager.GetForCurrentView().BackRequested += System_BackRequested;
        }

        private void NavView_ItemInvoked(muxc.NavigationView sender,
                                         muxc.NavigationViewItemInvokedEventArgs args)
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
        private Passer passer;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            passer = e.Parameter as Passer;
            passer.Tab.IconSource = new muxc.FontIconSource()
            {
                Glyph = "\uF6AA"
            };
        }

        // NavView_SelectionChanged is not used in this example, but is shown for completeness.
        // You will typically handle either ItemInvoked or SelectionChanged to perform navigation,
        // but not both.
        private void NavView_SelectionChanged(muxc.NavigationView sender,
                                              muxc.NavigationViewSelectionChangedEventArgs args)
        {

            if (args.IsSettingsSelected == true)
            {
                NavView_Navigate("settings", args.RecommendedNavigationTransitionInfo);
            }
            else if (args.SelectedItemContainer != null)
            {
                var navItemTag = args.SelectedItemContainer.Tag.ToString();
                NavView_Navigate(navItemTag, args.RecommendedNavigationTransitionInfo);
            }
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

    }
}
