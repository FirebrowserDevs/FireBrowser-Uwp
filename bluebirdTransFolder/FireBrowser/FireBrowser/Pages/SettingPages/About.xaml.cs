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

    private void TextBlock_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
    {
        string url = (sender as TextBlock).Tag.ToString();
        //Globals.MainPageContent.WebViewControl.CoreWebView2.Navigate(url);
    }
}
