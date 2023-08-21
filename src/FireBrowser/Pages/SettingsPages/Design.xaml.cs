using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FireBrowser.Pages.SettingsPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Design : Page
    {
        public Design()
        {
            this.InitializeComponent();
            loadValues();
        }

        public void loadValues()
        {
            var value = !string.IsNullOrEmpty(FireBrowserInterop.SettingsHelper.GetSetting("ColorTool")) ? FireBrowserInterop.SettingsHelper.GetSetting("ColorTool") : "#000000";
            var value2 = !string.IsNullOrEmpty(FireBrowserInterop.SettingsHelper.GetSetting("ColorTV")) ? FireBrowserInterop.SettingsHelper.GetSetting("ColorTV") : "#000000";

            var color = !string.IsNullOrEmpty(FireBrowserInterop.SettingsHelper.GetSetting("ColorBackground")) ? FireBrowserInterop.SettingsHelper.GetSetting("ColorBackground") : "#000000";
            var layout = !string.IsNullOrEmpty(FireBrowserInterop.SettingsHelper.GetSetting("Background")) ? FireBrowserInterop.SettingsHelper.GetSetting("Background") : "0";
            var auto = !string.IsNullOrEmpty(FireBrowserInterop.SettingsHelper.GetSetting("Auto")) ? FireBrowserInterop.SettingsHelper.GetSetting("Auto") : "0";


            AutoTog.IsOn = auto switch
            {
                "0" => false,
                "1" => true,
                _ => throw new System.NotImplementedException()
            };

            Type.SelectedItem = layout switch
            {
                "0" => "Default",
                "1" => "Featured",
                "2" => "Custom",
                _ => throw new System.NotImplementedException()
            };
            ColorTB.Text = value;
            ColorTV.Text = value2;
            Color.Text = color;
        }

        private void Type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selection = e.AddedItems[0].ToString();
            if (selection == "Default")
            {
                Color.IsEnabled = false;
                FireBrowserInterop.SettingsHelper.SetSetting("Background", "0");
            }
            if (selection == "Featured")
            {
                Color.IsEnabled = false;
                FireBrowserInterop.SettingsHelper.SetSetting("Background", "1");
            }
            if (selection == "Custom")
            {
                Color.IsEnabled = true;
                FireBrowserInterop.SettingsHelper.SetSetting("Background", "2");
            }
        }

        private void AutoTog_Toggled(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                FireBrowserInterop.SettingsHelper.SetSetting("Auto", toggleSwitch.IsOn ? "1" : "0");
            }
        }

        private void ColorTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            string value = ColorTB.Text.ToString();
            FireBrowserInterop.SettingsHelper.SetSetting("ColorTool", value);
        }

        private void ColorTV_TextChanged(object sender, TextChangedEventArgs e)
        {
            string value = ColorTV.Text.ToString();
            FireBrowserInterop.SettingsHelper.SetSetting("ColorTV", value);
        }

        private void Color_TextChanged(object sender, TextChangedEventArgs e)
        {
            FireBrowserInterop.SettingsHelper.SetSetting("ColorBackground", $"{Color.Text.ToString()}");
        }
    }
}
