using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using JsonSerializer = System.Text.Json.JsonSerializer;

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
            public List<Profile> Profiles { get; set; }
        }

        public class Profile
        {
            public Accountdata AccountData { get; set; }
            public UISettings UI { get; set; }
            public Sleepingtabs SleepingTabs { get; set; }
            public Privacy Privacy { get; set; }
            public Other Other { get; set; }
            public List<UIPin> ToolbarPins { get; set; }
            public Newtab NewTab { get; set; }
            public bool DebugSettingsEnabled { get; set; }
            public List<UIPin> FlyoutPins { get; set; }
        }

        public class Accountdata
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public string Token { get; set; }
            public AppData.CachableImage ProfilePic { get; set; }
            public bool FinishedFirstLaunch { get; set; }
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
        }

        public class Sleepingtabs
        {
            public bool IsEnabled { get; set; }
            public int PutTabsToSleepAfter { get; set; }
            public List<Uri> Blocklist { get; set; }
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
            public Core.Models.Settings.NewTabBackground NewTabBackground { get; set; }
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
            ResourceLoader res = ResourceLoader.GetForCurrentView();
            string _res;

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
        public static async Task<Profile> GetSettingsDataAsync()
        {
            var userFolder = await AppData.GetUserFolder();
            var fileData = await userFolder.GetFileAsync("UserSettings.json");

            try
            {
                var json = JsonSerializer.Deserialize<Profile>(File.ReadAllText(fileData.Path), AppData.serializerOptions);
                return json;
            }
            catch (Exception ex)
            {
                //To-Do: Copy over DefaultData and write in logs    
                return null;
            }
        }
        /// <summary>
        /// LEGACY, DO NOT USE UNLESS YOU CANNOT USE THE ASYNC VERSION
        /// </summary>
        public static Profile currentProfile = JsonSerializer.Deserialize<Profile>(File.ReadAllText($"{ApplicationData.Current.LocalFolder.Path}\\FireBrowserData\\{AppData.CurrentProfileCore.FriendlyID}\\UserSettings.json"), AppData.serializerOptions);
    }
}
