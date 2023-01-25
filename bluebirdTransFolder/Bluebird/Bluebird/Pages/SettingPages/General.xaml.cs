using Bluebird.Core;
using System.Threading.Tasks;
using System;
using Windows.ApplicationModel;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace Bluebird.Pages.SettingPages;

public sealed partial class General : Page
{
    public General()
    {
        this.InitializeComponent();
        id();
    }

    private async void id()
    {
        var startup = await StartupTask.GetAsync("BlueBirdStartUp");
        UpdateToggleState(startup.State);
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
        if (selection == "Ask") SetEngine("Ask", "https://www.ask.com/web?q=");
        if (selection == "Baidu") SetEngine("Baidu", "https://www.baidu.com/s?ie=utf-8&f=8&rsv_bp=1&rsv_idx=1&tn=baidu&wd=");
        if (selection == "Bing") SetEngine("Bing", "https://www.bing.com?q=");
        if (selection == "DuckDuckGo") SetEngine("DuckDuckGo","https://www.duckduckgo.com?q=");
        if (selection == "Ecosia") SetEngine("Ecosia", "https://www.ecosia.org/search?q=");
        if (selection == "Google") SetEngine("Google", "https://www.google.com/search?q=");
        if (selection == "Startpage") SetEngine("Startpage", "https://www.startpage.com/search?q=");
        if (selection == "Qwant") SetEngine("Qwant", "https://www.qwant.com/?q=");
        if (selection == "Qwant Lite") SetEngine("Qwant Lite", "https://lite.qwant.com/?q=");
        if (selection == "Yahoo!") SetEngine("Yahoo!", "https://search.yahoo.com/search?p=");
    }

    private void SetEngine(string EngineFriendlyName, string SearchUrl)
    {
        SettingsHelper.SetSetting("EngineFriendlyName", EngineFriendlyName);
        SettingsHelper.SetSetting("SearchUrl", SearchUrl);
    }

    private void UpdateToggleState(StartupTaskState state)
    {
        AutoStart.IsEnabled = true;
        switch (state)
        {
            case StartupTaskState.Enabled:
                AutoStart.IsChecked = true;
                break;
            case StartupTaskState.Disabled:
            case StartupTaskState.DisabledByUser:
                AutoStart.IsChecked = false;
                break;
            default:
                AutoStart.IsEnabled = false;
                break;
        }

    }
    private async Task ToggleLaunchOnStartup(bool enable)
    {
        var startup = await StartupTask.GetAsync("BlueBirdStartUp");
        switch (startup.State)
        {
            case StartupTaskState.Enabled when !enable:
                startup.Disable();
                break;
            case StartupTaskState.Disabled when enable:
                var updatedState = await startup.RequestEnableAsync();
                UpdateToggleState(updatedState);
                break;
            case StartupTaskState.DisabledByUser when enable:
                await new MessageDialog("Unable to change state of startup task via the application - enable via Startup tab on Task Manager (Ctrl+Shift+Esc)").ShowAsync();
                break;
            default:
                await new MessageDialog("Unable to change state of startup task").ShowAsync();
                break;
        }
    }

    private async void AutoStart_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
    {
        await ToggleLaunchOnStartup(AutoStart.IsChecked ?? false);
    }
}
