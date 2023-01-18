using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.UI.Popups;

//Szablon elementu Pusta strona jest udokumentowany na stronie https://go.microsoft.com/fwlink/?LinkId=234238

namespace FireBrowser.Pages.Settings
{
    /// <summary>
    /// Pusta strona, która może być używana samodzielnie lub do której można nawigować wewnątrz ramki.
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
            var startup = await StartupTask.GetAsync("AirplaneStartUp");
            UpdateToggleState(startup.State);
        }
        private async void LaunchOnStartupToggle_Click(object sender, RoutedEventArgs e)
        {
            await ToggleLaunchOnStartup(LaunchOnStartupToggle.IsChecked ?? false);
        }

        private void UpdateToggleState(StartupTaskState state)
        {
            LaunchOnStartupToggle.IsEnabled = true;
            switch (state)
            {
                case StartupTaskState.Enabled:
                    LaunchOnStartupToggle.IsChecked = true;
                    break;
                case StartupTaskState.Disabled:
                case StartupTaskState.DisabledByUser:
                    LaunchOnStartupToggle.IsChecked = false;
                    break;
                default:
                    LaunchOnStartupToggle.IsEnabled = false;
                    break;
            }
          
        }
        private async Task ToggleLaunchOnStartup(bool enable)
        {
            var startup = await StartupTask.GetAsync("AirplaneStartUp");
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
                    await new MessageDialog("Unable to change state of startup task via the application - enable via Startup tab on Task Manager (Ctrl+Shift+Esc)").ShowAsync();
                    break;
                default:
                    await new MessageDialog("Unable to change state of startup task").ShowAsync();
                    break;
            }
        }
    }
}
