using FireBrowser.Core;
using QRCoder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static FireBrowser.Core.UserData;
using FireBrowser;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FireBrowser.Pages.SettingsPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NewTab : Page
    {
        public NewTab()
        {
            this.InitializeComponent();
       
            ButtonVisible();
        }

        private void SearchengineSelection_Loaded(object sender, RoutedEventArgs e)
        {
            string selection = SettingsHelper.GetSetting("EngineFriendlyName");
            if (selection != null)
            {
                SearchengineSelection.PlaceholderText = selection;
            }
            else
            {
                SearchengineSelection.PlaceholderText = "Google";
            }
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
        }

        private void SetEngine(string EngineFriendlyName, string SearchUrl)
        {
            SettingsHelper.SetSetting("EngineFriendlyName", EngineFriendlyName);
            SettingsHelper.SetSetting("SearchUrl", SearchUrl);
        }
   

        public string ReadButton1 = SettingsHelper.GetSetting("Readbutton");
        public string AdblockBtn1 = SettingsHelper.GetSetting("AdBtn");
        public string Downloads1 = SettingsHelper.GetSetting("DwBtn");
        public string Translate1 = SettingsHelper.GetSetting("TransBtn");
        public string Favorites1 = SettingsHelper.GetSetting("FavBtn");
        public string Historybtn1 = SettingsHelper.GetSetting("HisBtn");
        public string QrCode1 = SettingsHelper.GetSetting("QrBtn");
        
        public void ButtonVisible()
        {
            if (ReadButton1 == "True")
            {
                Easy.IsChecked = true;
            }
            else if(ReadButton1 == "0")
            {
                Easy.IsChecked = false;
            }
            if (AdblockBtn1 == "True")
            {
                Adbl.IsChecked = true;
            }
            else if (AdblockBtn1 == "0")
            {
               Adbl.IsChecked= false;
            }
            if (Downloads1 == "True")
            {
               Dwbl.IsChecked = true;
            }
            else if (Downloads1 == "0")
            {
               Dwbl.IsChecked = false;
            }
            if (Translate1 == "True")
            {
              Trbl.IsChecked= true;
            }
            else if (Translate1 == "0")
            {
                Trbl.IsChecked= false;
            }
            if (Favorites1 == "True")
            {
              Frbl.IsChecked = true;
            }
            else if (Favorites1 == "0")
            {
                Frbl.IsChecked= false;
            }
            if (Historybtn1 == "True")
            {
               Hsbl.IsChecked = true;
            }
            else if (Historybtn1 == "0")
            {
                Hsbl.IsChecked = false;
            }
            if (QrCode1 == "True")
            {
                Qrbl.IsChecked = true;
            }
            else if (QrCode1 == "0")
            {
                Qrbl.IsChecked = false;
            }
        }
        private void Page_Loading(FrameworkElement sender, object args)
        {
           
        }

        private void Qrbl_Checked(object sender, RoutedEventArgs e)
        {
            if (Qrbl.IsChecked == true)
            {
                SettingsHelper.SetSetting("QrBtn", "True");
            }
            else
            {
                SettingsHelper.SetSetting("QrBtn", "0");
            }
        }

        private void Hsbl_Checked(object sender, RoutedEventArgs e)
        {
            if (Hsbl.IsChecked == true)
            {
                SettingsHelper.SetSetting("HisBtn", "True");
            }
            else
            {
             
                SettingsHelper.SetSetting("HisBtn", "0");
            }
        }

        private void Frbl_Checked(object sender, RoutedEventArgs e)
        {
            if (Frbl.IsChecked == true)
            {
                SettingsHelper.SetSetting("FavBtn", "True");
            }
            else
            {
                SettingsHelper.SetSetting("FavBtn", "0");
            }
        }

        private void Trbl_Checked(object sender, RoutedEventArgs e)
        {
            if (Trbl.IsChecked == true)
            {
                SettingsHelper.SetSetting("TransBtn", "True");
            }
            else
            {
                SettingsHelper.SetSetting("TransBtn", "0");
            }
        }

        private void Dwbl_Checked(object sender, RoutedEventArgs e)
        {
            if (Dwbl.IsChecked == true)
            {
                SettingsHelper.SetSetting("DwBtn", "True");
            }
            else
            {
                SettingsHelper.SetSetting("DwBtn", "0");
            }
        }

        private void Adbl_Checked(object sender, RoutedEventArgs e)
        {
            if (Adbl.IsChecked == true)
            {
                SettingsHelper.SetSetting("AdBtn", "True");
            }
            else
            {
                SettingsHelper.SetSetting("AdBtn", "0");
            }
        }

        private void Easy_Checked(object sender, RoutedEventArgs e)
        {
            if(Easy.IsChecked == true)
            {
                SettingsHelper.SetSetting("Readbutton", "True");
            }
            else
            {
                SettingsHelper.SetSetting("Readbutton", "0");
            }
        }
    }
}
