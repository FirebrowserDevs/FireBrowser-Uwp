using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.IO;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FireBrowser.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CompactOverlay : Page
    {
        public CompactOverlay()
        {
            this.InitializeComponent();
            ViewModel = new CompactOverlayViewModel();
        }
        public CompactOverlayViewModel ViewModel { get; set; }

        public partial class CompactOverlayViewModel : ObservableObject
        {
            [ObservableProperty]
            private Uri address;
            [ObservableProperty]
            private string text;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            testWv.CoreWebView2.Stop();
            await testWv.CoreWebView2.TrySuspendAsync();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
           
        }
    }
}
