using FireBrowser.Pages;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FireBrowser.Core;
public class UseContent
{
    public static WebContent WebContent => (Window.Current.Content as Frame)?.Content as WebContent;
    public static MainPage MainPageContent => (Window.Current.Content as Frame)?.Content as MainPage;
    public static SettingsPage SettingsContent => (Window.Current.Content as Frame)?.Content as SettingsPage;
    public static NewTab NewTabPage => (Window.Current.Content as Frame)?.Content as NewTab;
}
