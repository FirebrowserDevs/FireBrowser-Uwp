using System;
using Windows.ApplicationModel.Core;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FireBrowser.Launch
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Setup : Page
    {
        MediaPlayer mediaPlayer;
        public Setup()
        {
            this.InitializeComponent();

            var coreTitleBar = CoreApplication.GetCurrentView().TitleBar;
            coreTitleBar.ExtendViewIntoTitleBar = true;
            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;

            var formattableTitleBar = ApplicationView.GetForCurrentView().TitleBar;
            formattableTitleBar.ButtonBackgroundColor = Colors.Transparent;
            formattableTitleBar.ButtonHoverBackgroundColor = Colors.Transparent;
            formattableTitleBar.ButtonInactiveBackgroundColor = Colors.Transparent;
            formattableTitleBar.InactiveBackgroundColor = Colors.Transparent;
            formattableTitleBar.ButtonPressedBackgroundColor = Colors.Transparent;

            Window.Current.SetTitleBar(TitleBar);
            DataContext = this;
        }

        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            TitleBar.MinWidth = (FlowDirection == FlowDirection.LeftToRight) ? sender.SystemOverlayRightInset : sender.SystemOverlayLeftInset;
            TitleBar.Height = sender.Height;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Visibility = Visibility.Visible;
            Frame.Navigate(typeof(SetupSettings), null, new DrillInNavigationTransitionInfo());
        }

        private string _introMessage = @"
• Seamless browsing experience.

• One-click access to favorite websites and a built-in favorites organizer.

• Immersive full-screen mode.

• Prioritizes user convenience.

• Caters to users seeking a user-friendly web browser with advanced features.
";
    }
}

/* INCASE U NEED PREVIOUS TEXT
 * 
 * 
 * FireBrowser is a top-notch web browser that has all the features you need for an effortless and enjoyable browsing experience. 
With FireBrowser, you can easily save your favorite websites to access them with just one click.
The built-in favorites feature allows you to organize your frequently visited sites and save time. Moreover, 
FireBrowser keeps a record of your browsing history, so you can easily revisit any page you have visited in the past.
One of the best things about FireBrowser is its full-screen mode. When you enable full-screen mode, FireBrowser will fill your entire screen,
providing you with an uninterrupted browsing experience. Whether you want to watch a movie or just browse your favorite websites, 
full-screen mode is perfect for both.
 */
