using FireBrowser.Core;
using FireBrowser.Shared;
using Windows.UI.Xaml.Controls;

namespace FireBrowser.Pages.SettingPages;

public sealed partial class About : Page
{
    public About()
    {
        this.InitializeComponent();
        FireBrowserVersion.Text = "FireBrowser " + AppVersion.GetAppVersion() + " (" + SystemHelper.GetSystemArchitecture() + ")";
        WebView2Version.Text = "WebView2 " + UA.wv2version;
    }
}
