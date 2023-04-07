using FireBrowser.Pages.SettingsPages;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static FireBrowser.App;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FireBrowser.Launch
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SetupStep2 : Page
    {
        public SetupStep2()
        {
            this.InitializeComponent();
            FireBrowserInterop.SettingsHelper.SetSetting("LightMode", "0");
        }

        private void LgMode_Toggled(object sender, RoutedEventArgs e)
        {
            FireBrowserInterop.SettingsHelper.SetSetting("LightMode", LgMode.IsOn ? "1" : "0");
        }

        private void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {
            MsLogin ms = new MsLogin();
            ms.ShowAsync();
        }

        private async void Install_Click(object sender, RoutedEventArgs e)
        {
            bool isFirstLaunch = true;

            var settingsFile = await ApplicationData.Current.LocalFolder.GetFileAsync("Params.json");
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
    }
}
