using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using FireBrowser.Core;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FireBrowser.Pages.SettingsPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Home : Page
    {
        public Home()
        {
            this.InitializeComponent();
        }

        private void DropDownButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FireBrowserInterop.SettingsHelper.SetSetting("LaunchFirst", "1");
            FireBrowserInterop.SystemHelper.RestartApp();
        }
    }
}
