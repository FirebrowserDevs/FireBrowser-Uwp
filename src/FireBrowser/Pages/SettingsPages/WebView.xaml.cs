using ColorCode.Compilation.Languages;
using FireBrowser.Core;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static System.Net.Mime.MediaTypeNames;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FireBrowser.Pages.SettingsPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class WebView : Page
    {
        public WebView()
        {
            this.InitializeComponent();
            Stack();
        }

        string stat = FireBrowserInterop.SettingsHelper.GetSetting("StatusBar");
        string keys = FireBrowserInterop.SettingsHelper.GetSetting("BrowserKeys");
        string script = FireBrowserInterop.SettingsHelper.GetSetting("BrowserScripts");

        public void Stack()
        {
            StatusTog.IsOn = stat == "1";
            BrowserKeys.IsOn = keys == "1";
            BrowserScripts.IsOn = script == "1";
        }

        private void StatusTog_Toggled(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                FireBrowserInterop.SettingsHelper.SetSetting("StatusBar", toggleSwitch.IsOn ? "1" : "0");
            }
        }

        private void BrowserKeys_Toggled(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                FireBrowserInterop.SettingsHelper.SetSetting("BrowserKeys", toggleSwitch.IsOn ? "1" : "0");
            }
        }

        private void Agent_TextChanged(object sender, TextChangedEventArgs e)
        {
            FireBrowserInterop.SettingsHelper.SetSetting("Useragent", Agent.Text.ToString());
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
           string us = FireBrowserInterop.SettingsHelper.GetSetting("Useragent");
           Agent.Text = us;
        }

        private void BrowserScripts_Toggled(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                FireBrowserInterop.SettingsHelper.SetSetting("BrowserScripts", toggleSwitch.IsOn ? "1" : "0");
            }
        }

        WebView2 Web = new WebView2();
        private async void ClearCookies_Click(object sender, RoutedEventArgs e)
        {
            await Web.EnsureCoreWebView2Async();
            Web.CoreWebView2.CookieManager.DeleteAllCookies();
        }     
    }
}
