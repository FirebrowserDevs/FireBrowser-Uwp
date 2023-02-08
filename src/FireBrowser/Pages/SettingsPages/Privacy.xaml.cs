using FireBrowser.Core;
using FireBrowser.Pages.SettingsPages.Restart;
using Microsoft.Toolkit.Uwp.UI;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FireBrowser.Pages.SettingsPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Privacy : Page
    {
        public Privacy()
        {
            this.InitializeComponent();
            UpdateText();
            Stack();
        }

        string javasc = SettingsHelper.GetSetting("DisableJavaScript");
        string webmes = SettingsHelper.GetSetting("DisableWebMess");
        string autogen = SettingsHelper.GetSetting("DisableGenAutoFill");
        string pass = SettingsHelper.GetSetting("DisablePassSave");
        public async void Stack()
        {
            if (javasc == "true")
            {
                    DisableJavaScriptToggle.IsOn = true;
            }
            else
            {
                    DisableJavaScriptToggle.IsOn = false;
            }
            if (webmes == "true")
            {
                DisablWebMessFillToggle.IsOn = true;
            }
            else
            {
                DisablWebMessFillToggle.IsOn = false;
            }
            if (autogen == "true")
            {
                DisableGenaralAutoFillToggle.IsOn = true;
            }
            else
            {
                DisableGenaralAutoFillToggle.IsOn = false;
            }
            if(pass == "true")
            {
                PasswordWebMessFillToggle.IsOn = true;
            }
            else
            {
                PasswordWebMessFillToggle.IsOn = false;
            }
        }

        int trueCount = 0;
        public async void UpdateText()
        {
            TextLevel.Text = trueCount switch
            {
                0 => "Default",
                1 => "Low",
                2 => "Medium",
                3 => "High",
                4 => "Extreme"
            };
            
        }

        public async void IfChanged()
        {
         
            RestartFrame.Navigate(typeof(RestartDialog));
                RestartFrame.Visibility = Visibility.Visible;
        }

        private void DisableJavaScriptToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (DisableJavaScriptToggle.IsOn)
            {
                SettingsHelper.SetSetting("DisableJavaScript", "true");
                trueCount++;
         
            }
            else
            {
                trueCount--;
                SettingsHelper.SetSetting("DisableJavaScript", "false");
              
            }
            IfChanged();
            UpdateText();
        }

        private void DisableGenaralAutoFillToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (DisableGenaralAutoFillToggle.IsOn)
            {
                SettingsHelper.SetSetting("DisableGenAutoFill", "true");
                trueCount++;
              
            }
            else
            {
                SettingsHelper.SetSetting("DisableGenAutoFill", "false");
                trueCount--;
              
            }
            IfChanged();
            UpdateText();
        }

      
        private void DisablWebMessFillToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (DisablWebMessFillToggle.IsOn)
            {
                SettingsHelper.SetSetting("DisableWebMess", "true");
                trueCount++;
            }
            else
            {
                SettingsHelper.SetSetting("DisableWebMess", "false");
                trueCount--;
            }
            IfChanged();
            UpdateText();
        }

        private void PasswordWebMessFillToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (PasswordWebMessFillToggle.IsOn)
            {
                trueCount++;
                SettingsHelper.SetSetting("DisablePassSave", "true");
            }
            else
            {
                trueCount--;
                SettingsHelper.SetSetting("DisablePassSave", "false");
            }
            IfChanged();
            UpdateText();
        }
    }
}
