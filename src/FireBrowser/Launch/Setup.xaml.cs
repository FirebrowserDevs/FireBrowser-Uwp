using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Media.Core;
using Windows.Media.Playback;

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

            mediaPlayer = new MediaPlayer();
            mediaPlayer.Source = MediaSource.CreateFromUri(new Uri("ms-appx:///Assets/Launch/firebrowser.mp4"));
            _mediaPlayerElement.SetMediaPlayer(mediaPlayer);

            mediaPlayer.CommandManager.IsEnabled = false;
            mediaPlayer.Play();
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
