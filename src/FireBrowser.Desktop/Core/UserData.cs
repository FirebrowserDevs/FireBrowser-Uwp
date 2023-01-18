using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Text.Json;
using Windows.Storage;

namespace FireBrowser.Core
{
    public class UserData
    {
        public enum FocusMode
        {
            AllowAll,
            BlockAll
        }
        public enum FavoriteIconType
        {
            Favicon,
            Folder
        }
        public class UserDataBase : ObservableObject
        {
            public float AppVersion { get; set; }
            public List<Profile> Profiles { get; set; }
        }
        public class FavoriteIcon
        {
            public FavoriteIconType Type { get; set; }
            public AppData.CachableImage? Favicon { get; set; }
        }

        public class Profile
        {
            public Favorite[] Favorites { get; set; }
            public Collection[] Collections { get; set; }
            public Installedapp[] InstalledApps { get; set; }
            public List<Site> NewTabPins { get; set; }
            public List<FocusList> Focus { get; set; }
        }

        public class Favorite
        {
            public string Name { get; set; }
            public AppData.CachableImage Favicon { get; set; }
            public string URL { get; set; }
            public List<Site> Content { get; set; }
        }

        public class FocusList
        {
            public string Name { get; set; }
            public string Icon { get; set; }
            public FocusMode Mode { get; set; }
            public List<Uri> SiteList { get; set; }
        }

        public class Collection
        {
            public string Name { get; set; }
            public string Icon { get; set; }
            public List<Content> Content { get; set; }
        }

        public class Content
        {
            public string Type { get; set; }
            public object ItemContent { get; set; }
        }

        public class CollectionNote
        {
            public string Text { get; set; }
            public string Color { get; set; }
        }

        public class Tabsaside
        {
            public string Name { get; set; }
            public List<Site> Content { get; set; }
        }

        public class Site
        {
            public string Name { get; set; }
            public AppData.CachableImage Favicon { get; set; }
            public Uri URL { get; set; }
        }

        public class Installedapp
        {
            public string Name { get; set; }
            public string Icon { get; set; }
            public string URL { get; set; }
            public bool Pinned { get; set; }
        }
        //public static UserDataBase appSettings = JsonSerializer.Deserialize<UserDataBase>(File.ReadAllText($"{Windows.Storage.ApplicationData.Current.LocalFolder.Path}/FireBrowserData/UserData.json"), AppData.options);
        //Slower, do not use unless required
        public static Profile currentProfile = JsonSerializer.Deserialize<Profile>(File.ReadAllText($"{ApplicationData.Current.LocalFolder.Path}\\FireBrowserData\\{AppData.CurrentProfileCore.FriendlyID}\\UserData.json"), AppData.serializerOptions);

        public static void SetTabsAside(TabView tabView)
        {
            var TabsAside = new Tabsaside();
            int tabsAsideCount = 0;
            foreach (var item in tabView.TabItems)
            {
                var tabViewItem = item as TabViewItem;
                var frame = tabViewItem.Content as Frame;
                if (frame.Content is Pages.WebContent)
                {
                    tabsAsideCount++;

                    var webContent = frame.Content as Pages.WebContent;

                    var TabAside = new Site
                    {
                        Name = webContent.WebViewElement.CoreWebView2.DocumentTitle,
                        URL = new Uri(webContent.WebViewElement.CoreWebView2.Source)
                    };
                    TabsAside.Content.Add(TabAside);
                }
                tabView.TabItems.Remove(tabViewItem);
            }
            //string title = (tabsAsideCount > 1) ? resourceLoader.GetString("TabsAsideDefaultTitleMultiple") : title = resourceLoader.GetString("TabsAsideDefaultTitleOne");

            TabsAside.Name = "Hello World";

            //currentProfile.TabsAside.Add(TabsAside);
        }
    }
}
