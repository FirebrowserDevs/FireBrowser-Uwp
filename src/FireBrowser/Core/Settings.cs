﻿using System.Collections.Generic;
using Windows.ApplicationModel.Resources;

namespace FireBrowser.Core
{
    public class Settings
    {
      
        public enum FavoritesBarVisibility
        {
            Always,
            NewTabOnly,
            Never
        }
        public enum UILayout
        {
            Classic,
            Compact,
            Vertical
        }
        public enum NewTabLayout
        {
            Classic,
            Simple,
            Productive
        }
        public enum NewTabBackground
        {
            None,
            Custom,
            Featured //Bing for now, in the future Unsplash or our own service
        }
        public enum FlyoutLayout
        {
            Compact,
            Normal
        }

        public class SettingsBase
        {
            public float AppVersion { get; set; }
            public int DefaultProfile { get; set; }
        }

        public class UISettings
        {
            public UILayout Layout { get; set; }
            public FlyoutLayout FlyoutLayout { get; set; }
            public bool FloatingTabs { get; set; }
            public string BaseTheme { get; set; }
            public int Theme { get; set; }
            public string ShowFavoritesBar { get; set; }
            public bool ShowCloseAllDialog { get; set; }
            public bool ShowToolbarUserButton { get; set; }
            public bool ShowSetTabsAsideButton { get; set; }
            public bool ShowWindowActionButton { get; set; }
            public bool ShowTabPreviewOnHover { get; set; }
            public bool ShowZoomSlider { get; set; }
            public List<UIPin> FlyoutPins { get; set; }
        }
        public class Sleepingtabs
        {
            public bool IsEnabled { get; set; }
            public int PutTabsToSleepAfter { get; set; }
        }

        public class Privacy
        {
            public bool ClearCookiesOnEveryLaunch { get; set; }
            public bool DoNotTrack { get; set; }
            public bool StoreHistory { get; set; }
            public bool SavePasswords { get; set; }
            public string SearchEngine { get; set; }
            public bool WebSearchSuggestions { get; set; }
        }

        public class Other
        {
            public bool AskBeforeDownloading { get; set; }
            public int DefaultZoomLevel { get; set; }
        }

        public class Newtab
        {
            public NewTabLayout NewTabLayout { get; set; }
            public string CustomNewTab { get; set; }
            public int PinAmount { get; set; }
            public bool ShowSearchSuggestions { get; set; }
        }
        public enum UIPinKind
        {
            Native,
            Web,
            Custom
        }
        //To-Do: UIPin kind, aka Native or Web or Custom (custom is extension)
        public class UIPin
        {
            public string Icon { get; set; }
            public string ID { get; set; }
            public string TextResource { get; set; }
            //Only used for 3rd party extensions that don't have a built in resource
            public LocalizedText TextLocalized { get;set;}
        }
        //Ignore for now
        public class LocalizedText
        {
            public string Default { get; set; }
            public string en { get; set; }
        }
    }
}
