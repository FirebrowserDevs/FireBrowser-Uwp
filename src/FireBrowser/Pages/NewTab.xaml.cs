using FireBrowser.ViewModel;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace FireBrowser.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NewTab : Page
    {
        public HomeViewModel ViewModel { get; set; }
        public NewTab()
        {
            this.InitializeComponent();

            ViewModel = new HomeViewModel()
            {
                BackgroundType = Core.Settings.NewTabBackground.None,
            };
        }


        public static Brush GetGridBackgroundAsync(Core.Settings.NewTabBackground backgroundType)
        { 
            switch (backgroundType)
            {
                case Core.Settings.NewTabBackground.None:
                    return new SolidColorBrush(Colors.Transparent);              
            }
            return new SolidColorBrush();
        }
    }
}
