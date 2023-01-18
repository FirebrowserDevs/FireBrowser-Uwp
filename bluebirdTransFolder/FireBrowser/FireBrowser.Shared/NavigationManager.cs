using System;
using Windows.UI.Core;

namespace FireBrowser.Shared
{
    public class NavigationManager
    {
        // EventHandler for back button
        public static EventHandler GoBackHandler;

        // Shows or hides the back button in the title bar on desktop
        public static void ShowDesktopBackButton(bool show)
        {
            if (show)
            {
                SystemNavigationManager currentView = SystemNavigationManager.GetForCurrentView();
                currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            }
            else
            {
                SystemNavigationManager currentView = SystemNavigationManager.GetForCurrentView();
                currentView.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            }
        }
    }
}
