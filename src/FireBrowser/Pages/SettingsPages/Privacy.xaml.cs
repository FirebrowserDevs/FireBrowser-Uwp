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

        string javasc = !string.IsNullOrEmpty(FireBrowserInterop.SettingsHelper.GetSetting("DisableJavaScript")) ? FireBrowserInterop.SettingsHelper.GetSetting("DisableJavaScript") : "true";
        string webmes = !string.IsNullOrEmpty(FireBrowserInterop.SettingsHelper.GetSetting("DisableWebMess")) ? FireBrowserInterop.SettingsHelper.GetSetting("DisableWebMess") : "true";
        string autogen = !string.IsNullOrEmpty(FireBrowserInterop.SettingsHelper.GetSetting("DisableGenAutoFill")) ? FireBrowserInterop.SettingsHelper.GetSetting("DisableGenAutoFill") : "true";
        string pass = !string.IsNullOrEmpty(FireBrowserInterop.SettingsHelper.GetSetting("DisablePassSave")) ? FireBrowserInterop.SettingsHelper.GetSetting("DisablePassSave") : "true";

        public void Stack()
        {
            DisableJavaScriptToggle.IsOn = javasc == "true";
            DisablWebMessFillToggle.IsOn = webmes == "true";
            DisableGenaralAutoFillToggle.IsOn = autogen == "true";
            PasswordWebMessFillToggle.IsOn = pass == "true";
        }

        int trueCount = 0;
        public void UpdateText()
        {
            TextLevel.Text = trueCount switch
            {
                0 => "Default",
                1 => "Low",
                2 => "Medium",
                3 => "High",
                4 => "Extreme",
                _ => throw new System.NotImplementedException()
            };
        }

        private void DisableJavaScriptToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (DisableJavaScriptToggle.IsOn)
            {
                FireBrowserInterop.SettingsHelper.SetSetting("DisableJavaScript", "true");
                trueCount++;
            }
            else
            {
                trueCount--;
                FireBrowserInterop.SettingsHelper.SetSetting("DisableJavaScript", "false");
            }
            UpdateText();
        }

        private void DisableGenaralAutoFillToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (DisableGenaralAutoFillToggle.IsOn)
            {
                FireBrowserInterop.SettingsHelper.SetSetting("DisableGenAutoFill", "true");
                trueCount++;
            }
            else
            {
                FireBrowserInterop.SettingsHelper.SetSetting("DisableGenAutoFill", "false");
                trueCount--;
            }
            UpdateText();
        }


        private void DisablWebMessFillToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (DisablWebMessFillToggle.IsOn)
            {
                FireBrowserInterop.SettingsHelper.SetSetting("DisableWebMess", "true");
                trueCount++;
            }
            else
            {
                FireBrowserInterop.SettingsHelper.SetSetting("DisableWebMess", "false");
                trueCount--;
            }
            UpdateText();
        }

        private void PasswordWebMessFillToggle_Toggled(object sender, RoutedEventArgs e)
        {
            if (PasswordWebMessFillToggle.IsOn)
            {
                trueCount++;
                FireBrowserInterop.SettingsHelper.SetSetting("DisablePassSave", "true");
            }
            else
            {
                trueCount--;
                FireBrowserInterop.SettingsHelper.SetSetting("DisablePassSave", "false");
            }
            UpdateText();
        }

        private async void Confirm_Click(object sender, RoutedEventArgs e)
        {
            await FireBrowserInterop.SystemHelper.RestartApp();
        }
    }
}
