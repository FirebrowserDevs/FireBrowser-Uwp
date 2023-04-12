using FireBrowser.Pages.SettingsPages;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Windows.ApplicationModel.Store;
using System;
using System.IO;
using System.Net.NetworkInformation;
using System.Threading;
using Windows.Storage;
using Windows.Storage.AccessCache;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using static FireBrowser.App;
using Windows.Devices.Geolocation;
using Windows.UI.Popups;
using Windows.Media.Capture;
using Windows.UI.Core;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FireBrowser.Launch
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SetupSettings : Page
    {
        public SetupSettings()
        {
            this.InitializeComponent();
            FireBrowserInterop.SettingsHelper.SetSetting("DisableJavaScript", "false");
            FireBrowserInterop.SettingsHelper.SetSetting("DisablePassSave", "false");
            FireBrowserInterop.SettingsHelper.SetSetting("DisableWebMess", "false");
            FireBrowserInterop.SettingsHelper.SetSetting("DisableGenAutoFill", "false");
        }


        private void SearchengineSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selection = e.AddedItems[0].ToString();
            if (selection == "Ask") SetEngine("Ask", "https://www.ask.com/web?q=");
            if (selection == "Baidu") SetEngine("Baidu", "https://www.baidu.com/s?ie=utf-8&f=8&rsv_bp=1&rsv_idx=1&tn=baidu&wd=");
            if (selection == "Bing") SetEngine("Bing", "https://www.bing.com?q=");
            if (selection == "DuckDuckGo") SetEngine("DuckDuckGo", "https://www.duckduckgo.com?q=");
            if (selection == "Ecosia") SetEngine("Ecosia", "https://www.ecosia.org/search?q=");
            if (selection == "Google") SetEngine("Google", "https://www.google.com/search?q=");
            if (selection == "Startpage") SetEngine("Startpage", "https://www.startpage.com/search?q=");
            if (selection == "Qwant") SetEngine("Qwant", "https://www.qwant.com/?q=");
            if (selection == "Qwant Lite") SetEngine("Qwant Lite", "https://lite.qwant.com/?q=");
            if (selection == "Yahoo!") SetEngine("Yahoo!", "https://search.yahoo.com/search?p=");
            if (selection == "Presearch") SetEngine("Presearch", "https://presearch.com/search?q=");
        }

        private void SetEngine(string EngineFriendlyName, string SearchUrl)
        {
            FireBrowserInterop.SettingsHelper.SetSetting("EngineFriendlyName", EngineFriendlyName);
            FireBrowserInterop.SettingsHelper.SetSetting("SearchUrl", SearchUrl);
        }


        public void setdefault()
        {
            FireBrowserInterop.SettingsHelper.SetSetting("Auto", "0");
        }

        public async void CreateDatabase()
        {
            await ApplicationData.Current.LocalFolder.CreateFileAsync("History.db", CreationCollisionOption.OpenIfExists);

            var connectionStringBuilder = new SqliteConnectionStringBuilder();
            connectionStringBuilder.DataSource = Path.Combine(ApplicationData.Current.LocalFolder.Path, "History.db");

            using (var db = new SqliteConnection(connectionStringBuilder.ConnectionString))
            {
                db.Open();

                string tableCommand = "CREATE TABLE IF NOT " +
                     "EXISTS urlsDb (Url NVARCHAR(2083) PRIMARY KEY NOT NULL, " +
                     "Title NVARCHAR(2048), " +
                     "Visit_Count INTEGER, " +
                     "Last_Visit_Time DATETIME)";


                SqliteCommand createTable = new SqliteCommand(tableCommand, db);

                createTable.ExecuteReader();
            }
        }
        private async void Install_Click(object sender, RoutedEventArgs e)
        {       
            setdefault();
            CreateDatabase();
            Content.Navigate(typeof(SetupStep2));
        }


        private void Read_Toggled(object sender, RoutedEventArgs e)
        {
            FireBrowserInterop.SettingsHelper.SetSetting("Readbutton", Read.IsOn ? "True" : "0");
        }

        private void Adbl_Toggled(object sender, RoutedEventArgs e)
        {
            FireBrowserInterop.SettingsHelper.SetSetting("AdBtn", Adbl.IsOn ? "True" : "0");
        }

        private void Dwbl_Toggled(object sender, RoutedEventArgs e)
        {
            FireBrowserInterop.SettingsHelper.SetSetting("DwBtn", Dwbl.IsOn ? "True" : "0");
        }

        private void Trbl_Toggled(object sender, RoutedEventArgs e)
        {
            FireBrowserInterop.SettingsHelper.SetSetting("TransBtn", Trbl.IsOn ? "True" : "0");
        }

        private void Frbl_Toggled(object sender, RoutedEventArgs e)
        {
            FireBrowserInterop.SettingsHelper.SetSetting("FavBtn", Frbl.IsOn ? "True" : "0");
        }

        private void Hsbl_Toggled(object sender, RoutedEventArgs e)
        {
            FireBrowserInterop.SettingsHelper.SetSetting("HisBtn", Hsbl.IsOn ? "True" : "0");
        }

        private void Qrbl_Toggled(object sender, RoutedEventArgs e)
        {
            FireBrowserInterop.SettingsHelper.SetSetting("QrBtn", Qrbl.IsOn ? "True" : "0");
        }

        private void Background_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selection = e.AddedItems[0].ToString();
            if (selection == "Default") FireBrowserInterop.SettingsHelper.SetSetting("Background", "0");
            if (selection == "Featured") FireBrowserInterop.SettingsHelper.SetSetting("Background", "1");
        }
   
        private async void Permissions_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new MessageDialog("This app needs access to your location to function properly. Do you want to allow location access?");
            dialog.Commands.Add(new UICommand("Allow", async (command) =>
            {
                // Request permission to access location
                var accessStatus = await Geolocator.RequestAccessAsync();
                if (accessStatus == GeolocationAccessStatus.Allowed)
                {
                    // Location access granted
                }
            }));
            dialog.Commands.Add(new UICommand("Deny", (command) =>
            {
                // Location access denied
            }));
            await dialog.ShowAsync();
        }

        private void Javascript_Toggled(object sender, RoutedEventArgs e)
        {
            FireBrowserInterop.SettingsHelper.SetSetting("DisableJavaScript", Javascript.IsOn ? "true" : "false");
        }

        private void AutoFillGen_Toggled(object sender, RoutedEventArgs e)
        {
            FireBrowserInterop.SettingsHelper.SetSetting("DisableGenAutoFill", AutoFillGen.IsOn ? "true" : "false");
        }

        private void Messages_Toggled(object sender, RoutedEventArgs e)
        {
            FireBrowserInterop.SettingsHelper.SetSetting("DisableWebMess", Messages.IsOn ? "true" : "false");
        }

        private void Passwords_Toggled(object sender, RoutedEventArgs e)
        {
            FireBrowserInterop.SettingsHelper.SetSetting("DisablePassSave", Passwords.IsOn ? "true" : "false");
        }
    }
}
