using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FireBrowser.Launch;

public sealed partial class SetupFinal : Page
{
    public SetupFinal() => InitializeComponent();

    private void StatusTog_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            FireBrowserInterop.SettingsHelper.SetSetting("StatusBar", toggleSwitch.IsOn ? "1" : "0");
        }
    }

    private void BrowserKeys_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            FireBrowserInterop.SettingsHelper.SetSetting("BrowserKeys", toggleSwitch.IsOn ? "1" : "0");
        }
    }

    private void BrowserScripts_Toggled(object sender, RoutedEventArgs e)
    {
        if (sender is ToggleSwitch toggleSwitch)
        {
            FireBrowserInterop.SettingsHelper.SetSetting("BrowserScripts", toggleSwitch.IsOn ? "1" : "0");
        }
    }

    private void Agent_TextChanged(object sender, TextChangedEventArgs e)
    {
        FireBrowserInterop.SettingsHelper.SetSetting("Useragent", Agent.Text.ToString());
    }

    private async void Finish_Click(object sender, RoutedEventArgs e)
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

        await FireBrowserInterop.SystemHelper.RestartApp();
    }
}
