using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FireBrowser.Core
{
    public sealed partial class FireAiSmart : ContentDialog
    {
        public FireAiSmart()
        {
            this.InitializeComponent();
            IsSecondaryButtonEnabled = false;
        }

        private void Proceed_Checked(object sender, RoutedEventArgs e)
        {
            IsSecondaryButtonEnabled = true;
        }

        private void Proceed_Unchecked(object sender, RoutedEventArgs e)
        {
            IsSecondaryButtonEnabled = false;
        }
    }
}
