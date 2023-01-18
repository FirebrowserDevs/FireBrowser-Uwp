using FireBrowser.Core;
using FireBrowser.Shared;
using System;
using Windows.UI.Xaml.Controls;

namespace FireBrowser.Pages;

/// <summary>
/// A page which allows you to change various settings of this app
/// </summary>
public sealed partial class SettingsPage : Page
{
    public SettingsPage()
    {
        this.InitializeComponent();
        WindowManager.SetWindowTitle("Settings");
    }
    private void SearchengineSelection_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    {
        string selection = SettingsHelper.GetSetting("EngineFriendlyName");
        if (selection != null)
        {
            SearchengineSelection.PlaceholderText = selection;
        }
        else
        {
            SearchengineSelection.PlaceholderText = "Qwant Lite";
        }
    }

    private void SearchengineSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        string selection = e.AddedItems[0].ToString();
        if (selection == "Ask") SetEngine("Ask", "https://www.ask.com/", "https://www.ask.com/web?q=");
        if (selection == "Baidu") SetEngine("Baidu", "https://www.baidu.com/", "https://www.baidu.com/s?ie=utf-8&f=8&rsv_bp=1&rsv_idx=1&tn=baidu&wd=");
        if (selection == "Bing") SetEngine("Bing", "https://www.bing.com/", "https://www.bing.com?q=");
        if (selection == "DuckDuckGo")  SetEngine("DuckDuckGo", "https://start.duckduckgo.com/", "https://www.duckduckgo.com?q=");
        if (selection == "Ecosia") SetEngine("Ecosia", "https://www.ecosia.org/", "https://www.ecosia.org/search?q=");
        if (selection == "Google") SetEngine("Google", "https://www.google.com/", "https://www.google.com/?q=");
        if (selection == "Startpage") SetEngine("Startpage", "https://www.startpage.com/", "https://www.startpage.com/search?q=");
        if (selection == "Qwant") SetEngine("Qwant", "https://www.qwant.com/", "https://www.qwant.com/?q=");
        if (selection == "Qwant Lite") SetEngine("Qwant Lite", "https://lite.qwant.com/", "https://lite.qwant.com/?q=");
        if (selection == "Yahoo!") SetEngine("Yahoo!", "https://yahoo.com/", "https://search.yahoo.com/search?p=");
    }

    private void SetEngine(string EngineFriendlyName, string HomepageUrl, string SearchUrl)
    {
        SettingsHelper.SetSetting("EngineFriendlyName", EngineFriendlyName);
        SettingsHelper.SetSetting("HomepageUrl", HomepageUrl);
        SettingsHelper.SetSetting("SearchUrl", SearchUrl);
    }

    #region Data
    private async void ClearBrowserData_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    {
        ContentDialog contentDialog = new()
        {
            Title = "Clear data",
            Content = "Do you want to clear all your browsing data including: Favorites and History",
            PrimaryButtonText = "Clear",
            SecondaryButtonText = "Cancel"
        };

        var result = await contentDialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            // Clear favorties
            FileHelper.DeleteLocalFile("Favorites.json");
            // Clear history
            FileHelper.DeleteLocalFile("History.json");
        }
    }
    #endregion

    #region Privacy
    private void DisableJavaScriptToggle_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    {
        string selection = SettingsHelper.GetSetting("DisableJavaScript");
        if (selection == "true")
        {
            DisableJavaScriptToggle.IsOn = true;
        }
    }
    private void DisableJavaScriptToggle_Toggled(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    {
        if (DisableJavaScriptToggle.IsOn)
        {
            SettingsHelper.SetSetting("DisableJavaScript", "true");
        }
        else
        {
            SettingsHelper.SetSetting("DisableJavaScript", "false");
        }
    }
    #endregion
}
