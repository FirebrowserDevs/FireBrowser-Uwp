using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using FireBrowser.Core;
using Windows.Storage;
using System.IO;
using System;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Bluetooth;
using Windows.UI.Popups;

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
            SysInfoBox.Text = "SysInfo: " + FireBrowserInterop.SystemHelper.GetSystemArchitecture();
        }


        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            StorageFile fileToDelete = await ApplicationData.Current.LocalFolder.GetFileAsync("Isettings.json");
            await fileToDelete.DeleteAsync();
            FireBrowserInterop.SystemHelper.RestartApp();
        }
    }
}
