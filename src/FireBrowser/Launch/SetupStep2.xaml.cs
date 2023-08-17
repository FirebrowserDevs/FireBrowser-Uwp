using FireBrowser.Pages.SettingsPages;
using Newtonsoft.Json;
using System;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FireBrowser.Launch
{
    public sealed partial class SetupStep2 : Page
    {
        public SetupStep2()
        {
            this.InitializeComponent();
            FireBrowserInterop.SettingsHelper.SetSetting("LightMode", "0");
            FireBrowserInterop.SettingsHelper.SetSetting("OpSw", "True");
        }

        private void LgMode_Toggled(object sender, RoutedEventArgs e)
        {
            FireBrowserInterop.SettingsHelper.SetSetting("LightMode", LgMode.IsOn ? "1" : "0");
        }

        private async void ConnectBtn_Click(object sender, RoutedEventArgs e)
        {
            MsLogin ms = new MsLogin();
            await ms.ShowAsync();
        }

        private async void Install_Click(object sender, RoutedEventArgs e)
        {
            bool isFirstLaunch = true;

            var settingsFile = await ApplicationData.Current.LocalFolder.GetFileAsync("Params.json");
            string settingsJson = await FileIO.ReadTextAsync(settingsFile);
            FireBrowserCore.Overlay.AppOverlay.AppSettings settings = JsonConvert.DeserializeObject<FireBrowserCore.Overlay.AppOverlay.AppSettings>(settingsJson);

            if (settings != null && settings.IsFirstLaunch)
            {
                // The app has been launched before, but this is the first launch after an update
                isFirstLaunch = true;
                settings.IsFirstLaunch = false; // Set the IsFirstLaunch property to false
                string updatedSettingsJson = JsonConvert.SerializeObject(settings);
                await FileIO.WriteTextAsync(settingsFile, updatedSettingsJson); // Save the updated settings
            }

            if (tbv.Text.Equals("#000000"))
            {
                FireBrowserInterop.SettingsHelper.SetSetting("ColorTool", "#000000");
            }
            if (tbc.Text.Equals("#000000"))
            {
                FireBrowserInterop.SettingsHelper.SetSetting("ColorTV", "#000000");
            }

            await FireBrowserInterop.SystemHelper.RestartApp();
        }

        private void tbv_TextChanged(object sender, TextChangedEventArgs e)
        {
            var value = tbv.Text;
            FireBrowserInterop.SettingsHelper.SetSetting("ColorTool", value);
        }

        private void tbc_TextChanged(object sender, TextChangedEventArgs e)
        {
            var value = tbc.Text;
            FireBrowserInterop.SettingsHelper.SetSetting("ColorTV", value);
        }

        private void TgTabUp_Toggled(object sender, RoutedEventArgs e)
        {
            FireBrowserInterop.SettingsHelper.SetSetting("OpSw", TgTabUp.IsOn ? "True" : "0");
        }
    }
}
