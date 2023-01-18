using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.Json;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FireBrowser.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SiteNoting : Page
    {
        public SiteNoting()
        {
            this.InitializeComponent();
            ViewModel = new NoteViewModel();
            DrawingCanvas.InkPresenter.InputDeviceTypes = Windows.UI.Core.CoreInputDeviceTypes.Pen
            | Windows.UI.Core.CoreInputDeviceTypes.Mouse
            | Windows.UI.Core.CoreInputDeviceTypes.Touch;
        }
        private class Base64Image
        {
            public string data { get; set; }
        }

        public NoteViewModel ViewModel { get; set; }
        public partial class NoteViewModel : ObservableObject
        {
            [ObservableProperty]
            private ImageSource source;
        }
        string base64;
        WriteableBitmap hello;
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            base64 = e.Parameter as string;
            var base64Class = JsonSerializer.Deserialize<Base64Image>(base64);
            var bytes = Convert.FromBase64String(base64Class.data);
            var buf = bytes.AsBuffer();
            var stream = buf.AsStream();
            var image = stream.AsRandomAccessStream();

            var decoder = await BitmapDecoder.CreateAsync(image);
            image.Seek(0);
            WriteableBitmap output;
            output = new((int)decoder.PixelHeight, (int)decoder.PixelWidth);
            await output.SetSourceAsync(image);
            ViewModel.Source = output;
        }

        private void NoteImage_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            var bounds = Window.Current.Bounds;
            this.Width = bounds.Width;
            this.Height = bounds.Height;
        }
        private async void save_Click(object sender, RoutedEventArgs e)
        {
         
        }
    }
}
