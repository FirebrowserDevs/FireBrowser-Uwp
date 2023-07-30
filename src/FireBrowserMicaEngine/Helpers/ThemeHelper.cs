﻿using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;
using Windows.UI.WindowManagement;
using Windows.UI.Xaml;
using Windows.UI;

namespace FireBrowserMicaEngine.Helpers
{
    public static class ThemeHelper
    {
        private static Window CurrentApplicationWindow;

        // Keep reference so it does not get optimized/garbage collected
        public static UISettings UISettings;

        public static ElementTheme actualTheme;

        /// <summary>
        /// Gets the current actual theme of the app based on the requested theme of the
        /// root element, or if that value is Default, the requested theme of the Application.
        /// </summary>
        public static ElementTheme ActualTheme => CurrentApplicationWindow != null
            && CurrentApplicationWindow.Dispatcher.HasThreadAccess
            && CurrentApplicationWindow.Content is FrameworkElement rootElement
            && rootElement.RequestedTheme != ElementTheme.Default
                ? rootElement.RequestedTheme
                : actualTheme;

        /// <summary>
        /// Gets or sets (with LocalSettings persistence) the RequestedTheme of the root element.
        /// </summary>
        public static ElementTheme RootTheme
        {
            get => CurrentApplicationWindow != null
                && CurrentApplicationWindow.Content is FrameworkElement rootElement
                    ? rootElement.RequestedTheme
                    : ElementTheme.Default;
            set
            {
                if (CurrentApplicationWindow == null) { return; }
                UpdateFrameworkElementRequestedTheme(value);
                UpdateSystemCaptionButtonColors();
                actualTheme = value;
            }
        }

        public static void Initialize()
        {
            // Save reference as this might be null when the user is in another app
            CurrentApplicationWindow = Window.Current;

            // Registering to color changes, thus we notice when user changes theme system wide
            UISettings = new UISettings();
            UISettings.ColorValuesChanged += UISettings_ColorValuesChanged;

            UpdateSystemCaptionButtonColors();
        }

        private static void UISettings_ColorValuesChanged(UISettings sender, object args) => UpdateSystemCaptionButtonColors();

        public static bool IsDarkTheme()
        {
            return Window.Current != null
                ? ActualTheme == ElementTheme.Default
                    ? Application.Current.RequestedTheme == ApplicationTheme.Dark
                    : ActualTheme == ElementTheme.Dark
                : ActualTheme == ElementTheme.Default
                    ? UISettings.GetColorValue(UIColorType.Background) == Colors.Black
                    : ActualTheme == ElementTheme.Dark;
        }

        public static async void UpdateFrameworkElementRequestedTheme(ElementTheme theme)
        {
            if (!CurrentApplicationWindow.Dispatcher.HasThreadAccess)
            {
                await CurrentApplicationWindow.Dispatcher.ResumeForegroundAsync();
            }

            if (CurrentApplicationWindow.Content is FrameworkElement rootElement)
            {
                rootElement.RequestedTheme = theme;
            }

            if (WindowHelper.IsSupportedAppWindow)
            {
                foreach (FrameworkElement element in WindowHelper.ActiveWindows.Keys)
                {
                    element.RequestedTheme = theme;
                }
            }
        }

        public static async void UpdateSystemCaptionButtonColors()
        {
            bool IsDark = IsDarkTheme();
            bool IsHighContrast = new AccessibilitySettings().HighContrast;

            Color ForegroundColor = IsDark || IsHighContrast ? Colors.White : Colors.Black;
            Color BackgroundColor = IsHighContrast ? Color.FromArgb(255, 0, 0, 0) : IsDark ? Color.FromArgb(255, 32, 32, 32) : Color.FromArgb(255, 243, 243, 243);

            if (!CurrentApplicationWindow.Dispatcher.HasThreadAccess)
            {
                await CurrentApplicationWindow.Dispatcher.ResumeForegroundAsync();
            }

            if (UIHelper.HasStatusBar)
            {

            }
            else
            {
                bool ExtendViewIntoTitleBar = CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar;
                ApplicationViewTitleBar TitleBar = ApplicationView.GetForCurrentView().TitleBar;
                TitleBar.ForegroundColor = TitleBar.ButtonForegroundColor = ForegroundColor;
                TitleBar.BackgroundColor = TitleBar.InactiveBackgroundColor = BackgroundColor;
                TitleBar.ButtonBackgroundColor = TitleBar.ButtonInactiveBackgroundColor = ExtendViewIntoTitleBar ? Colors.Transparent : BackgroundColor;
            }

            if (WindowHelper.IsSupportedAppWindow)
            {
                foreach (AppWindow window in WindowHelper.ActiveWindows.Values)
                {
                    bool ExtendViewIntoTitleBar = window.TitleBar.ExtendsContentIntoTitleBar;
                    AppWindowTitleBar TitleBar = window.TitleBar;
                    TitleBar.ForegroundColor = TitleBar.ButtonForegroundColor = ForegroundColor;
                    TitleBar.BackgroundColor = TitleBar.InactiveBackgroundColor = BackgroundColor;
                    TitleBar.ButtonBackgroundColor = TitleBar.ButtonInactiveBackgroundColor = ExtendViewIntoTitleBar ? Colors.Transparent : BackgroundColor;
                }
            }
        }
    }

}
