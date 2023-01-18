using FireBrowser.Core;
using FireBrowser.Shared;
using Windows.UI.Xaml.Controls;

namespace FireBrowser.Pages;

public sealed partial class AboutPage : Page
{
    public AboutPage()
    {
        this.InitializeComponent();
        WindowManager.SetWindowTitle("About");
        FireBrowserVersion.Text = "FireBrowser " + AppVersion.GetAppVersion() + " (" + SystemHelper.GetSystemArchitecture() + ")";
        WebView2Version.Text = "WebView2 " + UA.wv2version;
    }

    private void TextBlock_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
    {
        string url = (sender as TextBlock).Tag.ToString();
        //Globals.MainPageContent.WebViewControl.CoreWebView2.Navigate(url);
    }
}
