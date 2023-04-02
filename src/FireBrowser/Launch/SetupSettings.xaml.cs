using FireBrowser.Pages.SettingsPages;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using static FireBrowser.App;

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
            checkcombobox();
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
            checkcombobox();
        }

        private void SetEngine(string EngineFriendlyName, string SearchUrl)
        {
            FireBrowserInterop.SettingsHelper.SetSetting("EngineFriendlyName", EngineFriendlyName);
            FireBrowserInterop.SettingsHelper.SetSetting("SearchUrl", SearchUrl);
        }

        private void checkcombobox()
        {
            Thread.Sleep(100);
            if (!string.IsNullOrEmpty(SearchengineSelection.SelectedItem?.ToString()) && !string.IsNullOrEmpty(Background.SelectedItem?.ToString()))
            {
                Install.IsEnabled = true;
            }
            else
            {
                Install.IsEnabled = false;
            }
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
            CreateDatabase();

            FireBrowserInterop.SettingsHelper.SetSetting("DisableJavaScript", "false");
            FireBrowserInterop.SettingsHelper.SetSetting("DisableGenAutoFill", "false");
            FireBrowserInterop.SettingsHelper.SetSetting("DisableWebMess", "false");
            FireBrowserInterop.SettingsHelper.SetSetting("DisablePassSave", "false");
            FireBrowserInterop.SettingsHelper.SetSetting("Auto", "0");

            bool isFirstLaunch = true;

            var settingsFile = await ApplicationData.Current.LocalFolder.GetFileAsync("Isettings.json");
            string settingsJson = await FileIO.ReadTextAsync(settingsFile);
            AppSettings settings = JsonConvert.DeserializeObject<AppSettings>(settingsJson);

            if (settings != null && settings.IsFirstLaunch)
            {
                // The app has been launched before, but this is the first launch after an update
                isFirstLaunch = true;
                settings.IsFirstLaunch = false; // Set the IsFirstLaunch property to false
                string updatedSettingsJson = JsonConvert.SerializeObject(settings);
                await FileIO.WriteTextAsync(settingsFile, updatedSettingsJson); // Save the updated settings
            }
            FireBrowserInterop.SystemHelper.RestartApp();
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
            checkcombobox();
        }

        private void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {
            MsLogin ms = new MsLogin();
            ms.ShowAsync();
        }

        private void LgMode_Toggled(object sender, RoutedEventArgs e)
        {
            FireBrowserInterop.SettingsHelper.SetSetting("LightMode", LgMode.IsOn ? "1" : "0");
        }
    }
}
