using FireBrowser.Core;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FireBrowser.Launch
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SetupSettings : Page
    {
        public SetupSettings()
        {
            this.InitializeComponent();
            checkcombobox();
        }


        private void SearchengineSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selection = e.AddedItems[0].ToString();
            if (selection == "Ask") SetEngine("Ask", "https://www.ask.com/web?q=");
            if (selection == "Baidu") SetEngine("Baidu", "https://www.baidu.com/s?ie=utf-8&f=8&rsv_bp=1&rsv_idx=1&tn=baidu&wd=");
            if (selection == "Bing") SetEngine("Bing", "https://www.bing.com?q=");
            if (selection == "DuckDuckGo") SetEngine("DuckDuckGo", "https://www.duckduckgo.com?q=");
            if (selection == "Ecosia") SetEngine("Ecosia", "https://www.ecosia.org/search?q=");
            if (selection == "Google") SetEngine("Google", "https://www.google.com/search?q=");
            if (selection == "Startpage") SetEngine("Startpage", "https://www.startpage.com/search?q=");
            if (selection == "Qwant") SetEngine("Qwant", "https://www.qwant.com/?q=");
            if (selection == "Qwant Lite") SetEngine("Qwant Lite", "https://lite.qwant.com/?q=");
            if (selection == "Yahoo!") SetEngine("Yahoo!", "https://search.yahoo.com/search?p=");
            checkcombobox();
        }

        private void SetEngine(string EngineFriendlyName, string SearchUrl)
        {
            FireBrowserInterop.SettingsHelper.SetSetting("EngineFriendlyName", EngineFriendlyName);
            FireBrowserInterop.SettingsHelper.SetSetting("SearchUrl", SearchUrl);
        }

        private void checkcombobox()
        {
            if (!string.IsNullOrEmpty(SearchengineSelection.SelectedItem?.ToString()) && !string.IsNullOrEmpty(Background.SelectedItem?.ToString()))
            {
                Install.IsEnabled = true;
            }
            else
            {
                Install.IsEnabled = false;
            }
        }
        private void Install_Click(object sender, RoutedEventArgs e)
        {
            
                FireBrowserInterop.SettingsHelper.SetSetting("LaunchFirst", "0");
                FireBrowserInterop.SystemHelper.RestartApp();
            
        }

        private void Read_Toggled(object sender, RoutedEventArgs e)
        {
            if (Read.IsOn == true)
            {
                FireBrowserInterop.SettingsHelper.SetSetting("Readbutton", "True");
            }
            else if (Read.IsOn == false)
            {

                FireBrowserInterop.SettingsHelper.SetSetting("Readbutton", "0");
            }
        }

        private void Adbl_Toggled(object sender, RoutedEventArgs e)
        {
            if (Adbl.IsOn == true)
            {
                FireBrowserInterop.SettingsHelper.SetSetting("AdBtn", "True");
            }
            else if (Adbl.IsOn == false)
            {
                FireBrowserInterop.SettingsHelper.SetSetting("AdBtn", "0");
            }
        }

        private void Dwbl_Toggled(object sender, RoutedEventArgs e)
        {
            if (Dwbl.IsOn == true)
            {
                FireBrowserInterop.SettingsHelper.SetSetting("DwBtn", "True");
            }
            else if (Dwbl.IsOn == false)
            {
                FireBrowserInterop.SettingsHelper.SetSetting("DwBtn", "0");
            }
        }

        private void Trbl_Toggled(object sender, RoutedEventArgs e)
        {
            if (Trbl.IsOn == true)
            {
                FireBrowserInterop.SettingsHelper.SetSetting("TransBtn", "True");
            }
            else if (Trbl.IsOn == false)
            {
                FireBrowserInterop.SettingsHelper.SetSetting("TransBtn", "0");
            }
        }

        private void Frbl_Toggled(object sender, RoutedEventArgs e)
        {
            if (Frbl.IsOn == true)
            {
                FireBrowserInterop.SettingsHelper.SetSetting("FavBtn", "True");
            }
            else if (Frbl.IsOn == false)
            {
                FireBrowserInterop.SettingsHelper.SetSetting("FavBtn", "0");
            }
        }

        private void Hsbl_Toggled(object sender, RoutedEventArgs e)
        {
            if (Hsbl.IsOn == true)
            {
                FireBrowserInterop.SettingsHelper.SetSetting("HisBtn", "True");
            }
            else if (Hsbl.IsOn == false)
            {

                FireBrowserInterop.SettingsHelper.SetSetting("HisBtn", "0");
            }
        }

        private void Qrbl_Toggled(object sender, RoutedEventArgs e)
        {
            if (Qrbl.IsOn == true)
            {
                FireBrowserInterop.SettingsHelper.SetSetting("QrBtn", "True");
            }
            else if (Qrbl.IsOn == false)
            {
                FireBrowserInterop.SettingsHelper.SetSetting("QrBtn", "0");
            }
        }

        private void Background_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selection = e.AddedItems[0].ToString();
            if (selection == "Default") FireBrowserInterop.SettingsHelper.SetSetting("Background", "0");
            if (selection == "Featured") FireBrowserInterop.SettingsHelper.SetSetting("Background", "1");
            checkcombobox();
        }
    }
}
