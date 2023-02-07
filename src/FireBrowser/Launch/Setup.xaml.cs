using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Windows;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FireBrowser.Launch
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Setup : Page
    {
        public Setup()
        {
            this.InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FrameNext.Visibility = Visibility.Visible;
            FrameNext.Navigate(typeof(SetupSettings));
            _mediaPlayerElement.Visibility = Visibility.Collapsed;
            txt2.Visibility = Visibility.Collapsed;
            txt.Visibility= Visibility.Collapsed;
            btn.Visibility = Visibility.Collapsed;
        }

        private void FrameNext_BringIntoViewRequested(UIElement sender, BringIntoViewRequestedEventArgs args)
        {

        }
    }
}
