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
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

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
            this.Result = ProtectResult.Nothing;
        }

        public enum ProtectResult
        {
            Allow,
            Deny,
            Cancel,
            Nothing
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
