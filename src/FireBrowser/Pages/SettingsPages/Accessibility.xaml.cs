using FireBrowser.Core;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FireBrowser.Pages.SettingsPages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Accessibility : Page
    {
        public Accessibility()
        {
            this.InitializeComponent();
            id();
        }

        private async void id()
        {
            var startup = await StartupTask.GetAsync("FireBrowserStartUp");
            UpdateToggleState(startup.State);
        }

        private void UpdateToggleState(StartupTaskState state)
        {
            LaunchOnStartupToggle.IsEnabled = true;
            LaunchOnStartupToggle.IsChecked = state switch
            {
                StartupTaskState.Enabled => true,
                StartupTaskState.Disabled => false,
                StartupTaskState.DisabledByUser => false,
                _ => LaunchOnStartupToggle.IsEnabled = false
            };
        }
        private async Task ToggleLaunchOnStartup(bool enable)
        {
            var startup = await StartupTask.GetAsync("FireBrowserStartUp");

            switch (startup.State)
            {
                case StartupTaskState.Enabled when !enable:
                    startup.Disable();
                    break;
                case StartupTaskState.Disabled when enable:
                    var updatedState = await startup.RequestEnableAsync();
                    UpdateToggleState(updatedState);
                    break;
                case StartupTaskState.DisabledByUser when enable:
                    ContentDialog cs = new ContentDialog();
                    cs.Title = "Unable to change state of startup task via the application";
                    cs.Content = "Enable via Startup tab on Task Manager (Ctrl+Shift+Esc)";
                    cs.PrimaryButtonText = "OK";
                    cs.ShowAsync();
                    break;
                default:
                    ContentDialog cs2 = new ContentDialog();
                    cs2.Title = "Unable to change state of startup task";
                    cs2.PrimaryButtonText = "OK";
                    cs2.ShowAsync();
                    break;
            }
        }
        private async void LaunchOnStartupToggle_Click(object sender, RoutedEventArgs e)
        {
            await ToggleLaunchOnStartup(LaunchOnStartupToggle.IsChecked ?? false);
        }
    }
}
