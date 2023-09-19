using FireBrowserDataBase;
using System;
using Windows.Devices.Geolocation;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FireBrowser.Launch;

public sealed partial class SetupSettings : Page
{
    public SetupSettings() => InitializeComponent();

    public void tempkeys()
    {
        FireBrowserInterop.SettingsHelper.SetSetting("DisableJavaScript", "false");
        FireBrowserInterop.SettingsHelper.SetSetting("DisablePassSave", "false");
        FireBrowserInterop.SettingsHelper.SetSetting("DisableWebMess", "false");
        FireBrowserInterop.SettingsHelper.SetSetting("DisableGenAutoFill", "false");
        FireBrowserInterop.SettingsHelper.SetSetting("ColorBackground", "#000000");
        FireBrowserInterop.SettingsHelper.SetSetting("StatusBar", "1");
        FireBrowserInterop.SettingsHelper.SetSetting("BrowserKeys", "1");
        FireBrowserInterop.SettingsHelper.SetSetting("BrowserScripts", "1");
        FireBrowserInterop.SettingsHelper.SetSetting("Useragent", "FireBrowser Webview");
        FireBrowserInterop.SettingsHelper.SetSetting("LightMode", "0");
        FireBrowserInterop.SettingsHelper.SetSetting("OpSw", "True");
    }

    private void SearchengineSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        string selection = e.AddedItems[0].ToString();
        string url;

        switch (selection)
        {
            case "Ask":
                url = "https://www.ask.com/web?q=";
                break;
            case "Baidu":
                url = "https://www.baidu.com/s?ie=utf-8&f=8&rsv_bp=1&rsv_idx=1&tn=baidu&wd=";
                break;
            case "Bing":
                url = "https://www.bing.com?q=";
                break;
            case "DuckDuckGo":
                url = "https://www.duckduckgo.com?q=";
                break;
            case "Ecosia":
                url = "https://www.ecosia.org/search?q=";
                break;
            case "Google":
                url = "https://www.google.com/search?q=";
                break;
            case "Startpage":
                url = "https://www.startpage.com/search?q=";
                break;
            case "Qwant":
                url = "https://www.qwant.com/?q=";
                break;
            case "Qwant Lite":
                url = "https://lite.qwant.com/?q=";
                break;
            case "Yahoo!":
                url = "https://search.yahoo.com/search?p=";
                break;
            case "Presearch":
                url = "https://presearch.com/search?q=";
                break;
            default:
                // Handle the case when selection doesn't match any of the predefined options.
                url = "https://www.google.com/search?q=";
                break;
        }

        if (!string.IsNullOrEmpty(url))
        {
            SetEngine(selection, url);
        }
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

    private async void Install_Click(object sender, RoutedEventArgs e)
    {
        setdefault();
        await DbCreation.CreateDatabase();
        ContentFrame.Navigate(typeof(SetupStep2));
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

    private void FrL_Toggled(object sender, RoutedEventArgs e)
    {
        FireBrowserInterop.SettingsHelper.SetSetting("FlBtn", FrL.IsOn ? "True" : "0");
    }

    private void Darklg_Toggled(object sender, RoutedEventArgs e)
    {
        FireBrowserInterop.SettingsHelper.SetSetting("DarkBtn", Darklg.IsOn ? "True" : "0");
    }

    private void Tooltl_Toggled(object sender, RoutedEventArgs e)
    {
        FireBrowserInterop.SettingsHelper.SetSetting("ToolBtn", Tooltl.IsOn ? "True" : "0");
    }

    private void Color_TextChanged(object sender, TextChangedEventArgs e)
    {
        FireBrowserInterop.SettingsHelper.SetSetting("ColorBackground", $"{Color.Text.ToString()}");
    }

    private void BackgroundTabs_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        string selection = e.AddedItems[0].ToString();

        Color.IsEnabled = selection switch
        {
            "Default" => false,
            "Featured" => false,
            "Custom" => true,
            _ => Color.IsEnabled
        };

        FireBrowserInterop.SettingsHelper.SetSetting("Background", selection switch
        {
            "Default" => "0",
            "Featured" => "1",
            "Custom" => "2",
            _ => FireBrowserInterop.SettingsHelper.GetSetting("Background")
        });
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        tempkeys();
    }
}
