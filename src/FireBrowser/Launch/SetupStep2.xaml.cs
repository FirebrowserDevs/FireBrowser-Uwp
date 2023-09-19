using FireBrowser.Pages.SettingsPages;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FireBrowser.Launch;

public sealed partial class SetupStep2 : Page
{
    public SetupStep2() => InitializeComponent();

    private void LgMode_Toggled(object sender, RoutedEventArgs e)
    {
        FireBrowserInterop.SettingsHelper.SetSetting("LightMode", LgMode.IsOn ? "1" : "0");
    }

    private async void ConnectBtn_Click(object sender, RoutedEventArgs e)
    {
        MsLogin ms = new MsLogin();
        await ms.ShowAsync();
    }

    private void Install_Click(object sender, RoutedEventArgs e)
    {
        if (tbv.Text.Equals("#000000"))
        {
            FireBrowserInterop.SettingsHelper.SetSetting("ColorTool", "#000000");
        }
        if (tbc.Text.Equals("#000000"))
        {
            FireBrowserInterop.SettingsHelper.SetSetting("ColorTV", "#000000");
        }
        ContentFrame.Navigate(typeof(SetupFinal));
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

    private void Langue_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        string selection = e.AddedItems[0].ToString();
        if (selection == "nl-NL") FireBrowserInterop.SettingsHelper.SetSetting("Lang", "nl-NL");
        if (selection == "en-US") FireBrowserInterop.SettingsHelper.SetSetting("Lang", "en-US");
    }

    private void Langue_Loaded(object sender, RoutedEventArgs e)
    {
        string selection = FireBrowserInterop.SettingsHelper.GetSetting("Lang");
        Langue.PlaceholderText = selection ?? "nl-NL";
        FireBrowserInterop.SettingsHelper.SetSetting("Lang", selection);
    }
}
