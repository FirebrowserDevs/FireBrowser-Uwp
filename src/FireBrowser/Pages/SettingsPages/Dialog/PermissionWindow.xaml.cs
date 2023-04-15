using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FireBrowser.Pages.SettingsPages.Dialog
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PermissionWindow : ContentDialog
    {
        public ProtectResult Result { get; set; }
        public PermissionWindow()
        {
            this.InitializeComponent();
            this.Result = ProtectResult.Random;
            Closed += PermissionWindow_Closed;
        }

        private void PermissionWindow_Closed(ContentDialog sender, ContentDialogClosedEventArgs args)
        {

        }

        public enum ProtectResult
        {
            Allow,
            Deny,
            Cancel,
            Nothing,
            Random
        }
        private void Allow_Click(object sender, RoutedEventArgs e)
        {
            Result = ProtectResult.Allow;
            Hide();
        }

        private void Deny_Click(object sender, RoutedEventArgs e)
        {
            Result = ProtectResult.Deny;
            Hide();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Result = ProtectResult.Cancel;
            Hide();
        }
    }
}
