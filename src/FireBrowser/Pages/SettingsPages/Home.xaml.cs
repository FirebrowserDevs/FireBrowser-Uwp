using Newtonsoft.Json;
using System;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FireBrowser.Pages.SettingsPages
{
    public sealed partial class Home : Page
    {
        public Home()
        {
            this.InitializeComponent();
            SysInfoBox.Text = "SysInfo: " + FireBrowserInterop.SystemHelper.GetSystemArchitecture();
            check();
        }

        public async void check()
        {
            // Get the local folder object
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;

            // Get the file object
            StorageFile file = await localFolder.GetFileAsync("Params.json");

            // Read the contents of the file
            string fileContent = await FileIO.ReadTextAsync(file);

            // Parse the JSON string to a dynamic object
            dynamic jsonObject = JsonConvert.DeserializeObject(fileContent);

            // Access the "IsConnected" parameter and check if it's true or false
            bool isConnected = jsonObject.IsConnected;

            if (isConnected == true)
            {
                TextStat.Text = "Connected";
            }
            else
            {
                TextStat.Text = "Connect";
            }
        }
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            StorageFile fileToDelete = await ApplicationData.Current.LocalFolder.GetFileAsync("Params.json");
            await fileToDelete.DeleteAsync();
            await FireBrowserInterop.SystemHelper.RestartApp();
        }

        private async void MsAccount_Click(object sender, RoutedEventArgs e)
        {
            MsLogin login = new MsLogin();
            await login.ShowAsync();
        }
    }
}
