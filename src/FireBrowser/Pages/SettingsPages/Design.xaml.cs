using System;
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

            var layout = FireBrowserInterop.SettingsHelper.GetSetting("Background");
            var auto = FireBrowserInterop.SettingsHelper.GetSetting("Auto");

            AutoTog.IsOn = auto switch
            {
                "0" => false,
                "1" => true
            };

            Type.SelectedItem = layout switch
            {
                "0" => "Default",
                "1" => "Featured"
            };
        }

        private void Type_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = Type.SelectedItem as string;

            FireBrowserInterop.SettingsHelper.SetSetting("Background", selected switch
            {
                "Default" => "0",
                "Featured" => "1",
                _ => throw new ArgumentException($"Invalid selection: {selected}")
            });
        }

        private void AutoTog_Toggled(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                FireBrowserInterop.SettingsHelper.SetSetting("Auto", toggleSwitch.IsOn ? "1" : "0");
            }
        }
    }
}
